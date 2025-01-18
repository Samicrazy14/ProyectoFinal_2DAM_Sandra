package com.example.appproyectofinal_sandra

import android.app.Activity
import android.content.Intent
import android.os.Bundle
import android.util.Log
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.SearchView
import androidx.lifecycle.lifecycleScope
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.appproyectofinal_sandra.activities.PdfAddActivity
import com.example.appproyectofinal_sandra.activities.PdfViewActivity
import com.example.appproyectofinal_sandra.adapters.AdapterPdfUser
import com.example.appproyectofinal_sandra.databinding.FragmentBooksUserBinding
import com.example.appproyectofinal_sandra.models.ModelPdf
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener
import kotlinx.coroutines.*
import org.json.JSONObject
import java.net.URL

class BooksUserFragment : Fragment {

    private lateinit var binding: FragmentBooksUserBinding
    private lateinit var firebaseAuth: FirebaseAuth

    //Datos de la categoría
    private var categoryId = ""
    private var category = ""

    //ArrayList de libros
    private lateinit var pdfArrayList: ArrayList<ModelPdf>
    private lateinit var adapterPdfUser: AdapterPdfUser

    private var currentPage = 1
    private var isLoading = false

    private val PDF_PICKER_CODE = 100

    //Datos del fragment
    companion object{
        private const val TAG = "BOOKS_USER_TAG"

         fun newInstance(categoryId: String, category:  String, uid: String): BooksUserFragment{
            val fragment = BooksUserFragment()

            val args = Bundle()
            args.putString("categoryId", categoryId)
            args.putString("category", category)
            args.putString("uid", uid)

            fragment.arguments = args
            return fragment
        }
    }

    constructor()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        var args = arguments
        if(args != null){
            categoryId = args.getString("categoryId")!!
            category = args.getString("category")!!
        }
    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        binding = FragmentBooksUserBinding.inflate(LayoutInflater.from(context), container, false)
        Log.d(TAG, "onCreateView $category")


        binding.addCategoryBtn.visibility = View.GONE
        binding.addPdfFab.visibility = View.GONE
        binding.filterSwitch.visibility = View.GONE
        binding.viewMyPdfBtn.visibility = View.VISIBLE

        binding.progressBar.visibility = View.VISIBLE
        binding.booksRv.visibility = View.INVISIBLE

        //init firebase auth
        firebaseAuth = FirebaseAuth.getInstance()
        showPullPdf()

        //Rellenar las pestañas
        if(category == "Novedades") {
            loadAllBooks()
        } else if(category == "Most Viewed") {
            loadMostViewedBooks("viewsCount")
        } else if(category == "Obras Públicas") {
            pdfArrayList = ArrayList()
            // Inicializar el adaptador con una lista vacía y asignarlo al RecyclerView
            adapterPdfUser = AdapterPdfUser(requireContext(), pdfArrayList)
            binding.booksRv.adapter = adapterPdfUser

            loadGutembergBooks()
        }else {
            loadCategorizedBooks()
        }

        binding.booksRv.addOnScrollListener(object : RecyclerView.OnScrollListener() {
            override fun onScrolled(recyclerView: RecyclerView, dx: Int, dy: Int) {
                super.onScrolled(recyclerView, dx, dy)
                if(category == "Obras Públicas") {
                    val layoutManager = recyclerView.layoutManager as LinearLayoutManager
                    val visibleItemCount = layoutManager.childCount
                    val totalItemCount = layoutManager.itemCount
                    val firstVisibleItemPosition = layoutManager.findFirstVisibleItemPosition()

                    if (!isLoading && (visibleItemCount + firstVisibleItemPosition) >= totalItemCount
                        && firstVisibleItemPosition >= 0
                    ) {
                        loadMoreGutenbergBooks()
                    }
                }
            }
        })

        //Barra de busqueda de libros
        binding.searchView.setOnQueryTextListener(object : SearchView.OnQueryTextListener {
            override fun onQueryTextSubmit(query: String?): Boolean {
                // Manejar la búsqueda cuando se presione el botón "Buscar"
                return true
            }

            override fun onQueryTextChange(newText: String?): Boolean {
                // Manejar los cambios en el texto de la búsqueda
                adapterPdfUser.filter?.filter(newText)
                return true
            }
        })

        //Añadir libro a pendientes
        binding.addPdfFab.setOnClickListener {
            startActivity(Intent(requireContext(), PdfAddActivity::class.java))
        }

        //Leer Pdf propio
        binding.viewMyPdfBtn.setOnClickListener {
            pickPdfFromDevice()
        }


        return binding.root
    }


    private fun pickPdfFromDevice() {
        val intent = Intent(Intent.ACTION_OPEN_DOCUMENT).apply {
            addCategory(Intent.CATEGORY_OPENABLE)
            type = "application/pdf"
        }
        startActivityForResult(intent, PDF_PICKER_CODE)
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if (requestCode == PDF_PICKER_CODE && resultCode == Activity.RESULT_OK) {
            data?.data?.let { uri ->
                // Guardar acceso persistente al archivo
                requireContext().contentResolver.takePersistableUriPermission(
                    uri,
                    Intent.FLAG_GRANT_READ_URI_PERMISSION
                )

                // Iniciar PdfViewActivity con el URI del archivo local
                val intent = Intent(context, PdfViewActivity::class.java).apply {
                    putExtra("isLocalPdf", true)
                    putExtra("localPdfUri", uri.toString())
                }
                startActivity(intent)
            }
        }
    }

    /**
     * Muestra la opción de subir pdf si eres author
     */
    private fun showPullPdf() {
        if(firebaseAuth.currentUser != null){
            val firebaseUser = firebaseAuth.currentUser!!
            val ref = FirebaseDatabase.getInstance().getReference("Users")
            ref.child(firebaseUser.uid)
                .addListenerForSingleValueEvent(object : ValueEventListener {
                    override fun onDataChange(snapshot: DataSnapshot) {
                        val userType = snapshot.child("userType").value
                        if (userType == "author"){
                            binding.addPdfFab.visibility = View.VISIBLE
                        }
                    }
                    override fun onCancelled(error: DatabaseError) {
                    }
                })
        }
    }

    /**
     * Cargar los datos de la pestaña todos los libros
     */
    private fun loadAllBooks() {
       pdfArrayList = ArrayList()
        val ref = FirebaseDatabase.getInstance().getReference("Books")
        ref.addValueEventListener(object: ValueEventListener{
            override fun onDataChange(snapshot: DataSnapshot) {
                pdfArrayList.clear()
                for (ds in snapshot.children){
                    val model = ds.getValue(ModelPdf::class.java)
                    pdfArrayList.add(model!!)
                }

                // Invertir el orden de la lista
                pdfArrayList.reverse()

                adapterPdfUser = AdapterPdfUser(context!!, pdfArrayList)
                binding.booksRv.adapter = adapterPdfUser
                binding.progressBar.visibility = View.GONE
                binding.booksRv.visibility = View.VISIBLE
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Cargar los datos de la pestaña los libros más vistos
     */
    private fun loadMostViewedBooks(orderBy: String) {
        pdfArrayList = ArrayList()
        val ref = FirebaseDatabase.getInstance().getReference("Books")
        ref.orderByChild(orderBy)
            .addValueEventListener(object: ValueEventListener{
            override fun onDataChange(snapshot: DataSnapshot) {
                pdfArrayList.clear()
                for (ds in snapshot.children){
                    val model = ds.getValue(ModelPdf::class.java)
                    pdfArrayList.add(model!!)
                }

                // Ordenar manualmente por el atributo deseado en orden descendente
                pdfArrayList.sortByDescending { it.viewsCount }

                adapterPdfUser = AdapterPdfUser(context!!, pdfArrayList)
                binding.booksRv.adapter = adapterPdfUser
                binding.progressBar.visibility = View.GONE
                binding.booksRv.visibility = View.VISIBLE
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Cargar los datos de la pestaña de libros de gutemberg
     */
    private fun loadGutembergBooks() {
        lifecycleScope.launch {
            getGutenbergBooks()
        }
    }

    /**
     * Recoge los datos de los libros de la API de gutemberg
     */
    private suspend fun getGutenbergBooks() {
        // Cambiar a Dispatchers.IO para las solicitudes de red
        withContext(Dispatchers.IO) {
            try {
                // Creamos una lista de Deferred para manejar las tareas en paralelo
                val deferredBooks = (1..10).map { i ->
                    async {
                        val bookId = (currentPage - 1) * 10 + i
                        val url = "https://gutendex.com/books/?ids=$bookId"
                        try {
                            val jsonString = URL(url).readText()
                            val jsonObject = JSONObject(jsonString)
                            val resultsArray = jsonObject.getJSONArray("results")

                            if (resultsArray.length() > 0) {
                                val bookObject = resultsArray.getJSONObject(0)
                                val formats = bookObject.getJSONObject("formats")

                                val authors = bookObject.getJSONArray("authors")
                                val author = if (authors.length() > 0) authors.getJSONObject(0)
                                    .getString("name") else "Unknown"

                                val bookshelves = bookObject.getJSONArray("bookshelves")
                                val categories = mutableListOf<String>()
                                for (j in 0 until bookshelves.length()) {
                                    var category = bookshelves.getString(j)
                                    if (category.startsWith("Browsing: ")) {
                                        category = category.substring("Browsing: ".length)
                                    }
                                    categories.add(category)
                                    break  // Rompe el ciclo después de agregar la primera categoría
                                }
                                val categoryId = categories.joinToString("")
                                //id
                                val id = bookObject.getString("id")
                                //imageUrl
                                val imagenUrl = formats.optString("image/jpeg", "")
                                //titulo
                                val title = bookObject.getString("title")
                                //ID DE USUARIO
                                val uid = "Project Gutenberg"
                                //url contenido
                                val bookUrl = formats.optString("application/epub+zip", "")
                                //visitas
                                val viewsCount = bookObject.getString("download_count").toLong()

                                // GOOGLEAPI: obtener descripción, fecha de publicación y número de páginas
                                val (description, publishedDate, pageCount) = getGoogleApiBookInfo(title)

                                // Crear y devolver el objeto del libro
                                ModelPdf().apply {
                                    this.author = author
                                    this.categoryId = categoryId
                                    this.description = description
                                    this.id = id
                                    this.imagenUrl = imagenUrl
                                    this.pagecount = pageCount.toLong()
                                    this.timestamp = MyApplication.yearToMillis(publishedDate)
                                    this.title = title
                                    this.uid = uid
                                    this.url = bookUrl
                                    this.viewsCount = viewsCount
                                }
                            } else {
                                null
                            }
                        } catch (e: Exception) {
                            e.printStackTrace()
                            null
                        }
                    }
                }

                // Esperar a que todas las tareas se completen y filtrar los resultados nulos
                val books = deferredBooks.awaitAll().filterNotNull()

                // Cambiar al hilo principal para actualizar la interfaz de usuario
                withContext(Dispatchers.Main) {
                    pdfArrayList.addAll(books)
                    adapterPdfUser.notifyItemRangeInserted(pdfArrayList.size - books.size, books.size)

                    // Esconder el progressBar y mostrar los libros
                    binding.progressBar.visibility = View.GONE
                    binding.booksRv.visibility = View.VISIBLE
                    isLoading = false
                    currentPage++
                }
            } catch (e: Exception) {
                e.printStackTrace()
                withContext(Dispatchers.Main) {
                    binding.progressBar.visibility = View.GONE
                    isLoading = false
                }
            }
        }
    }

    /**
     * Carga más libros de gutemberg
     */
    private fun loadMoreGutenbergBooks() {
        isLoading = true
        binding.progressBar.visibility = View.VISIBLE
        viewLifecycleOwner.lifecycleScope.launch {
            getGutenbergBooks()
        }
    }

    /**
     * Consulta a Google API Books datos extra de los libros
     */
    private suspend fun getGoogleApiBookInfo(title: String): Triple<String, String, Int> {
        val formattedTitle = title.replace(" ", "+")
        val url = "https://www.googleapis.com/books/v1/volumes?q=$formattedTitle"

        return withContext(Dispatchers.IO) {
            val response = URL(url).readText()
            val jsonObject = JSONObject(response)
            val items = jsonObject.getJSONArray("items")

            for (i in 0 until items.length()) {
                val volumeInfo = items.getJSONObject(i).getJSONObject("volumeInfo")

                val description = volumeInfo.optString("description", "")
                val publishedDate = volumeInfo.optString("publishedDate", "")
                val pageCount = volumeInfo.optInt("pageCount", 0)

                if (description.isNotEmpty() && publishedDate.isNotEmpty() && pageCount > 0) {
                    return@withContext Triple(description, publishedDate, pageCount)
                }
            }

            Triple("", "", 0) // Si no se encuentra información completa
        }
    }


    /**
     * Cargar los datos de la pestaña de una categoría de libros
     */
    private fun loadCategorizedBooks() {
        pdfArrayList = ArrayList()
        val ref = FirebaseDatabase.getInstance().getReference("Books")
        ref.orderByChild("categoryId").equalTo(categoryId)
            .addValueEventListener(object: ValueEventListener{
                override fun onDataChange(snapshot: DataSnapshot) {
                    pdfArrayList.clear()
                    for (ds in snapshot.children){
                        val model = ds.getValue(ModelPdf::class.java)
                        pdfArrayList.add(model!!)
                    }

                    adapterPdfUser = AdapterPdfUser(context!!, pdfArrayList)
                    binding.booksRv.adapter = adapterPdfUser
                    binding.progressBar.visibility = View.GONE
                    binding.booksRv.visibility = View.VISIBLE
                }
                override fun onCancelled(error: DatabaseError) {
                }
            })
    }
}
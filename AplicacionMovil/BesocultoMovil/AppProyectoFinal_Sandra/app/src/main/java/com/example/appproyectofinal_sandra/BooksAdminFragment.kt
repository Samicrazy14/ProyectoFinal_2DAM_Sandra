package com.example.appproyectofinal_sandra

import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.SearchView
import androidx.fragment.app.Fragment
import com.example.appproyectofinal_sandra.activities.CategoryAddActivity
import com.example.appproyectofinal_sandra.activities.PdfAddActivity
import com.example.appproyectofinal_sandra.adapters.AdapterCategory
import com.example.appproyectofinal_sandra.adapters.AdapterPdfAdmin
import com.example.appproyectofinal_sandra.adapters.AdapterPendingPdf
import com.example.appproyectofinal_sandra.adapters.AdapterUser
import com.example.appproyectofinal_sandra.databinding.FragmentBooksUserBinding
import com.example.appproyectofinal_sandra.models.ModelCategory
import com.example.appproyectofinal_sandra.models.ModelPdf
import com.example.appproyectofinal_sandra.models.ModelUser
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener

class BooksAdminFragment : Fragment {

    private lateinit var binding: FragmentBooksUserBinding

    //Datos de la categoría
    private var categoryId = ""
    private var category = ""

    //ArrayList de libros
    private lateinit var pdfArrayList: ArrayList<ModelPdf>
    private lateinit var adapterPdfAdmin: AdapterPdfAdmin
    private lateinit var adapterPendingPdf: AdapterPendingPdf

    //ArrayList de categorias
    private lateinit var adapterCategory: AdapterCategory
    private lateinit var categoryArrayList: ArrayList<ModelCategory>

    //ArrayList de usuarios
    private lateinit var adapterUser: AdapterUser
    private lateinit var userArrayList: ArrayList<ModelUser>

    //Datos del fragment
    companion object{
        private const val TAG = "BOOKS_USER_TAG"

         fun newInstance(categoryId: String, category:  String, uid: String): BooksAdminFragment{
            val fragment = BooksAdminFragment()

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

        binding.progressBar.visibility = View.VISIBLE
        binding.booksRv.visibility = View.INVISIBLE

        //Rellenar las pestañas
        if(category == "Categories") {
            binding.addCategoryBtn.visibility = View.VISIBLE
            binding.addPdfFab.visibility = View.GONE
            binding.filterSwitch.visibility = View.GONE
            binding.viewMyPdfBtn.visibility = View.GONE
            loadCategories()//load categories
        } else if(category == "Books") {
            binding.addCategoryBtn.visibility = View.GONE
            binding.addPdfFab.visibility = View.VISIBLE
            binding.filterSwitch.visibility = View.VISIBLE
            binding.viewMyPdfBtn.visibility = View.GONE
            loadAllBooks()//load books
        } else {
            binding.addCategoryBtn.visibility = View.GONE
            binding.addPdfFab.visibility = View.GONE
            binding.filterSwitch.visibility = View.VISIBLE
            binding.viewMyPdfBtn.visibility = View.GONE
            loadUsers()//load users
        }

        //Añadir categoría
        binding.addCategoryBtn.setOnClickListener {
            startActivity(Intent(requireContext(), CategoryAddActivity::class.java))
        }

        //Añadir libro
        binding.addPdfFab.setOnClickListener {
            startActivity(Intent(requireContext(), PdfAddActivity::class.java))
        }

        //Filtrar usuarios y libros por pendientes
        binding.filterSwitch.setOnCheckedChangeListener { _, isChecked ->
            if(category == "Books") {
                if (isChecked) {
                    loadAllPendingBooks()// Filtrar por "pending" books
                } else {
                    loadAllBooks()
                }
            } else {
                if (isChecked) {
                    loadPendingUsers() // Filtrar por "pending" users
                } else {
                    loadUsers() // Show all users
                }
            }
        }

        //Barra de busqueda de libros
        binding.searchView.setOnQueryTextListener(object : SearchView.OnQueryTextListener {
            override fun onQueryTextSubmit(query: String?): Boolean {
                // Manejar la búsqueda cuando se presione el botón "Buscar"
                return true
            }

            override fun onQueryTextChange(newText: String?): Boolean {
                // Manejar los cambios en el texto de la búsqueda
                if(category == "Categories") {
                    adapterCategory.filter?.filter(newText)
                } else if(category == "Books") {
                    adapterPdfAdmin.filter?.filter(newText)
                } else {
                    adapterUser.filter?.filter(newText)
                }
                return true
            }
        })

        return binding.root
    }

    /**
     * Carga los datos de la pestaña categorias
     */

    private fun loadCategories() {
        categoryArrayList = ArrayList()

        val ref = FirebaseDatabase.getInstance().getReference("Categories")
        ref.addValueEventListener(object : ValueEventListener {
            override fun onDataChange(snapshot: DataSnapshot) {
                categoryArrayList.clear()
              for (ds in snapshot.children){
                  val model = ds.getValue(ModelCategory::class.java)
                  categoryArrayList.add(model!!)
              }

                adapterCategory = AdapterCategory(requireContext(), categoryArrayList)
                binding.booksRv.adapter = adapterCategory
                binding.progressBar.visibility = View.GONE
                binding.booksRv.visibility = View.VISIBLE
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Cargar los datos de la pestaña libros con libros
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

                adapterPdfAdmin = AdapterPdfAdmin(context!!, pdfArrayList)
                binding.booksRv.adapter = adapterPdfAdmin
                binding.progressBar.visibility = View.GONE
                binding.booksRv.visibility = View.VISIBLE
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Cargar los datos de la pestaña libros con libros pendientes
     */
    private fun loadAllPendingBooks() {
        pdfArrayList = ArrayList()
        val ref = FirebaseDatabase.getInstance().getReference("PendingBooks")
        ref.addValueEventListener(object: ValueEventListener{
            override fun onDataChange(snapshot: DataSnapshot) {
                pdfArrayList.clear()
                for (ds in snapshot.children){
                    val model = ds.getValue(ModelPdf::class.java)
                    pdfArrayList.add(model!!)
                }

                adapterPendingPdf = AdapterPendingPdf(context!!, pdfArrayList)
                binding.booksRv.adapter = adapterPendingPdf
                binding.progressBar.visibility = View.GONE
                binding.booksRv.visibility = View.VISIBLE
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Cargar los datos de la pestaña usuarios con usuarios
     */
    private fun loadUsers() {
        userArrayList = ArrayList()
        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.addValueEventListener(object: ValueEventListener{
            override fun onDataChange(snapshot: DataSnapshot) {
                userArrayList.clear()
                for (ds in snapshot.children){
                    val model = ds.getValue(ModelUser::class.java)
                    if (model != null && model.userType != "admin") { // Filtra los usuarios que no son admin
                        userArrayList.add(model)
                    }
                }

                adapterUser  = AdapterUser(requireContext(), userArrayList)
                binding.booksRv.adapter = adapterUser
                binding.progressBar.visibility = View.GONE
                binding.booksRv.visibility = View.VISIBLE
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Cargar los datos de la pestaña usuarios con usuarios pendientes
     */
    private fun loadPendingUsers() {
        userArrayList = ArrayList()
        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.addValueEventListener(object: ValueEventListener{
            override fun onDataChange(snapshot: DataSnapshot) {
                userArrayList.clear()
                for (ds in snapshot.children){
                    val model = ds.getValue(ModelUser::class.java)
                    if (model != null && model.userType == "pending user") {
                        // Añadir solo los usuarios con userType "pending user"
                        userArrayList.add(model!!)
                    }
                }

                adapterUser  = AdapterUser(requireContext(), userArrayList)
                binding.booksRv.adapter = adapterUser
                binding.progressBar.visibility = View.GONE
                binding.booksRv.visibility = View.VISIBLE
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }
}
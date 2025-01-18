package com.example.appproyectofinal_sandra.activities

import android.app.ProgressDialog
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import com.bumptech.glide.Glide
import com.example.appproyectofinal_sandra.MyApplication
import com.example.appproyectofinal_sandra.R
import com.example.appproyectofinal_sandra.adapters.AdapterComment
import com.example.appproyectofinal_sandra.databinding.ActivityPdfDetailBinding
import com.example.appproyectofinal_sandra.databinding.DialogCommentAddBinding
import com.example.appproyectofinal_sandra.models.ModelComment
import com.folioreader.FolioReader
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener
import kotlinx.coroutines.CoroutineScope
import kotlinx.coroutines.Dispatchers
import kotlinx.coroutines.launch
import kotlinx.coroutines.withContext
import okhttp3.OkHttpClient
import java.io.File
import java.io.FileOutputStream
import java.io.IOException
import java.io.InputStream

class PdfDetailActivity : AppCompatActivity() {

    private lateinit var binding: ActivityPdfDetailBinding
    private lateinit var firebaseAuth: FirebaseAuth
    private lateinit var progressDialog: ProgressDialog
    val TAG = "PDF_DETAIL_TAG"

    //Variables del libro
    private var bookId = ""
    private var type = ""
    private var urlEpub = ""
    private var isInMyFavorite = false

    //Variables de comentarios
    private var comment = ""
    private lateinit var commentArrayList: ArrayList<ModelComment>
    private lateinit var adapterComment: AdapterComment

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityPdfDetailBinding.inflate(layoutInflater)
        setContentView(binding.root)

        //init progress
        progressDialog = ProgressDialog(this)
        progressDialog.setTitle("Por favor, espere")
        progressDialog.setCanceledOnTouchOutside(false)

        bookId = intent.getStringExtra("bookId")!!
        type = intent.getStringExtra("type")!!

        binding.progressAllBar.visibility = View.GONE

        //init firebaseAuth
        firebaseAuth = FirebaseAuth.getInstance()
        if(firebaseAuth.currentUser != null){
            checkIsfavorite()
        }

        MyApplication.incrementBookViewCount(bookId, type)
        loadBookDetails(type)
        showComments(type)

        //Salir
        binding.backBtn.setOnClickListener {
            onBackPressed()
        }

        //Ver contenido del libro
        binding.readBookBtn.setOnClickListener {
            if (type != "GutembergBooks") {
                val intent = Intent(this, PdfViewActivity::class.java)
                intent.putExtra("bookId", bookId)
                intent.putExtra("type", type)
                startActivity(intent)
            } else {
                Log.d(TAG,"readBookBtn: Project Gutenberg url: $urlEpub")
                if (!urlEpub.isNullOrEmpty()){
                    binding.progressAllBar.visibility = View.VISIBLE
                    val epubFile = File(getExternalFilesDir(null), "book.epub")
                    downloadEpubFile(urlEpub, epubFile)
                }
            }
        }

        //Añadir a favoritos
        binding.favoriteBtn.setOnClickListener {
           if (firebaseAuth.currentUser == null){
               Toast.makeText(this, "No estas logeado", Toast.LENGTH_SHORT).show()
           } else {
               if (isInMyFavorite){
                   MyApplication.removeFromfavorite(this, bookId)
               }else{
                   addTofavorite()
               }
           }
        }

        //Añadir comentario
        binding.addCommentBtn.setOnClickListener {
            if (firebaseAuth.currentUser == null){
                Toast.makeText(this, "No estas logeado", Toast.LENGTH_SHORT).show()
            } else {
                addCommentDialog()
            }
        }
    }

    /**
     * Comprobar y mostrar si el libro esta en favoritos
     */
    private fun checkIsfavorite(){
        Log.d(TAG, "checkIsfavorite")

        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.child(firebaseAuth.uid!!).child("Favorites").child(bookId)
            .addValueEventListener(object: ValueEventListener{
                override fun onDataChange(snapshot: DataSnapshot) {
                    isInMyFavorite = snapshot.exists()
                    if (isInMyFavorite){
                        Log.d(TAG, "checkIsfavorite: onDataChange: en favoritos")
                        binding.favoriteBtn.setCompoundDrawablesRelativeWithIntrinsicBounds(0,
                            R.drawable.ic_favorite_filled_white, 0, 0)
                        binding.favoriteBtn.text = "Eliminar de favoritos"
                    }else{
                        Log.d(TAG, "checkIsfavorite: onDataChange: no esta en favoritos")
                        binding.favoriteBtn.setCompoundDrawablesRelativeWithIntrinsicBounds(0,
                            R.drawable.ic_favorite_border, 0, 0)
                        binding.favoriteBtn.text = "Añadir a favoritos"
                    }
                }
                override fun onCancelled(error: DatabaseError) {
                }
            })
    }

    /**
     * Cargar los datos del libro
     */
    private fun loadBookDetails(type: String) {
        val ref = FirebaseDatabase.getInstance().getReference(type)
        ref.child(bookId)
            .addListenerForSingleValueEvent(object: ValueEventListener{
                override fun onDataChange(snapshot: DataSnapshot) {
                    val categoryId = "${snapshot.child("categoryId").value}"
                    val description = "${snapshot.child("description").value}"
                    val timestamp = "${snapshot.child("timestamp").value}"
                    val title = "${snapshot.child("title").value}"
                    val url = "${snapshot.child("url").value}"
                    val viewsCount = "${snapshot.child("viewsCount").value}"
                    val pagesCount = "${snapshot.child("pagecount").value}"
                    val date = MyApplication.formatTimeStamp(timestamp.toLong())

                    if (type != "GutembergBooks"){
                        MyApplication.loadCategory(categoryId, binding.categoryTv)

                        binding.pdfView.visibility = View.VISIBLE
                        binding.gutembergIv.visibility = View.INVISIBLE
                        MyApplication.loadPdfFromUrlSinglePage(
                            "$url",
                            binding.pdfView,
                            binding.progressBar,
                            null
                        )
                        binding.pagesTv.text = pagesCount
                    } else {
                        binding.categoryTv.text = categoryId
                        binding.pdfView.visibility = View.INVISIBLE
                        binding.gutembergIv.visibility = View.VISIBLE

                        urlEpub = url

                        val imagenUrl = "${snapshot.child("imagenUrl").value}"
                        try {
                            Glide.with(this@PdfDetailActivity)
                                .load(imagenUrl)
                                .placeholder(R.drawable.ic_person_gray)
                                .into(binding.gutembergIv)
                            binding.progressBar.visibility = View.INVISIBLE
                        } catch (e: Exception){
                            binding.gutembergIv.setImageResource(R.drawable.ic_person_gray)
                            binding.progressBar.visibility = View.INVISIBLE
                        }
                        binding.pagesTv.text = pagesCount
                    }


                    binding.titleTv.text = title
                    binding.descriptionTv.text = description
                    binding.viewTv.text = viewsCount
                    binding.dateTv.text = date
                }
                override fun onCancelled(error: DatabaseError) {
                }
            })
    }

    /**
     * Cargar los comentarios del libro
     */
    private fun showComments(type: String) {
        commentArrayList = ArrayList()

        val ref = FirebaseDatabase.getInstance().getReference(type)
        ref.child(bookId).child("Comments")
            .addValueEventListener(object: ValueEventListener{
            override fun onDataChange(snapshot: DataSnapshot) {
                commentArrayList.clear()
                for (ds in snapshot.children){
                    val model = ds.getValue(ModelComment::class.java)
                    commentArrayList.add(model!!)
                }

                adapterComment = AdapterComment(this@PdfDetailActivity, commentArrayList, type)
                binding.commentsRv.adapter = adapterComment
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Descargamos y visualizamos el epub
     */
    fun downloadEpubFile(url: String, file: File) {
        CoroutineScope(Dispatchers.IO).launch {
            try {
                val client = OkHttpClient()
                val request = okhttp3.Request.Builder().url(url).build()

                client.newCall(request).execute().use { response ->
                    if (!response.isSuccessful) throw IOException("Failed to download file: $response")

                    val inputStream: InputStream? = response.body?.byteStream()
                    val outputStream = FileOutputStream(file)
                    inputStream?.copyTo(outputStream)
                    outputStream.close()
                }

                // Una vez que el archivo se descargó correctamente
                println("Descarga completa: ${file.absolutePath}")
                withContext(Dispatchers.Main) {
                    binding.progressAllBar.visibility = View.GONE

                    // Inicializar FolioReader
                    var folioReader = FolioReader.get()

                    // Abrir el archivo EPUB
                    folioReader.openBook(file.absolutePath)
                }
            } catch (e: Exception) {
                e.printStackTrace()
            }
        }
    }

    /**
     * Añadir a favoritos
     */
    private fun addTofavorite(){
        Log.d(TAG, "addTofavorite")
        val timestamp = System.currentTimeMillis()
        val hashMap = HashMap<String, Any>()
        hashMap["bookId"] = bookId
        hashMap["timestamp"] = timestamp
        hashMap["type"] = type

        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.child(firebaseAuth.uid!!).child("Favorites").child(bookId)
            .setValue(hashMap)
            .addOnSuccessListener {
                Log.d(TAG, "addTofavorite: Success")
                Toast.makeText(this, "Añadido a favoritos", Toast.LENGTH_SHORT)
                    .show()
            }
            .addOnFailureListener{ e ->
                Log.d(TAG, "addTofavorite: Fallo al añadir a favoritos: ${e.message}")
                Toast.makeText(this, "Fallo al añadir a favoritos: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }
    }

    /**
     * Dialogo de añadir comentario
     */
    private fun addCommentDialog() {
        val commentAddBinding = DialogCommentAddBinding.inflate(LayoutInflater.from(this))
        val builder = AlertDialog.Builder(this, R.style.CustomDialog)
        builder.setView(commentAddBinding.root)
        val alertDialog = builder.create()
        alertDialog.show()

        //Salir del diálogo
        commentAddBinding.backBtn.setOnClickListener {
            alertDialog.dismiss()
        }

        //Enviar el comentario
        commentAddBinding.submitBtn.setOnClickListener{
            comment = commentAddBinding.commentEt.text.toString().trim()

            if(comment.isEmpty()){
                Toast.makeText(this, "Introduce un comentario...", Toast.LENGTH_SHORT).show()
            }else {
                alertDialog.dismiss()
                addComment(type)
            }
        }
    }

    /**
     * Añadir comentario al libro
     */
    private fun addComment(type: String) {
        progressDialog.setMessage("Añadiendo comentario")
        progressDialog.show()

        val timestamp = "${System.currentTimeMillis()}"
        val hashMap = HashMap<String, Any>()
        hashMap["id"] = "$timestamp"
        hashMap["bookId"] = "$bookId"
        hashMap["timestamp"] = "$timestamp"
        hashMap["comment"] = "$comment"
        hashMap["uid"] = "${firebaseAuth.uid}"

        val ref = FirebaseDatabase.getInstance().getReference(type)
        ref.child(bookId).child("Comments").child(timestamp)
            .setValue(hashMap)
            .addOnSuccessListener {
                progressDialog.dismiss()
                Toast.makeText(this, "Comentario añadido", Toast.LENGTH_SHORT)
                    .show()
            }
            .addOnFailureListener{ e ->
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al añadir el comentario: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }
    }
}
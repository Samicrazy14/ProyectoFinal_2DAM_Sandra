package com.example.appproyectofinal_sandra.activities

import android.net.Uri
import android.os.Bundle
import android.util.Log
import android.view.View
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.appproyectofinal_sandra.Constants
import com.example.appproyectofinal_sandra.databinding.ActivityPdfViewBinding
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener
import com.google.firebase.storage.FirebaseStorage

class PdfViewActivity : AppCompatActivity() {

    private lateinit var binding: ActivityPdfViewBinding
    private companion object{
        const val TAG ="PDF_VIEW_TAG"
    }

    //Identificador del libro
    var bookId = ""
    var type = ""
    var isLocalPdf = false

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityPdfViewBinding.inflate(layoutInflater)
        setContentView(binding.root)

        //Leer archivo dispositivo
        isLocalPdf = intent.getBooleanExtra("isLocalPdf", false)

        if (isLocalPdf) {
            val localPdfUri = intent.getStringExtra("localPdfUri")
            localPdfUri?.let {
                loadBookFromLocalUri(Uri.parse(it))
            }
        } else {
            bookId = intent.getStringExtra("bookId")!!
            type = intent.getStringExtra("type")!!
            loadBookDetails()
        }

        //Salir de la visualización del libro
        binding.backBtn.setOnClickListener {
            onBackPressed()
        }
    }

    /**
     * Obtener url del libro
     */
    private fun loadBookDetails() {
        Log.d(TAG,"loadBookDetails get url")

        val ref = FirebaseDatabase.getInstance().getReference(type)
        ref.child(bookId)
            .addListenerForSingleValueEvent(object : ValueEventListener {
            override fun onDataChange(snapshot: DataSnapshot) {
                val pdfUrl = snapshot.child("url").value
                Log.d(TAG,"onDataChange: FIREBASE url: $pdfUrl")

                loadBookFromUrl("$pdfUrl")
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Cargar contenido del libro
     */
    private fun loadBookFromUrl(pdfUrl: String) {
        Log.d(TAG,"loadBookFromUrl get pdf from storage")

        val reference = FirebaseStorage.getInstance().getReferenceFromUrl(pdfUrl)
        reference.getBytes(Constants.MAX_BYTES_PDF)
            .addOnSuccessListener {bytes ->
                Log.d(TAG,"loadBookFromUrl: pdf conseguido")

                //Configurar vista libro
                binding.pdfView.fromBytes(bytes)
                    .swipeHorizontal(false)
                    .onPageChange { page, pageCount ->
                        val currentPage = page+1
                        binding.toolbarSubTitleTv.text = "$currentPage/$pageCount"
                        Log.d(TAG,"loadBookFromUrl: $currentPage/$pageCount")
                    }
                    .onError { t ->
                        Log.d(TAG,"loadBookFromUrl: ${t.message}")
                    }
                    .onPageError { page, t ->
                        Log.d(TAG,"loadBookFromUrl: ${t.message}")
                    }
                    .load()
                binding.progressBar.visibility = View.GONE
            }
            .addOnFailureListener{ e ->
                Log.d(TAG,"loadBookFromUrl: Fallo al coger el pdf: ${e.message}")
                binding.progressBar.visibility = View.GONE
            }
    }

    /**
     * Cargar contenido del libro desde dispositivo
     */
    private fun loadBookFromLocalUri(uri: Uri) {
        try {
            binding.progressBar.visibility = View.VISIBLE

            binding.pdfView.fromUri(uri)
                .swipeHorizontal(false)
                .onPageChange { page, pageCount ->
                    val currentPage = page + 1
                    binding.toolbarSubTitleTv.text = "$currentPage/$pageCount"
                }
                .onError { t ->
                    binding.progressBar.visibility = View.GONE
                    Log.d(TAG, "Error al cargar PDF local: ${t.message}")
                    Toast.makeText(this, "Error al cargar PDF", Toast.LENGTH_LONG).show()
                }
                .onPageError { page, t ->
                    binding.progressBar.visibility = View.GONE
                    Log.d(TAG, "Error en página $page: ${t.message}")
                }
                .load()

            binding.progressBar.visibility = View.GONE
        } catch (e: Exception) {
            binding.progressBar.visibility = View.GONE
            Log.d(TAG, "Error al cargar PDF local: ${e.message}")
            Toast.makeText(this, "Error al cargar PDF", Toast.LENGTH_LONG).show()
        }
    }
}
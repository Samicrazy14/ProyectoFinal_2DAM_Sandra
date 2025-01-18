package com.example.appproyectofinal_sandra.activities

import android.app.ProgressDialog
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.util.Log
import android.widget.Toast
import androidx.activity.result.ActivityResult
import androidx.activity.result.ActivityResultCallback
import androidx.activity.result.contract.ActivityResultContracts
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import com.example.appproyectofinal_sandra.databinding.ActivityPdfAddBinding
import com.example.appproyectofinal_sandra.models.ModelCategory
import com.google.android.gms.tasks.Task
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener
import com.google.firebase.storage.FirebaseStorage

class PdfAddActivity : AppCompatActivity() {

    private lateinit var binding: ActivityPdfAddBinding
    private lateinit var firebaseAuth: FirebaseAuth
    private lateinit var progressDialog: ProgressDialog
    private val TAG = "PDF_ADD_TAG"

    //Lista de categorias disponibles
    private lateinit var categoryArrayList: ArrayList<ModelCategory>

    //Datos de categoría seleecionada
    private var selectedCategoryId = ""
    private var selectedCategoryTitle = ""

    //Datos del libro
    private var pdfUri: Uri? = null
    private var title = ""
    private var description = ""
    private var category = ""
    private var pagecount = 0

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityPdfAddBinding.inflate(layoutInflater)
        setContentView(binding.root)

        //init firebase auth
        firebaseAuth = FirebaseAuth.getInstance()

        //init progress
        progressDialog = ProgressDialog(this)
        progressDialog.setTitle("Por favor, espere")
        progressDialog.setCanceledOnTouchOutside(false)

        loadPdfCategories()

        //Salir
        binding.backBtn.setOnClickListener{
            onBackPressed()
        }

        //Diálogo de selección de categoría
        binding.categoryTv.setOnClickListener {
            categoryPickDialog()
        }

        //Añadir pdf
        binding.attachPdfBtn.setOnClickListener {
            pdfPickIntent()
        }

        //Validar datos del libro
        binding.submitBtn.setOnClickListener {
            validateData()
        }
    }

    /**
     * Cargar categorías disponibles
     */
    private fun loadPdfCategories() {
        Log.d(TAG,"loadPdfCategories")
        categoryArrayList = ArrayList()

        val ref = FirebaseDatabase.getInstance().getReference("Categories")
        ref.addListenerForSingleValueEvent(object : ValueEventListener {
            override fun onDataChange(snapshot: DataSnapshot) {
                categoryArrayList.clear()
                for(ds in snapshot.children){
                    val model = ds.getValue(ModelCategory::class.java)
                    categoryArrayList.add(model!!)
                    Log.d(TAG,"onDataChange: ${model.category}")
                }
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Dialogo de selección de categoría
     */
    private fun categoryPickDialog() {
        Log.d(TAG,"categoryPickDialog")

        val categoriesArray = arrayOfNulls<String>(categoryArrayList.size)
        for (i in categoryArrayList.indices){
            categoriesArray[i] = categoryArrayList[i].category
        }

        val builder = AlertDialog.Builder(this)
        builder.setTitle("Elige categoria")
            .setItems(categoriesArray){dialog, which ->
                selectedCategoryTitle = categoryArrayList[which].category
                selectedCategoryId = categoryArrayList[which].id

                binding.categoryTv.text = selectedCategoryTitle

                Log.d(TAG,"categoryPickDialog id: $selectedCategoryId")
                Log.d(TAG,"categoryPickDialog Title: $selectedCategoryTitle")
            }
            .show()
    }

    /**
     * Añadir archivo pdf
     */
    private fun pdfPickIntent() {
        Log.d(TAG,"pdfPickIntent")

        val intent = Intent()
        intent.type ="application/pdf"
        intent.action = Intent.ACTION_GET_CONTENT
        pdfActivityResultLauncher.launch(intent)
    }

    val pdfActivityResultLauncher = registerForActivityResult(
        ActivityResultContracts.StartActivityForResult(),
        ActivityResultCallback<ActivityResult> { result ->
            if (result.resultCode == RESULT_OK){
                Log.d(TAG,"PDF aceptado")
                pdfUri = result.data!!.data
                getPdfPageCount(pdfUri!!)
                Toast.makeText(this, "Pdf selecionado", Toast.LENGTH_SHORT).show()
            } else {
                Log.d(TAG,"PDF cancelado")
                Toast.makeText(this, "Cancelado", Toast.LENGTH_SHORT).show()
            }
        }
    )

    private fun getPdfPageCount(pdfUri: Uri) {
        binding.pdfView.fromUri(pdfUri)
            .swipeHorizontal(false)
            .onPageChange { page, pageCount ->
                pagecount = pageCount
                Toast.makeText(this, "El PDF tiene $pageCount páginas.", Toast.LENGTH_SHORT).show()
            }
            .onError { throwable ->
                Log.e(TAG, "Error al cargar el PDF: ${throwable.message}")
                Toast.makeText(this, "Error al cargar el PDF: ${throwable.message}", Toast.LENGTH_SHORT).show()
            }
            .load()
    }

    /**
     * Validar datos del libro
     */
    private fun validateData() {
        Log.d(TAG,"validateData")

        title = binding.titleEt.text.toString().trim()
        description = binding.descriptionEt.text.toString().trim()
        category = binding.categoryTv.text.toString().trim()

        if (title.isEmpty()) {
            Toast.makeText(this, "Introduce el título", Toast.LENGTH_SHORT).show()
        } else if (description.isEmpty()) {
            Toast.makeText(this, "Introduce la descripción", Toast.LENGTH_SHORT).show()
        } else if (category.isEmpty()) {
            Toast.makeText(this, "Selecciona una categoría", Toast.LENGTH_SHORT).show()
        } else if (pdfUri == null) {
            Toast.makeText(this, "Selecciona un PDF", Toast.LENGTH_SHORT).show()
        }else {
            uploadPdfToStorage()
        }
    }

    /**
     * Subir archivo pdf a storage
     */
    private fun uploadPdfToStorage() {
        Log.d(TAG,"uploadPdfToStorage")
        progressDialog.setMessage("Subiendo Pdf...")
        progressDialog.show()

        val timestamp = System.currentTimeMillis()
        val filePathAndName = "Books/$timestamp"

        val storageReference = FirebaseStorage.getInstance().getReference(filePathAndName)
        storageReference.putFile(pdfUri!!)
            .addOnSuccessListener {taskSnapshot ->
                Log.d(TAG,"uploadPdfToStorage: obteniendo url")

                val uriTask: Task<Uri> = taskSnapshot.storage.downloadUrl
                while (!uriTask.isSuccessful);
                val uploadedPdfUrl = "${uriTask.result}"

                uploadPdfInfoToDb(uploadedPdfUrl, timestamp)
            }
            .addOnFailureListener{e->
                Log.d(TAG,"uploadPdfToStorage: Fallo al subir: ${e.message}")
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al subir: ${e.message}", Toast.LENGTH_SHORT).show()
            }
    }

    /**
     * Subir datos del libro a firebase
     */
    private fun uploadPdfInfoToDb(uploadedPdfUrl: String, timestamp: Long) {
        Log.d(TAG,"uploadPdfInfoToDb")
        progressDialog.setMessage("Guardando la información del pdf...")
        progressDialog.show()

        //user registered id
        val uid = firebaseAuth.uid
        //Montamos los datos del libro
        val hashMap: HashMap<String, Any> = HashMap()
        hashMap["uid"] = "$uid"
        hashMap["id"] = "$timestamp"
        hashMap["title"] = "$title"
        hashMap["description"] = "$description"
        hashMap["categoryId"] = "$selectedCategoryId"
        hashMap["pagecount"] = pagecount
        hashMap["url"] = "$uploadedPdfUrl"
        hashMap["timestamp"] = timestamp
        hashMap["viewsCount"] = 0

        val ref = FirebaseDatabase.getInstance().getReference("PendingBooks")
        ref.child("$timestamp")
            .setValue(hashMap)
            .addOnSuccessListener {
                Log.d(TAG,"uploadPdfInfoToDb: subir a db")
                progressDialog.dismiss()
                Toast.makeText(this, "Información subido con exito, queda pendiente de la aprobación por un administrador", Toast.LENGTH_SHORT)
                    .show()
                pdfUri = null
                onBackPressed()
            }
            .addOnFailureListener{ e ->
                Log.d(TAG,"uploadPdfInfoToDb: Fallo al subir la información: ${e.message}")
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al subir la información: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }
    }
}
package com.example.appproyectofinal_sandra.activities

import android.app.AlertDialog
import android.app.ProgressDialog
import android.os.Bundle
import android.util.Log
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.appproyectofinal_sandra.databinding.ActivityPdfEditBinding
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener

class PdfEditActivity : AppCompatActivity() {

    private lateinit var binding: ActivityPdfEditBinding
    private lateinit var progressDialog: ProgressDialog
    private companion object{
        private  const val TAG = "PDF_EDIT_TAG"
    }

    //Listtado de categorias
    private lateinit var categoryTitleArrayList: ArrayList<String>
    private lateinit var categoryIdArrayList: ArrayList<String>

    //Identificador del libro
    private var bookId = ""
    private var type = ""

    //Datos a modificar
    private var selectedCategoryId = ""
    private var selectedCategoryTitle = ""
    private var title = ""
    private var description = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityPdfEditBinding.inflate(layoutInflater)
        setContentView(binding.root)

        bookId = intent.getStringExtra("bookId")!!
        type = intent.getStringExtra("type")!!

        //init progress dialog
        progressDialog = ProgressDialog(this)
        progressDialog.setTitle("Por favor, espere")
        progressDialog.setCanceledOnTouchOutside(false)

        loadCategories()
        loadBookInfo()

        //Salir
        binding.backBtn.setOnClickListener {
            onBackPressed()
        }

        //Cambiar categoría
        binding.categoryTv.setOnClickListener {
            categoryDialog()
        }

        //Guardar los cambios
        binding.submitBtn.setOnClickListener {
            validateData()
        }
    }

    /**
     * Carga las categorias disponibles
     */
    private fun loadCategories() {
        Log.d(TAG, "loadCategories")

        categoryTitleArrayList = ArrayList()
        categoryIdArrayList = ArrayList()

        val ref = FirebaseDatabase.getInstance().getReference("Categories")
        ref.addListenerForSingleValueEvent(object: ValueEventListener{
            override fun onDataChange(snapshot: DataSnapshot) {
                categoryTitleArrayList.clear()
                categoryIdArrayList.clear()

                for (ds in snapshot.children){

                    val id = "${ds.child("id").value}"
                    val category = "${ds.child("category").value}"

                    categoryTitleArrayList.add(category)
                    categoryIdArrayList.add(id)

                    Log.d(TAG, "onDataChange: Category Id: $id")
                    Log.d(TAG, "onDataChange: Category: $category")
                }
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Carga la información del libro
     */
    private fun loadBookInfo() {
        Log.d(TAG, "loadBookInfo")

        val ref = FirebaseDatabase.getInstance().getReference(type)
        ref.child(bookId)
            .addListenerForSingleValueEvent(object: ValueEventListener{
                override fun onDataChange(snapshot: DataSnapshot) {
                    selectedCategoryId = snapshot.child("categoryId").value.toString()
                    val description = snapshot.child("description").value.toString()
                    val title = snapshot.child("title").value.toString()

                    binding.titleEt.setText(title)
                    binding.descriptionEt.setText(description)

                    Log.d(TAG, "onDataChange: cargando info del libro")
                    val refBookCategory = FirebaseDatabase.getInstance().getReference("Categories")
                    refBookCategory.child(selectedCategoryId)
                        .addListenerForSingleValueEvent(object: ValueEventListener{
                            override fun onDataChange(snapshot: DataSnapshot) {
                                val category = snapshot.child("category").value

                                binding.categoryTv.text = category.toString()
                            }
                            override fun onCancelled(error: DatabaseError) {
                            }
                        })
                }
                override fun onCancelled(error: DatabaseError) {
                }
            })
    }

    /**
     * Dialogo selector de categorias
     */
    private fun categoryDialog() {
        val categoriesArray = arrayOfNulls<String>(categoryTitleArrayList.size)

        for (i in categoryTitleArrayList.indices){
            categoriesArray[i] = categoryTitleArrayList[i]
        }

        val builder = AlertDialog.Builder(this)
        builder.setTitle("Elige categoría")
            .setItems(categoriesArray){dialog, position ->
                selectedCategoryId = categoryIdArrayList[position]
                selectedCategoryTitle = categoryTitleArrayList[position]

                binding.categoryTv.text = selectedCategoryTitle
            }
            .show()
    }

    /**
     * Validar el formato de los datos editados
     */
    private fun validateData() {
        title = binding.titleEt.text.toString().trim()
        description = binding.descriptionEt.text.toString().trim()

        if (title.isEmpty()) {
            Toast.makeText(this, "Introduce el título", Toast.LENGTH_SHORT).show()
        } else if (description.isEmpty()) {
            Toast.makeText(this, "Introduce la descripción", Toast.LENGTH_SHORT).show()
        } else if (selectedCategoryId.isEmpty()) {
            Toast.makeText(this, "Selecciona una categoría", Toast.LENGTH_SHORT).show()
        } else {
            updatePdf()
        }
    }

    /**
     * Actualizar los datos del libro
     */
    private fun updatePdf() {
        Log.d(TAG,"updatePdf")
        progressDialog.setMessage("Actualizando la información del libro...")
        progressDialog.show()

        //Montamos los datos del libro
        val hashMap = HashMap<String, Any>()
        hashMap["title"] = "$title"
        hashMap["description"] = "$description"
        hashMap["categoryId"] = "$selectedCategoryId"

        val ref = FirebaseDatabase.getInstance().getReference(type)
        ref.child(bookId)
            .updateChildren(hashMap)
            .addOnSuccessListener {
                Log.d(TAG,"updatePdf: success")
                progressDialog.dismiss()
                Toast.makeText(this, "Información actualizada con exito", Toast.LENGTH_SHORT)
                    .show()
            }
            .addOnFailureListener{ e ->
                Log.d(TAG,"updatePdf: Fallo al actualizar la información: ${e.message}")
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al actualizar la información: ${e.message}", Toast.LENGTH_LONG).show()
            }
    }
}
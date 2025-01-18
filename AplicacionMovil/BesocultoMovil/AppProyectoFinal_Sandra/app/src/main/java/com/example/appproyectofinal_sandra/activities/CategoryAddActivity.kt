package com.example.appproyectofinal_sandra.activities

import android.app.ProgressDialog
import android.os.Bundle
import android.view.View
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.appproyectofinal_sandra.adapters.AdapterCategory
import com.example.appproyectofinal_sandra.databinding.ActivityCategoryAddBinding
import com.example.appproyectofinal_sandra.models.ModelCategory
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener
import java.text.Normalizer

class CategoryAddActivity : AppCompatActivity() {

    private lateinit var binding: ActivityCategoryAddBinding
    private lateinit var firebaseAuth: FirebaseAuth
    private lateinit var progressDialog: ProgressDialog

    // Lista de categorías existentes
    private lateinit var categoryList: ArrayList<String>

    //Nombre de la categoría a añadir
    private var category = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityCategoryAddBinding.inflate(layoutInflater)
        setContentView(binding.root)

        //init firebase auth
        firebaseAuth = FirebaseAuth.getInstance()

        //init progress
        progressDialog = ProgressDialog(this)
        progressDialog.setTitle("Por favor, espere")
        progressDialog.setCanceledOnTouchOutside(false)

        //Cargar categorías existentes
        loadCategories()

        //Salir
        binding.backBtn.setOnClickListener {
            onBackPressed()
        }

        //Guardar nueva categoría
        binding.submitBtn.setOnClickListener {
            validateData()
        }
    }

    /**
     * Cargar categorías existentes
     */
    private fun loadCategories() {

        categoryList = ArrayList()

        val ref = FirebaseDatabase.getInstance().getReference("Categories")
        ref.addValueEventListener(object : ValueEventListener {
            override fun onDataChange(snapshot: DataSnapshot) {
                categoryList.clear()

                for (ds in snapshot.children) {
                    val category = ds.child("category").getValue(String::class.java)

                    category?.let {
                        categoryList.add(normalizeString(it))
                    }
                }
            }
            override fun onCancelled(error: DatabaseError) {
            }
        })
    }

    /**
     * Validar datos de la nueva categoría
     */
    private fun validateData() {
        category = binding.categoryEt.text.toString().trim()

        if (category.isEmpty()) {
            Toast.makeText(this, "Introduce la categoría", Toast.LENGTH_SHORT).show()
        } else {
            val normalizedCategory = normalizeString(category)

            val resultado = categoryList.any{it.equals(normalizedCategory)}

            if (!resultado) {
                addCategoryFirebase()
            } else {
                Toast.makeText(this, "La categoría ya existe", Toast.LENGTH_SHORT).show()
            }
        }
    }

    /**
     * Subir categoría a firebase
     */
    private fun addCategoryFirebase() {
        progressDialog.show()

        val timestamp = System.currentTimeMillis()
        //Montamos los datos de la categoria
        val hashMap = HashMap<String, Any>()
        hashMap["id"] = "$timestamp"
        hashMap["category"] = category
        hashMap["timestamp"] = timestamp
        hashMap["uid"] = "${firebaseAuth.uid}"

        val ref = FirebaseDatabase.getInstance().getReference("Categories")
        ref.child("$timestamp")
            .setValue(hashMap)
            .addOnSuccessListener {
                progressDialog.dismiss()
                Toast.makeText(this, "Categoria agregada con exito", Toast.LENGTH_SHORT)
                    .show()
            }
            .addOnFailureListener{ e ->
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al añadir la categoria: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }
    }

    /**
     * Normalizar una cadena: convierte a minúsculas, elimina acentos y caracteres no alfabéticos.
     */
    private fun normalizeString(input: String): String {
        // Convertir a minúsculas
        var result = input.lowercase()

        // Eliminar acentos utilizando Normalizer
        result = Normalizer.normalize(result, Normalizer.Form.NFD)
        result = result.replace(Regex("\\p{InCombiningDiacriticalMarks}+"), "")

        // Eliminar cualquier carácter que no sea una letra (a-z)
        result = result.replace(Regex("[^a-z]"), "")

        return result
    }
}
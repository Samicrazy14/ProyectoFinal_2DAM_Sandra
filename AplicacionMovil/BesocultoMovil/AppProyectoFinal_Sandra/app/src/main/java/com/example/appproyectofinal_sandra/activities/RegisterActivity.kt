package com.example.appproyectofinal_sandra.activities

import android.app.ProgressDialog
import android.content.Intent
import android.os.Bundle
import android.util.Patterns
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.appproyectofinal_sandra.databinding.ActivityRegisterBinding
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.FirebaseDatabase

class RegisterActivity : AppCompatActivity() {

    private lateinit var binding: ActivityRegisterBinding
    private lateinit var firebaseAuth: FirebaseAuth
    private lateinit var progressDialog: ProgressDialog

    //Datos de Registro
    private var name = ""
    private var email = ""
    private var password = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityRegisterBinding.inflate(layoutInflater)
        setContentView(binding.root)

        //init firebase auth
        firebaseAuth = FirebaseAuth.getInstance()

        //init progress create new user
        progressDialog = ProgressDialog(this)
        progressDialog.setTitle("Por favor, espere")
        progressDialog.setCanceledOnTouchOutside(false)

        //Clickar en volver
        binding.backBtn.setOnClickListener {
            onBackPressed()
        }

        //Clickar en registrar
        binding.registerBtn.setOnClickListener {
            validateData()
        }
    }

    //Validar datos del formulario de login
    private fun validateData() {
        name = binding.nameEt.text.toString().trim()
        email = binding.emailEt.text.toString().trim()
        password = binding.passwordEt.text.toString().trim()
        val cPassword = binding.cPpasswordEt.text.toString().trim()

        if (name.isEmpty()) {
            Toast.makeText(this, "Introduce tu nombre", Toast.LENGTH_SHORT).show()
        } else if (!Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
            Toast.makeText(this, "Introduce un email valido", Toast.LENGTH_SHORT).show()
        } else if (password.isEmpty()) {
            Toast.makeText(this, "Introduce tu contaseña (6 caracteres minimo)", Toast.LENGTH_SHORT).show()
        } else if (cPassword.isEmpty()) {
            Toast.makeText(this, "Confirma tu contraseña", Toast.LENGTH_SHORT).show()
        } else if (password != cPassword) {
            Toast.makeText(this, "Las contraseñas no coinciden", Toast.LENGTH_SHORT).show()
        } else {
            createUserAccount()
        }
    }

    //Crear nueva cuenta de usuario
    private fun createUserAccount() {
        progressDialog.setMessage("Creando cuenta...")
        progressDialog.show()

        firebaseAuth.createUserWithEmailAndPassword(email, password)
            .addOnSuccessListener {
                updateUserInfo()
            }
            .addOnFailureListener { e ->
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al crear la cuenta: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }
    }

    //Subir datos de usuario y redirigir al inicio
    private fun updateUserInfo() {
        progressDialog.setMessage("Guardando la información del usuario...")
        progressDialog.show()

        val timestamp = System.currentTimeMillis()
        val uid = firebaseAuth.uid //user id

        //Montamos los datos del usuario
        val hashMap: HashMap<String, Any?> = HashMap()
        hashMap["uid"] = uid
        hashMap["email"] = email
        hashMap["name"] = name
        hashMap["profileImage"] = "" // default empty, change in profile edit
        hashMap["userType"] = "user" //user or admin
        hashMap["timestamp"] = timestamp

        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.child(uid!!)
            .setValue(hashMap)
            .addOnSuccessListener {
                progressDialog.dismiss()
                Toast.makeText(this, "Usuario creado con exito", Toast.LENGTH_SHORT)
                    .show()
                startActivity(Intent(this@RegisterActivity, DashBoardUserActivity::class.java))
                finish()
            }
            .addOnFailureListener{ e ->
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al guardar al usuario: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }

    }
}
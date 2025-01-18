package com.example.appproyectofinal_sandra.activities

import android.app.ProgressDialog
import android.content.Intent
import android.os.Bundle
import android.util.Log
import android.util.Patterns
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.appproyectofinal_sandra.databinding.ActivityLoginBinding
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener

class LoginActivity : AppCompatActivity() {

    private lateinit var binding: ActivityLoginBinding
    private lateinit var firebaseAuth: FirebaseAuth
    private lateinit var progressDialog: ProgressDialog

    //Datos de Login
    private var email = ""
    private var password = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityLoginBinding.inflate(layoutInflater)
        setContentView(binding.root)

        //init firebase auth
        firebaseAuth = FirebaseAuth.getInstance()

        //init progress login  user
        progressDialog = ProgressDialog(this)
        progressDialog.setTitle("Por favor, espere")
        progressDialog.setCanceledOnTouchOutside(false)

        //Clickar en no tengo cuenta registar
        binding.noAccountTv.setOnClickListener {
            startActivity(Intent(this, RegisterActivity::class.java))
        }

        //Clickar en hacer login
        binding.loginBtn.setOnClickListener {
            validateData()
        }

        //Clickar en recuperar contraseña
        binding.forgotTv.setOnClickListener {
            startActivity(Intent(this, ForgotPasswordActivity::class.java))
        }
    }

    //Validar datos del formulario de login
    private fun validateData() {
        email = binding.emailEt.text.toString().trim()
        password = binding.passwordEt.text.toString().trim()

        if (!Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
            Toast.makeText(this, "Introduce un email valido", Toast.LENGTH_SHORT).show()
        } else if (password.isEmpty()) {
            Toast.makeText(this, "Introduce tu contaseña", Toast.LENGTH_SHORT).show()
        } else {
            loginUser()
        }
    }

    //Login de usuario
    private fun loginUser() {
        progressDialog.setMessage("Logging in...")
        progressDialog.show()

        firebaseAuth.signInWithEmailAndPassword(email, password)
            .addOnSuccessListener {
                checkUser()
            }
            .addOnFailureListener { e ->
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al logearte: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }
    }

    //Comprobar el tipo de usuario y redirigir a la página correspondiente
    private fun checkUser() {
        progressDialog.setMessage("Comprobando usuario...")
        progressDialog.show()

        val firebaseUser = firebaseAuth.currentUser!!

        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.child(firebaseUser.uid)
            .addListenerForSingleValueEvent(object : ValueEventListener {
                override fun onDataChange(snapshot: DataSnapshot) {
                    progressDialog.dismiss()

                    val userType = snapshot.child("userType").value.toString()
                    Log.d("Login",userType)
                    if (userType != "admin"){
                        startActivity(Intent(this@LoginActivity, DashBoardUserActivity::class.java))
                        finish()
                    }else {
                        startActivity(Intent(this@LoginActivity, DashBoardAdminActivity::class.java))
                        finish()
                    }
                }
                override fun onCancelled(error: DatabaseError) {
                }
            })
    }
}
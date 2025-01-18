package com.example.appproyectofinal_sandra.activities

import android.app.ProgressDialog
import android.os.Bundle
import android.util.Patterns
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.appproyectofinal_sandra.databinding.ActivityForgotPasswordBinding
import com.google.firebase.auth.FirebaseAuth

class ForgotPasswordActivity : AppCompatActivity() {

    private lateinit var binding: ActivityForgotPasswordBinding
    private lateinit var firebaseAuth: FirebaseAuth
    private lateinit var progressDialog: ProgressDialog

    //Email de recuperación
    private var email = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityForgotPasswordBinding.inflate(layoutInflater)
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

        //Clickar en recuperar contraseña
        binding.submitBtn.setOnClickListener {
            validateData()
        }
    }

    //Validar correo de recuperación
    private fun validateData() {
        email = binding.emailEt.text.toString().trim()

        if (email.isEmpty()) {
            Toast.makeText(this, "Introduce tu nombre", Toast.LENGTH_SHORT).show()
        } else if (!Patterns.EMAIL_ADDRESS.matcher(email).matches()) {
            Toast.makeText(this, "Introduce un email valido", Toast.LENGTH_SHORT).show()
        } else {
            recoverPassword()
        }
    }

    //Recuperar contraseña
    private fun recoverPassword() {
        progressDialog.setMessage("Enviando correo a $email...")
        progressDialog.show()

        firebaseAuth.sendPasswordResetEmail(email)
            .addOnSuccessListener {
                progressDialog.dismiss()
                Toast.makeText(this, "Correo enviado a:\n$email", Toast.LENGTH_SHORT)
                    .show()
            }
            .addOnFailureListener { e ->
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al enviar el correo a $email: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }
    }
}
package com.example.appproyectofinal_sandra.activities

import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.util.Log
import androidx.appcompat.app.AppCompatActivity
import com.example.appproyectofinal_sandra.R
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener

class SplashActivity : AppCompatActivity() {

    private lateinit var firebaseAuth: FirebaseAuth

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_splash)

        //init firebase auth para los datos de usuario
        firebaseAuth = FirebaseAuth.getInstance()

        Handler().postDelayed(Runnable {
            checkUser()
        }, 1000)//1seg de animacion
    }

    private fun checkUser() {
        val firebaseUser = firebaseAuth.currentUser
        if(firebaseUser == null){//Si no hay usuario registrado al Main
            startActivity(Intent(this, MainActivity::class.java))
            finish()
        } else {//Si esta registrado ver el tipo de usuario
            val ref = FirebaseDatabase.getInstance().getReference("Users")
            ref.child(firebaseUser.uid)
                .addListenerForSingleValueEvent(object : ValueEventListener {
                    override fun onDataChange(snapshot: DataSnapshot) {
                        val userType = snapshot.child("userType").value.toString()
                        Log.d("Splashscreen", userType)
                        if (userType != "admin"){
                            startActivity(Intent(this@SplashActivity, DashBoardUserActivity::class.java))
                            finish()
                        }else {
                            startActivity(Intent(this@SplashActivity, DashBoardAdminActivity::class.java))
                            finish()
                        }
                    }
                    override fun onCancelled(error: DatabaseError) {
                    }
                })
        }
    }
}
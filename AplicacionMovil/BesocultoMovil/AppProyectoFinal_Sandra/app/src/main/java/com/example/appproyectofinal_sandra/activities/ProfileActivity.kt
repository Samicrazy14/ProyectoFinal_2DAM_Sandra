package com.example.appproyectofinal_sandra.activities

import android.app.AlertDialog
import android.app.ProgressDialog
import android.content.Intent
import android.os.Bundle
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.bumptech.glide.Glide
import com.example.appproyectofinal_sandra.MyApplication
import com.example.appproyectofinal_sandra.R
import com.example.appproyectofinal_sandra.adapters.AdapterPdfFavorite
import com.example.appproyectofinal_sandra.databinding.ActivityProfileBinding
import com.example.appproyectofinal_sandra.models.ModelPdf
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.auth.FirebaseUser
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener

class ProfileActivity : AppCompatActivity() {

    private lateinit var binding: ActivityProfileBinding
    private lateinit var progressDialog: ProgressDialog
    private lateinit var firebaseAuth: FirebaseAuth
    private lateinit var firebaseUser: FirebaseUser

    //ArrayList de libros favoritos y su adaptador
    private lateinit var booksArrayList: ArrayList<ModelPdf>
    private lateinit var adapterPdfFavorite: AdapterPdfFavorite

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityProfileBinding.inflate(layoutInflater)
        setContentView(binding.root)

        binding.accountTypeTv.text = "N/A"
        binding.memberDateTv.text = "N/A"
        binding.favoriteBookCountTv.text = "N/A"
        binding.accountStatusTv.text = "N/A"

        //init progress
        progressDialog = ProgressDialog(this)
        progressDialog.setTitle("Por favor, espere")
        progressDialog.setCanceledOnTouchOutside(false)

        //init firebase auth
        firebaseAuth = FirebaseAuth.getInstance()
        firebaseUser = firebaseAuth.currentUser!!

        loadUserInfo()
        loadFavoriteBooks()

        //Salir del Perfil
        binding.backBtn.setOnClickListener {
            onBackPressed()
        }

        //Editar Perfil
        binding.profileEditBtn.setOnClickListener {
            startActivity(Intent(this, ProfileEditActivity::class.java))
        }

        //Verificar Perfil
        binding.accountStatus.setOnClickListener {
            if (binding.accountStatusTv.text == "Verificado"){
                Toast.makeText(this, "Ya estas verificado", Toast.LENGTH_SHORT).show()
            }else if(binding.accountStatusTv.text == "Verificación pendiente"){
                Toast.makeText(this, "Verificación pendiente de ser aprobada", Toast.LENGTH_SHORT).show()
            } else {
                emailVerificationDialog()
            }
        }
    }

    /**
     * Cargar datos de usuario en el perfil
     */
    private fun loadUserInfo() {
        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.child(firebaseAuth.uid!!)
            .addListenerForSingleValueEvent(object : ValueEventListener {
                override fun onDataChange(snapshot: DataSnapshot) {
                    val email = "${snapshot.child("email").value}"
                    val name = "${snapshot.child("name").value}"
                    val profileImage = "${snapshot.child("profileImage").value}"
                    val timestamp = "${snapshot.child("timestamp").value}"
                    val userType = "${snapshot.child("userType").value}"
                    val formattedDate = MyApplication.formatTimeStamp(timestamp.toLong())

                    if(userType == "author"){
                        binding.accountStatusTv.text = "Verificado"
                    } else if(userType == "pending user"){
                        binding.accountStatusTv.text = "Verificación pendiente"
                    } else {
                        binding.accountStatusTv.text = "No verificado"
                    }

                    binding.nameTv.text = name
                    binding.emailTv.text = email
                    binding.memberDateTv.text = formattedDate
                    binding.accountTypeTv.text = userType

                    try {
                        Glide.with(this@ProfileActivity)
                            .load(profileImage)
                            .placeholder(R.drawable.ic_person_gray)
                            .into(binding.profileIv)
                    } catch (e: Exception){

                    }
                }
                override fun onCancelled(error: DatabaseError) {
                }
            })
    }

    /**
     * Cargar libros favoritos del usuario en el perfil
     */
    private fun loadFavoriteBooks() {
        booksArrayList = ArrayList()

        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.child(firebaseAuth.uid!!).child("Favorites")
            .addValueEventListener(object: ValueEventListener{
                override fun onDataChange(snapshot: DataSnapshot) {
                    booksArrayList.clear()

                    for(ds in snapshot.children){
                        val bookId = "${ds.child("bookId").value}"
                        val type = "${ds.child("type").value}"

                        val modelPdf = ModelPdf()
                        modelPdf.id = bookId
                        modelPdf.uid = type

                        booksArrayList.add(modelPdf)
                    }

                    binding.favoriteBookCountTv.text = "${booksArrayList.size}"

                    adapterPdfFavorite = AdapterPdfFavorite(this@ProfileActivity, booksArrayList)
                    binding.favoriteRv.adapter = adapterPdfFavorite
                }
                override fun onCancelled(error: DatabaseError) {
                }
            })
    }

    /**
     * Dialogo de confirmación para enviar un correo para verificar la cuenta
     */
    private fun emailVerificationDialog() {
        val builder = AlertDialog.Builder(this)
        builder.setTitle("Verificar Email")
            .setMessage("¿Estas seguro de querer verificar tu usuario ${firebaseUser.email}? Se actualizará tu cuenta a una de escritor tras ser procesada tu solicitud.")
            .setPositiveButton("Enviar"){ _, _ ->
                sendEmailVerification()
            }
            .setNegativeButton("Cancelar"){ d, _ ->
                d.dismiss()
            }
            .show()
    }

    /**
     * Envia un correo para verificar la cuenta
     */
    private fun sendEmailVerification() {
        val hashMap: HashMap<String, Any> = HashMap()
        hashMap["userType"] = "pending user"

        val reference = FirebaseDatabase.getInstance().getReference("Users")
        reference.child(firebaseAuth.uid!!)
            .updateChildren(hashMap)
            .addOnSuccessListener {
                progressDialog.dismiss()
                Toast.makeText(this, "Solicitud creada con exito", Toast.LENGTH_SHORT)
                    .show()
            }
            .addOnFailureListener{ e ->
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al solicitar la petición: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }
    }
}
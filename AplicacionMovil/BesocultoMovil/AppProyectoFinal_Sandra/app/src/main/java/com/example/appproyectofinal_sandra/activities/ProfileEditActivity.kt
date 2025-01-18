package com.example.appproyectofinal_sandra.activities

import android.app.Activity
import android.app.ProgressDialog
import android.content.ContentValues
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import android.provider.MediaStore
import android.view.Menu
import android.widget.Toast
import androidx.activity.result.ActivityResult
import androidx.activity.result.ActivityResultCallback
import androidx.activity.result.contract.ActivityResultContracts
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.PopupMenu
import com.bumptech.glide.Glide
import com.example.appproyectofinal_sandra.R
import com.example.appproyectofinal_sandra.databinding.ActivityProfileEditBinding
import com.google.android.gms.tasks.Task
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener
import com.google.firebase.storage.FirebaseStorage

class ProfileEditActivity : AppCompatActivity() {

    private lateinit var binding: ActivityProfileEditBinding
    private lateinit var firebaseAuth: FirebaseAuth
    private lateinit var progressDialog: ProgressDialog

    //Datos editables del perfil
    private var imageUri: Uri?= null
    private var name = ""

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        binding = ActivityProfileEditBinding.inflate(layoutInflater)
        setContentView(binding.root)

        //init progress create new user
        progressDialog = ProgressDialog(this)
        progressDialog.setTitle("Por favor, espere")
        progressDialog.setCanceledOnTouchOutside(false)

        //init firebase auth
        firebaseAuth = FirebaseAuth.getInstance()
        loadUserInfo()

        //Salir
        binding.backBtn.setOnClickListener {
            onBackPressed()
        }

        //Cambiar Imagen
        binding.profileIv.setOnClickListener {
            showImageAttachMenu()
        }

        //Actualizar Perfil
        binding.updateBtn.setOnClickListener {
            validateData()
        }
    }

    /**
     * Cargar datos de usuario en la vista
     */
    private fun loadUserInfo() {
        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.child(firebaseAuth.uid!!)
            .addListenerForSingleValueEvent(object : ValueEventListener {
                override fun onDataChange(snapshot: DataSnapshot) {
                    val name = "${snapshot.child("name").value}"
                    val profileImage = "${snapshot.child("profileImage").value}"

                    binding.nameEt.setText(name)

                    try {
                        Glide.with(this@ProfileEditActivity)
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
     * Popupmenu para selecionar si cambiar la imagen desde cámara o galería
     */
    private fun showImageAttachMenu() {
        val popupMenu = PopupMenu(this, binding.profileIv)
        popupMenu.menu.add(Menu.NONE, 0, 0, "Cámara")
        popupMenu.menu.add(Menu.NONE, 1, 1, "Galería")
        popupMenu.show()

        popupMenu.setOnMenuItemClickListener {item->
            val id = item.itemId
            if(id == 0){
                pickImageCamera()
            } else if (id == 1){
                pickImageGallery()
            }
            true
        }
    }

    /**
     * Le mandamos a selecionar la imagen a galería
     */
    private fun pickImageGallery() {
        val intent = Intent(Intent.ACTION_PICK)
        intent.type = "image/*"
        galleryActivityResultLauncher.launch(intent)
    }

    /**
     * Gestión de la seleción de imagen en la galería
     */
    private val galleryActivityResultLauncher = registerForActivityResult(
        ActivityResultContracts.StartActivityForResult(),
        ActivityResultCallback<ActivityResult> { result ->
            if(result.resultCode == Activity.RESULT_OK){
                val data = result.data
                imageUri = data!!.data

                binding.profileIv.setImageURI(imageUri)
            }
            else {
                Toast.makeText(this, "Cancelado", Toast.LENGTH_SHORT).show()
            }
        }
    )

    /**
     * Le mandamos a la cámara
     */
    private fun pickImageCamera() {
        val values = ContentValues()
        values.put(MediaStore.Images.Media.TITLE, "Temp_Title")
        values.put(MediaStore.Images.Media.DESCRIPTION, "Temp_Description")

        imageUri = contentResolver.insert(MediaStore.Images.Media.EXTERNAL_CONTENT_URI, values)

        val intent = Intent(MediaStore.ACTION_IMAGE_CAPTURE)
        intent.putExtra(MediaStore.EXTRA_OUTPUT, imageUri)

        cameraActivityResultLauncher.launch(intent)
    }

    /**
     * Gestión de la toma de imagen en la cámara
     */
    private val cameraActivityResultLauncher = registerForActivityResult(
        ActivityResultContracts.StartActivityForResult(),
        ActivityResultCallback<ActivityResult> { result ->
            if(result.resultCode == Activity.RESULT_OK){
                binding.profileIv.setImageURI(imageUri)
            }
            else {
                Toast.makeText(this, "Cancelado", Toast.LENGTH_SHORT).show()
            }
        }
    )

    /**
     * Validar nuevos campos
     */
    private fun validateData() {
        name = binding.nameEt.text.toString().trim()

        if (name.isEmpty()) {
            Toast.makeText(this, "Introduce tu nombre", Toast.LENGTH_SHORT).show()
        } else {
            if(imageUri == null){
                updateProfile("")
            }
            else {
                uploadImage()
            }
        }
    }

    /**
     * Actualizar perfil sin cambio de imagen
     */
    private fun updateProfile(uploadedImageUrl: String) {
        progressDialog.setMessage("Actualizando perfil...")
        progressDialog.show()

        val hashMap: HashMap<String, Any> = HashMap()
        hashMap["name"] = name
        if(imageUri != null){
            hashMap["profileImage"] = uploadedImageUrl

        }

        val reference = FirebaseDatabase.getInstance().getReference("Users")
        reference.child(firebaseAuth.uid!!)
            .updateChildren(hashMap)
            .addOnSuccessListener {
                progressDialog.dismiss()
                Toast.makeText(this, "Perfil actualizado con exito", Toast.LENGTH_SHORT)
                    .show()
            }
            .addOnFailureListener{ e ->
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al actualizar el perfil: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }
    }

    /**
     * Actualizar perfil e imagen (storage)
     */
    private fun uploadImage() {
        progressDialog.setMessage("Subiendo imagen...")
        progressDialog.show()

        val filePathAndName = "Profile/"+firebaseAuth.uid

        val reference = FirebaseStorage.getInstance().getReference(filePathAndName)
        reference.putFile(imageUri!!)
            .addOnSuccessListener { taskSnapshot ->
                val uriTask: Task<Uri> = taskSnapshot.storage.downloadUrl
                while(!uriTask.isSuccessful);

                val uploadedImageUrl = "${uriTask.result}"
                updateProfile(uploadedImageUrl)
            }
            .addOnFailureListener{ e ->
                progressDialog.dismiss()
                Toast.makeText(this, "Fallo al actualizar la imagen: ${e.message}", Toast.LENGTH_LONG).show()
            }
    }
}
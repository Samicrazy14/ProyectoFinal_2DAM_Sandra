package com.example.appproyectofinal_sandra.adapters

import android.app.AlertDialog
import android.content.Context
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.appproyectofinal_sandra.MyApplication
import com.example.appproyectofinal_sandra.R
import com.example.appproyectofinal_sandra.databinding.RowCommentBinding
import com.example.appproyectofinal_sandra.models.ModelComment
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener

class AdapterComment :RecyclerView.Adapter<AdapterComment.HolderComment>{

    private var firebaseAuth: FirebaseAuth
    val context: Context
    private lateinit var binding: RowCommentBinding

    //ArrayList de comentarios
    private val commentArrayList: ArrayList<ModelComment>

    //Tipo de libro
    private val type: String

    //Constructor
    constructor(
        context: Context,
        commentArrayList: ArrayList<ModelComment>,
        type: String
    ) : super() {
        this.context = context
        this.commentArrayList = commentArrayList
        this.type = type
        firebaseAuth = FirebaseAuth.getInstance()
    }

    /**
     * Asigna valores y funcionalidades a la vista de los items de la lista de comentarios
     */
    override fun onBindViewHolder(holder: HolderComment, position: Int) {
        val model = commentArrayList[position]
        val comment = model.comment
        val timestamp = model.timestamp
        val date = MyApplication.formatTimeStamp(timestamp.toLong())
        
        holder.dateTv.text = date
        holder.commentTv.text = comment

        //Cargar info de los usuarios de los comentarios
        loadUserDetails(model, holder)

        //Eliminar comentario
        holder.itemView.setOnClickListener{
            getUserdata(model)
        }

    }

    /**
     * Solicita los datos del usuario para ver si puede eliminar el comentario
     */
    private fun getUserdata(model: ModelComment) {
        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.child(firebaseAuth.uid!!)
            .addListenerForSingleValueEvent(object : ValueEventListener {
                override fun onDataChange(snapshot: DataSnapshot) {
                    val userType = "${snapshot.child("userType").value}"
                    if(firebaseAuth.currentUser != null && (firebaseAuth.uid == model.uid || userType == "admin")){
                        deleteCommentDialog(model)
                    }
                }
                override fun onCancelled(error: DatabaseError) {
                }
            })
    }

    /**
     * Cargar imagen y nombre de los usuarios de los comentarios
     */
    private fun loadUserDetails(model: ModelComment, holder: AdapterComment.HolderComment) {
        val uid = model.uid

        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.child(uid)
            .addListenerForSingleValueEvent(object : ValueEventListener {
                override fun onDataChange(snapshot: DataSnapshot) {
                    val name = "${snapshot.child("name").value}"
                    val profileImage = "${snapshot.child("profileImage").value}"

                    holder.nameTv.text = name
                    try {
                        Glide.with(context)
                            .load(profileImage)
                            .placeholder(R.drawable.ic_person_gray)
                            .into(holder.profileIv)
                    } catch (e: Exception){
                        holder.profileIv.setImageResource(R.drawable.ic_person_gray)
                    }
                }
                override fun onCancelled(error: DatabaseError) {
                }
            })
    }

    /**
     * Eliminar tu comentario
     */
    private fun deleteCommentDialog(model: ModelComment) {
        val builder = AlertDialog.Builder(context)
        builder.setTitle("Eliminar comentario")
            .setMessage("¿Estas seguro de que desea eliminar este mensaje?")
            .setPositiveButton("Eliminar"){ _, _ ->
                val  bookId = model.bookId
                val commentId = model.id

                val ref = FirebaseDatabase.getInstance().getReference(type)
                ref.child(bookId).child("Comments").child(commentId)
                    .removeValue()
                    .addOnSuccessListener {
                        Toast.makeText(context, "Comentario eliminado", Toast.LENGTH_SHORT)
                            .show()
                    }
                    .addOnFailureListener{ e ->
                        Toast.makeText(context, "Fallo al eliminar el comentario: ${e.message}", Toast.LENGTH_SHORT)
                            .show()
                    }
            }
            .setNegativeButton("Cancelar"){ a, _ ->
                a.dismiss()
            }
            .show()
    }

    /**
     * Asigna la vista a los items de la lista de comentarios
     */
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): HolderComment {
        binding = RowCommentBinding.inflate(LayoutInflater.from(context), parent, false)
        return HolderComment(binding.root)
    }

    /**
     * Tamaño de la lista de comentarios
     */
    override fun getItemCount(): Int {
        return commentArrayList.size
    }

    inner class HolderComment(itemView: View) : RecyclerView.ViewHolder(itemView){
        val profileIv = binding.profileIv
        val nameTv = binding.nameTv
        val commentTv = binding.commentTv
        val dateTv = binding.dateTv
    }
}
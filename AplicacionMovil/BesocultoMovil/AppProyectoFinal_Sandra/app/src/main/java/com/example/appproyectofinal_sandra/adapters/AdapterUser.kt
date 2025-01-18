package com.example.appproyectofinal_sandra.adapters

import android.app.AlertDialog
import android.content.Context
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Filter
import android.widget.Filterable
import android.widget.Toast
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.appproyectofinal_sandra.MyApplication
import com.example.appproyectofinal_sandra.R
import com.example.appproyectofinal_sandra.databinding.RowUserBinding
import com.example.appproyectofinal_sandra.filters.FilterUser
import com.example.appproyectofinal_sandra.models.ModelUser
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.GenericTypeIndicator
import com.google.firebase.database.MutableData
import com.google.firebase.database.Transaction
import com.google.firebase.database.ValueEventListener


class AdapterUser :RecyclerView.Adapter<AdapterUser.HolderUser>, Filterable {

    private var firebaseAuth: FirebaseAuth
    val context: Context
    private lateinit var binding: RowUserBinding

    //ArrayList de USUARIOS
    var userArrayList: ArrayList<ModelUser>

    //Lista filtrada
    private var filterList: ArrayList<ModelUser>
    private var filter: FilterUser? = null


    //Constructor
    constructor(
        context: Context,
        userArrayList: ArrayList<ModelUser>
    ) : super() {
        this.context = context
        this.userArrayList = userArrayList
        this.filterList = userArrayList
        firebaseAuth = FirebaseAuth.getInstance()
    }

    /**
     * Asigna valores y funcionalidades a la vista de los items de la lista de usuarios
     */
    override fun onBindViewHolder(holder: HolderUser, position: Int) {
        val model = userArrayList[position]

        //Cargar info de los usuarios de los comentarios
        loadUserDetails(model, holder)

        //Aceptar o denegar cambio de rol de usuario
        holder.moreBtn.setOnClickListener {
            moreOptionsDialog(model)
        }

        //Borrar usuario
        holder.deleteBtn.setOnClickListener{
            val builder = AlertDialog.Builder(context)
            builder.setTitle("Borrar")
                .setMessage("¿Estas seguro de que desea eliminar al usuario: ${model.email}?")
                .setPositiveButton("Confirmar"){ _, _ ->
                    Toast.makeText(context,"Borrando...", Toast.LENGTH_SHORT).show()
                    MyApplication.deleteUser(context, model.uid, model.email, model.profileImage)
                }
                .setNegativeButton("Cancelar"){ a, _ ->
                    a.dismiss()
                }
                .show()
        }
    }

    /**
     * Cargar imagen y datos de los usuarios
     */
    private fun loadUserDetails(model: ModelUser, holder: AdapterUser.HolderUser) {
        val uid = model.uid

        val ref = FirebaseDatabase.getInstance().getReference("Users")
        ref.child(uid)
            .addListenerForSingleValueEvent(object : ValueEventListener {
                override fun onDataChange(snapshot: DataSnapshot) {
                    val name = "${snapshot.child("name").value}"
                    val email = "${snapshot.child("email").value}"
                    val userType = "${snapshot.child("userType").value}"
                    val timestamp = "${snapshot.child("timestamp").value}"
                    val date = MyApplication.formatTimeStamp(timestamp.toLong())
                    val profileImage = "${snapshot.child("profileImage").value}"

                    if(userType != "pending user"){
                        holder.moreBtn.visibility = View.GONE
                    }

                    holder.nameTv.text = name
                    holder.dateTv.text = date
                    holder.emailTv.text = email
                    holder.rolTv.text = userType
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
     * Opciones de modificar el usuario, editar o borrar
     */
    private fun moreOptionsDialog(model: ModelUser) {
        val builder = AlertDialog.Builder(context)
        builder.setTitle("Cambio de Rol")
            .setMessage("¿Quiere dar el rango de autor al usuario: ${model.email}?")
            .setPositiveButton("Aceptar"){ _, _ ->
                    changeUserType("author", model.uid)
            }
            .setNegativeButton("Denegar"){ a, _ ->
                    changeUserType("user", model.uid)
            }
            .show()
    }

    /**
     * Actualiza la petición de cambio de rol
     */
    private fun changeUserType(rol: String, uid: String) {
        val ref = FirebaseDatabase.getInstance().getReference("Users").child(uid)
        ref.runTransaction(object : Transaction.Handler {
            override fun doTransaction(mutableData: MutableData): Transaction.Result {
                val currentUser = mutableData.getValue(object : GenericTypeIndicator<HashMap<String, Any>>() {})
                    ?: return Transaction.abort()

                // Actualizamos el valor en el mapa
                currentUser["userType"] = rol

                // Establecemos el valor actualizado
                mutableData.value = currentUser

                return Transaction.success(mutableData)
            }

            override fun onComplete(
                error: DatabaseError?,
                committed: Boolean,
                currentData: DataSnapshot?
            ) {
                if (error != null) {
                    Log.e("Firebase", "Error al actualizar el rol", error.toException())
                } else {
                    Log.d("Firebase", "Rol actualizado exitosamente")
                }
            }
        })
    }

    /**
     * Asigna la vista a los items de la lista de usuarios
     */
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): HolderUser {
        binding = RowUserBinding.inflate(LayoutInflater.from(context), parent, false)
        return HolderUser(binding.root)
    }

    /**
     * Tamaño de la lista de usuarios
     */
    override fun getItemCount(): Int {
        return userArrayList.size
    }

    inner class HolderUser(itemView: View) : RecyclerView.ViewHolder(itemView){
        val profileIv = binding.profileIv
        val nameTv = binding.nameTv
        val emailTv = binding.emailTv
        val rolTv = binding.rolTv
        val dateTv = binding.dateTv
        val moreBtn = binding.moreBtn
        val deleteBtn = binding.deleteBtn
    }

    override fun getFilter(): Filter {
        println(filter)
        if (filter == null){
            filter = FilterUser(userArrayList, this)
        }
        return filter as FilterUser
    }
}
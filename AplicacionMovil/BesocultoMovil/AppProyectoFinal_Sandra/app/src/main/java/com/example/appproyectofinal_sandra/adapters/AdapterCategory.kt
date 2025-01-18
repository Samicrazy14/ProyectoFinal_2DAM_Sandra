package com.example.appproyectofinal_sandra.adapters

import android.app.AlertDialog
import android.content.Context
import android.content.Intent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Filter
import android.widget.Filterable
import android.widget.ImageButton
import android.widget.TextView
import android.widget.Toast
import androidx.recyclerview.widget.RecyclerView
import com.example.appproyectofinal_sandra.filters.FilterCategory
import com.example.appproyectofinal_sandra.models.ModelCategory
import com.example.appproyectofinal_sandra.activities.PdfListAdminActivity
import com.example.appproyectofinal_sandra.databinding.RowCategoryBinding
import com.google.firebase.database.FirebaseDatabase

class AdapterCategory :RecyclerView.Adapter<AdapterCategory.HolderCategory>, Filterable {

    private lateinit var binding: RowCategoryBinding
    private val context:Context

    //ArrayList de categorias
    var categoryArrayList: ArrayList<ModelCategory>

    //Lista filtrada
    private var filterList: ArrayList<ModelCategory>
    private var filter: FilterCategory? = null

    //Constructor
    constructor(context:Context, categoryArrayList: ArrayList<ModelCategory>){
        this.context = context
        this.categoryArrayList = categoryArrayList
        this.filterList = categoryArrayList
    }

    /**
     * Asigna valores y funcionalidades a la vista de los items de la lista de categorias
     */
    override fun onBindViewHolder(holder: HolderCategory, position: Int) {
        val model = categoryArrayList[position]
        val id = model.id
        val category = model.category

        holder.categoryTv.text = category

        //Borrar Categoría
        holder.deleteBtn.setOnClickListener{
            val builder = AlertDialog.Builder(context)
            builder.setTitle("Borrar")
                .setMessage("¿Estas seguro de que desea eliminar esta categoría?")
                .setPositiveButton("Confirmar"){ _, _ ->
                    Toast.makeText(context,"Borrando...", Toast.LENGTH_SHORT).show()
                    deleteCategory(model)
                }
                .setNegativeButton("Cancelar"){ a, _ ->
                    a.dismiss()
                }
                .show()
        }

        //Ir a ver libro
        holder.itemView.setOnClickListener{
            val intent = Intent(context, PdfListAdminActivity::class.java)
            intent.putExtra("categoryId", id)
            intent.putExtra("category", category)
            context.startActivity(intent)
        }

    }

    /**
     * Borrar categoría
     */
    private fun deleteCategory(model: ModelCategory) {
        val id = model.id

        val ref = FirebaseDatabase.getInstance().getReference("Categories")
        ref.child(id)
            .removeValue()
            .addOnSuccessListener {
                Toast.makeText(context, "Categoria eliminada", Toast.LENGTH_SHORT)
                    .show()
            }
            .addOnFailureListener{ e ->
                Toast.makeText(context, "Fallo al eliminar la categoria: ${e.message}", Toast.LENGTH_LONG)
                    .show()
            }
    }

    /**
     * Asigna la vista a los items de la lista de categorias
     */
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): HolderCategory {
        binding = RowCategoryBinding.inflate(LayoutInflater.from(context), parent, false)
        return HolderCategory(binding.root)
    }

    /**
     * Tamaño de la lista de categorias
     */
    override fun getItemCount(): Int {
        return categoryArrayList.size
    }

    inner class HolderCategory(itemView: View):RecyclerView.ViewHolder(itemView){
        var categoryTv:TextView = binding.categoryTv
        var deleteBtn:ImageButton = binding.deleteBtn
    }

    override fun getFilter(): Filter {
        if(filter == null){
            filter = FilterCategory(filterList, this)
        }
        return filter as FilterCategory
    }
}
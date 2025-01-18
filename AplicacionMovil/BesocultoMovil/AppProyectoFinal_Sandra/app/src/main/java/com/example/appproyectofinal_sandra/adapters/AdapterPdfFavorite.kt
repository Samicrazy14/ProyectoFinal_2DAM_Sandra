package com.example.appproyectofinal_sandra.adapters

import android.content.Context
import android.content.Intent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.appproyectofinal_sandra.MyApplication
import com.example.appproyectofinal_sandra.R
import com.example.appproyectofinal_sandra.activities.PdfDetailActivity
import com.example.appproyectofinal_sandra.databinding.RowPdfFavoriteBinding
import com.example.appproyectofinal_sandra.models.ModelPdf
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.ValueEventListener

class AdapterPdfFavorite :RecyclerView.Adapter<AdapterPdfFavorite.HolderPdfFavorite> {

    private lateinit var binding: RowPdfFavoriteBinding
    private var context: Context

    //ArrayList de libros favoritos
    private var booksFavArrayList: ArrayList<ModelPdf>

    //Constructor
    constructor(context: Context, booksFavArrayList: ArrayList<ModelPdf>){
        this.context = context
        this.booksFavArrayList = booksFavArrayList
    }

    /**
     * Asigna valores y funcionalidades a la vista de los items de la lista de libros fav
     */
    override fun onBindViewHolder(holder: AdapterPdfFavorite.HolderPdfFavorite, position: Int) {
        val model = booksFavArrayList[position]

        loadBookDetails(model, holder)

        //Ir a ver libro
        holder.itemView.setOnClickListener {
            val intent = Intent(context, PdfDetailActivity::class.java)
            intent.putExtra("bookId", model.id)
            if (model.uid != "Project Gutenberg"){
                intent.putExtra("type", "Books")
            } else {
                intent.putExtra("type", "GutembergBooks")
            }
            context.startActivity(intent)
        }

        //Eliminar de favoritos
        holder.removeFavBtn.setOnClickListener {
            MyApplication.removeFromfavorite(context, model.id)
        }
    }

    /**
     * Cargar datos de los libros fav
     */
    private fun loadBookDetails(model: ModelPdf, holder: HolderPdfFavorite) {
        val bookId = model.id
        val type = model.uid
        println(model.id+" "+model.uid)

        val ref = FirebaseDatabase.getInstance().getReference(type)
        ref.child(bookId)
            .addListenerForSingleValueEvent(object: ValueEventListener{
                override fun onDataChange(snapshot: DataSnapshot) {
                    val categoryId = "${snapshot.child("categoryId").value}"
                    val description = "${snapshot.child("description").value}"
                    val timestamp = "${snapshot.child("timestamp").value}"
                    val title = "${snapshot.child("title").value}"
                    val uid = "${snapshot.child("uid").value}"
                    val url = "${snapshot.child("url").value}"
                    val viewsCount = "${snapshot.child("viewsCount").value}"
                    val pagesCount = "${snapshot.child("pagecount").value}"

                    model.categoryId = categoryId
                    model.description = description
                    model.timestamp = timestamp.toLong()
                    model.title = title
                    model.uid = uid
                    model.url = url
                    model.viewsCount = viewsCount.toLong()

                    val date = MyApplication.formatTimeStamp(timestamp.toLong())
                    if (type == "Books") {
                        println("Normal")
                        MyApplication.loadCategory("$categoryId", holder.categoryTv)

                        holder.pdfView.visibility = View.VISIBLE
                        holder.gutembergIv.visibility = View.INVISIBLE
                        MyApplication.loadPdfFromUrlSinglePage(
                            "$url",
                            holder.pdfView,
                            holder.progressBar,
                            null
                        )

                        holder.sizeTv.text = "${pagesCount} pág"
                    } else {
                        holder.categoryTv.text = categoryId
                        holder.pdfView.visibility = View.INVISIBLE
                        holder.gutembergIv.visibility = View.VISIBLE

                        val imagenUrl = "${snapshot.child("imagenUrl").value}"
                        model.imagenUrl = imagenUrl
                        println(imagenUrl)
                        try {
                            Glide.with(context)
                                .load(imagenUrl)
                                .placeholder(R.drawable.ic_person_gray)
                                .into(holder.gutembergIv)
                            holder.progressBar.visibility = View.INVISIBLE
                        } catch (e: Exception){
                            holder.gutembergIv.setImageResource(R.drawable.ic_person_gray)
                            holder.progressBar.visibility = View.INVISIBLE
                        }
                        holder.sizeTv.text = "${pagesCount} pág"
                    }

                    holder.titleTv.text = title
                    holder.descriptionTv.text = description
                    holder.dateTv.text = date
                }
                override fun onCancelled(error: DatabaseError) {
                }
            })
    }

    /**
     * Asigna la vista a los items de la lista de libros fav
     */
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): AdapterPdfFavorite.HolderPdfFavorite {
        binding = RowPdfFavoriteBinding.inflate(LayoutInflater.from(context), parent, false)
        return HolderPdfFavorite(binding.root)
    }

    /**
     * Tamaño de la lista de libros fav
     */
    override fun getItemCount(): Int {
        return booksFavArrayList.size
    }

    inner class HolderPdfFavorite(itemView: View) : RecyclerView.ViewHolder(itemView){
        var pdfView = binding.pdfView
        val gutembergIv = binding.gutembergIv
        var progressBar = binding.progressBar
        var titleTv = binding.titleTv
        var removeFavBtn = binding.removeFavBtn
        var descriptionTv = binding.descriptionTv
        var categoryTv = binding.categoryTv
        var sizeTv = binding.sizeTv
        var dateTv = binding.dateTv
    }
}
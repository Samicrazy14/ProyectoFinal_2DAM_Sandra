package com.example.appproyectofinal_sandra.adapters

import android.content.Context
import android.content.Intent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Filter
import android.widget.Filterable
import androidx.recyclerview.widget.RecyclerView
import com.bumptech.glide.Glide
import com.example.appproyectofinal_sandra.filters.FilterPdfUser
import com.example.appproyectofinal_sandra.MyApplication
import com.example.appproyectofinal_sandra.R
import com.example.appproyectofinal_sandra.activities.PdfDetailActivity
import com.example.appproyectofinal_sandra.databinding.RowPdfUserBinding
import com.example.appproyectofinal_sandra.models.ModelPdf
import com.google.firebase.database.FirebaseDatabase

class AdapterPdfUser :RecyclerView.Adapter<AdapterPdfUser.HolderPdfUser>, Filterable{

    private var context: Context
    private lateinit var binding: RowPdfUserBinding

    //ArrayList de libros
    var pdfArrayList: ArrayList<ModelPdf>

    //Lista filtrada
    private var filterList: ArrayList<ModelPdf>
    private var filter: FilterPdfUser? = null

    //Constructor
    constructor(
        context: Context,
        pdfArrayList: ArrayList<ModelPdf>
    ) : super() {
        this.context = context
        this.pdfArrayList = pdfArrayList
        this.filterList = pdfArrayList
    }

    /**
     * Asigna valores y funcionalidades a la vista de los items de la lista de libros
     */
    override fun onBindViewHolder(holder: HolderPdfUser, position: Int) {
        val model = pdfArrayList[position]
        val pdfId = model.id
        val categoryId = model.categoryId
        val title = model.title
        val description = model.description
        val url = model.url
        val timestamp = model.timestamp
        val date = MyApplication.formatTimeStamp(timestamp)
        val pages = model.pagecount

        holder.titleTv.text = title
        holder.descriptionTv.text = description
        holder.dateTv.text = date
        holder.sizeTv.text = "${pages} pág"

        //Ver si es libro firebase o gutemberg
        if(model.uid !== "Project Gutenberg") {
            MyApplication.loadCategory(categoryId, holder.categoryTv)
            holder.gutembergIv.visibility = View.INVISIBLE
            holder.pdfView.visibility = View.VISIBLE
            MyApplication.loadPdfFromUrlSinglePage(url, holder.pdfView, holder.progressBar, null)
            holder.sizeTv.text = "${pages} pág"
        } else {
            holder.categoryTv.text = categoryId
            holder.pdfView.visibility = View.INVISIBLE
            holder.gutembergIv.visibility = View.VISIBLE
            try {
                Glide.with(context)
                    .load(model.imagenUrl)
                    .placeholder(R.drawable.ic_person_gray)
                    .into(holder.gutembergIv)
                holder.progressBar.visibility = View.INVISIBLE
            } catch (e: Exception){
                holder.gutembergIv.setImageResource(R.drawable.ic_person_gray)
                holder.progressBar.visibility = View.INVISIBLE
            }
        }


        //Ir a ver libro
        holder.itemView.setOnClickListener {
            if(model.uid !== "Project Gutenberg") {
                val intent = Intent(context, PdfDetailActivity::class.java)
                intent.putExtra("bookId", pdfId)
                intent.putExtra("type", "Books")
                context.startActivity(intent)
            } else {
                //Montamos los datos del libro
                val hashMap: HashMap<String, Any> = HashMap()
                hashMap["author"] = "${model.author}"
                hashMap["categoryId"] = "${model.categoryId}"
                hashMap["description"] = "${model.description}"
                hashMap["id"] = "${model.id}"
                hashMap["imagenUrl"] = "${model.imagenUrl}"
                hashMap["pagecount"] = model.pagecount
                hashMap["timestamp"] = model.timestamp
                hashMap["title"] = "$title"
                hashMap["uid"] = "${model.uid}"
                hashMap["url"] = "${model.url}"
                hashMap["viewsCount"] = model.viewsCount

                val ref = FirebaseDatabase.getInstance().getReference("GutembergBooks")
                ref.child(model.id).get().addOnCompleteListener { task ->
                    if (task.isSuccessful) {
                        val snapshot = task.result
                        if (snapshot.exists()) {
                            val intent = Intent(context, PdfDetailActivity::class.java)
                            intent.putExtra("bookId", model.id)
                            intent.putExtra("type", "GutembergBooks")
                            context.startActivity(intent)
                        } else {
                            ref.child(model.id)
                                .setValue(hashMap)
                                .addOnSuccessListener {
                                    val intent = Intent(context, PdfDetailActivity::class.java)
                                    intent.putExtra("bookId", model.id)
                                    intent.putExtra("type", "GutembergBooks")
                                    context.startActivity(intent)
                                }
                        }
                    }
                }
            }
        }
    }

    /**
     * Asigna la vista a los items de la lista de libros
     */
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): HolderPdfUser {
        binding = RowPdfUserBinding.inflate(LayoutInflater.from(context), parent, false)
        return HolderPdfUser(binding.root)
    }

    /**
     * Tamaño de la lista de libros
     */
    override fun getItemCount(): Int {
        return pdfArrayList.size
    }

    inner class HolderPdfUser(itemView: View) : RecyclerView.ViewHolder(itemView){
        val pdfView = binding.pdfView
        val gutembergIv = binding.gutembergIv
        val progressBar = binding.progressBar
        val titleTv = binding.titleTv
        val descriptionTv = binding.descriptionTv
        val categoryTv = binding.categoryTv
        val sizeTv = binding.sizeTv
        val dateTv = binding.dateTv
    }

    override fun getFilter(): Filter {
        println(filter)
        if (filter == null){
            filter = FilterPdfUser(pdfArrayList, this)
        }
        return filter as FilterPdfUser
    }
}
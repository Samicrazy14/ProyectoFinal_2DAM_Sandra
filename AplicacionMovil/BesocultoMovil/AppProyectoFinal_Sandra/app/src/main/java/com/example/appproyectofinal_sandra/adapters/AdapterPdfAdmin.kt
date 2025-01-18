package com.example.appproyectofinal_sandra.adapters

import android.app.ProgressDialog
import android.content.Context
import android.content.Intent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Filter
import android.widget.Filterable
import androidx.appcompat.app.AlertDialog
import androidx.recyclerview.widget.RecyclerView
import com.example.appproyectofinal_sandra.filters.FilterPdfAdmin
import com.example.appproyectofinal_sandra.MyApplication
import com.example.appproyectofinal_sandra.activities.PdfDetailActivity
import com.example.appproyectofinal_sandra.activities.PdfEditActivity
import com.example.appproyectofinal_sandra.databinding.RowPdfAdminBinding
import com.example.appproyectofinal_sandra.models.ModelPdf

class AdapterPdfAdmin :RecyclerView.Adapter<AdapterPdfAdmin.HolderPdfAdmin>, Filterable{

    private var context: Context
    private lateinit var binding: RowPdfAdminBinding
    private lateinit var progressDialog: ProgressDialog

    //ArrayList de libros
    var pdfArrayList: ArrayList<ModelPdf>

    //Lista filtrada
    private var filterList: ArrayList<ModelPdf>
    private var filter: FilterPdfAdmin? = null

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
    override fun onBindViewHolder(holder: HolderPdfAdmin, position: Int) {
        val model = pdfArrayList[position]
        val pdfId = model.id
        val categoryId = model.categoryId
        val title = model.title
        val description = model.description
        val pdfUrl = model.url
        val timestamp = model.timestamp
        val formattedDate = MyApplication.formatTimeStamp(timestamp)
        val pages = model.pagecount

        //init progress
        progressDialog = ProgressDialog(context)
        progressDialog.setTitle("Por favor, espere")
        progressDialog.setCanceledOnTouchOutside(false)

        holder.titleTv.text = title
        holder.descriptionTv.text = description
        holder.dateTv.text = formattedDate

        MyApplication.loadCategory(categoryId, holder.categoryTv)

        MyApplication.loadPdfFromUrlSinglePage(
            pdfUrl,
            holder.pdfView,
            holder.progressBar,
            null
        )

        holder.sizeTv.text = "${pages} pág"
        //Opciones de modificar el libro, editar o borrar
        holder.moreBtn.setOnClickListener {
            moreOptionsDialog(model)
        }

        //Ir a ver libro
        holder.itemView.setOnClickListener {
            val intent = Intent(context, PdfDetailActivity::class.java)
            intent.putExtra("bookId", pdfId)
            intent.putExtra("type", "Books")
            context.startActivity(intent)
        }
    }

    /**
     * Opciones de modificar el libro, editar o borrar
     */
    private fun moreOptionsDialog(model: ModelPdf) {
        val bookId = model.id
        val bookUrl = model.url
        val bookTitle = model.title


        val options = arrayOf("Editar", "Borrar")

        val builder = AlertDialog.Builder(context)
        builder.setTitle("Elige una opción")
            .setItems(options){ _, position->
                if(position == 0){
                    val intent = Intent(context, PdfEditActivity::class.java)
                    intent.putExtra("bookId", bookId)
                    intent.putExtra("type", "Books")
                    context.startActivity(intent)
                } else if(position == 1){
                    MyApplication.deleteBook(context, bookId, bookUrl, bookTitle, "Books")
                }
            }
            .show()
    }

    /**
     * Asigna la vista a los items de la lista de libros
     */
    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): HolderPdfAdmin {
        binding = RowPdfAdminBinding.inflate(LayoutInflater.from(context), parent, false)
        return HolderPdfAdmin(binding.root)
    }

    /**
     * Tamaño de la lista de libros
     */
    override fun getItemCount(): Int {
        return pdfArrayList.size
    }

    inner class HolderPdfAdmin(itemView: View) : RecyclerView.ViewHolder(itemView){
        val pdfView = binding.pdfView
        val progressBar = binding.progressBar
        val titleTv = binding.titleTv
        val descriptionTv = binding.descriptionTv
        val categoryTv = binding.categoryTv
        val sizeTv = binding.sizeTv
        val dateTv = binding.dateTv
        val moreBtn = binding.moreBtn
    }

    override fun getFilter(): Filter {
        if (filter == null){
            filter = FilterPdfAdmin(filterList, this)
        }
        return filter as FilterPdfAdmin
    }
}
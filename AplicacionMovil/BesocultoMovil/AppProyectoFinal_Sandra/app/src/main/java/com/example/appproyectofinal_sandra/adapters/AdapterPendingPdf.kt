package com.example.appproyectofinal_sandra.adapters

import android.app.ProgressDialog
import android.content.Context
import android.content.Intent
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Filter
import android.widget.Filterable
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.recyclerview.widget.RecyclerView
import com.example.appproyectofinal_sandra.MyApplication
import com.example.appproyectofinal_sandra.activities.PdfDetailActivity
import com.example.appproyectofinal_sandra.activities.PdfEditActivity
import com.example.appproyectofinal_sandra.databinding.RowPdfAdminBinding
import com.example.appproyectofinal_sandra.filters.FilterPendingPdf
import com.example.appproyectofinal_sandra.models.ModelPdf
import com.google.firebase.database.FirebaseDatabase

class AdapterPendingPdf :RecyclerView.Adapter<AdapterPendingPdf.HolderPdfAdmin>, Filterable{

    private var context: Context
    private lateinit var binding: RowPdfAdminBinding
    private lateinit var progressDialog: ProgressDialog

    //ArrayList de libros
    var pdfArrayList: ArrayList<ModelPdf>

    //Lista filtrada
    private var filterList: ArrayList<ModelPdf>
    private var filter: FilterPendingPdf? = null

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
        holder.sizeTv.text = "${pages} pág"
        MyApplication.loadCategory(categoryId, holder.categoryTv)

        MyApplication.loadPdfFromUrlSinglePage(
            pdfUrl,
            holder.pdfView,
            holder.progressBar,
            null
        )

        //Opciones de modificar el libro, editar o borrar
        holder.moreBtn.setOnClickListener {
            moreOptionsDialog(model)
        }

        //Ir a ver libro
        holder.itemView.setOnClickListener {
            val intent = Intent(context, PdfDetailActivity::class.java)
            intent.putExtra("bookId", pdfId)
            intent.putExtra("type", "PendingBooks")
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


        val options = arrayOf("Subir","Editar", "Borrar")

        val builder = AlertDialog.Builder(context)
        builder.setTitle("Elige una opción")
            .setItems(options){ _, position->
                if(position == 0){
                    moveBookFromPendingToBooks(bookId)
                } else if(position == 1){
                    val intent = Intent(context, PdfEditActivity::class.java)
                    intent.putExtra("bookId", bookId)
                    intent.putExtra("type", "PendingBooks")
                    context.startActivity(intent)
                } else if(position == 2){
                    MyApplication.deleteBook(context, bookId, bookUrl, bookTitle, "PendingBooks")
                }
            }
            .show()
    }

    /**
     * Mueve una entrada de PendingBooks a Books
     */
    private fun moveBookFromPendingToBooks(bookId: String) {
        val TAG = "PDF_MOVE_TAG"
        Log.d(TAG, "moveBookFromPendingToBooks: moviendo libro con id $bookId de PendingBooks a Books")

        // Mostrar un ProgressDialog para indicar que el proceso está en curso
        progressDialog.setMessage("Moviendo libro...")
        progressDialog.show()

        // Referencia a la base de datos de Firebase para PendingBooks y Books
        val pendingRef = FirebaseDatabase.getInstance().getReference("PendingBooks")
        val booksRef = FirebaseDatabase.getInstance().getReference("Books")

        // Leer el libro de PendingBooks
        pendingRef.child(bookId)
            .get()
            .addOnCompleteListener { task ->
                if (task.isSuccessful) {
                    val snapshot = task.result
                    if (snapshot != null && snapshot.exists()) {
                        // Obtener datos del libro de PendingBooks
                        val bookData = snapshot.value as? HashMap<String, Any> ?: run {
                            Log.e(TAG, "moveBookFromPendingToBooks: Datos del libro son null o de formato incorrecto")
                            progressDialog.dismiss()
                            Toast.makeText(context, "Datos del libro incorrectos", Toast.LENGTH_LONG).show()
                            return@addOnCompleteListener
                        }

                        // Mover libro a Books
                        booksRef.child(bookId).setValue(bookData)
                            .addOnSuccessListener {
                                Log.d(TAG, "moveBookFromPendingToBooks: libro movido a Books")

                                // Eliminar libro de PendingBooks
                                pendingRef.child(bookId).removeValue()
                                    .addOnSuccessListener {
                                        Log.d(TAG, "moveBookFromPendingToBooks: libro eliminado de PendingBooks")
                                        progressDialog.dismiss()
                                        Toast.makeText(context, "Libro movido con éxito", Toast.LENGTH_SHORT).show()
                                    }
                                    .addOnFailureListener { e ->
                                        Log.e(TAG, "moveBookFromPendingToBooks: Fallo al eliminar de PendingBooks: ${e.message}")
                                        progressDialog.dismiss()
                                        Toast.makeText(context, "Error al eliminar de PendingBooks: ${e.message}", Toast.LENGTH_LONG).show()
                                    }
                            }
                            .addOnFailureListener { e ->
                                Log.e(TAG, "moveBookFromPendingToBooks: Fallo al mover a Books: ${e.message}")
                                progressDialog.dismiss()
                                Toast.makeText(context, "Error al mover a Books: ${e.message}", Toast.LENGTH_LONG).show()
                            }
                    } else {
                        Log.e(TAG, "moveBookFromPendingToBooks: No existe el libro en PendingBooks")
                        progressDialog.dismiss()
                        Toast.makeText(context, "El libro no existe en PendingBooks", Toast.LENGTH_LONG).show()
                    }
                } else {
                    val exception = task.exception
                    Log.e(TAG, "moveBookFromPendingToBooks: Fallo al leer de PendingBooks: ${exception?.message}")
                    progressDialog.dismiss()
                    Toast.makeText(context, "Error al leer de PendingBooks: ${exception?.message}", Toast.LENGTH_LONG).show()
                }
            }
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
            filter = FilterPendingPdf(filterList, this)
        }
        return filter as FilterPendingPdf
    }
}
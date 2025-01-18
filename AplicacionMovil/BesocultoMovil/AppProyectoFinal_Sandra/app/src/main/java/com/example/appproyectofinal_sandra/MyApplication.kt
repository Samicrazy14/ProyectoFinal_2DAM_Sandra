package com.example.appproyectofinal_sandra

import android.app.Application
import android.app.ProgressDialog
import android.content.Context
import android.util.Log
import android.view.View
import android.widget.ProgressBar
import android.widget.TextView
import android.widget.Toast
import com.github.barteksc.pdfviewer.PDFView
import com.google.firebase.auth.FirebaseAuth
import com.google.firebase.database.DataSnapshot
import com.google.firebase.database.DatabaseError
import com.google.firebase.database.FirebaseDatabase
import com.google.firebase.database.GenericTypeIndicator
import com.google.firebase.database.MutableData
import com.google.firebase.database.Transaction
import com.google.firebase.database.ValueEventListener
import com.google.firebase.storage.FirebaseStorage
import java.text.SimpleDateFormat
import java.util.Calendar
import java.util.Locale

class MyApplication: Application() {
    companion object{

        /**
         *  Formatea de fecha Long a dd/MM/yyyy
         */
        fun formatTimeStamp(timestamp: Long) : String{
            val cal = Calendar.getInstance(Locale.ENGLISH)
            cal.timeInMillis = timestamp
            val formatter = SimpleDateFormat("dd/MM/yyyy", Locale.ENGLISH)
            return formatter.format(cal.time)
        }

        /**
         *  Formatea la fecha de año a ms long
         */
        fun yearToMillis(year: String): Long {
            val calendar = Calendar.getInstance(Locale.ENGLISH)
            calendar.clear()

            return try {
                // Caso 1: Si el input es solo un año (ej: "1920")
                if (year.length == 4 && year.toIntOrNull() != null) {
                    val year = year.toInt()
                    calendar.set(Calendar.YEAR, year)
                    calendar.set(Calendar.DAY_OF_YEAR, 1)  // Para asegurar que se toma el 1 de enero
                    calendar.timeInMillis
                }
                // Caso 2: Si el input es año y mes (ej: "2010-08")
                else if (year.length == 7 && year.matches(Regex("\\d{4}-\\d{2}"))) {
                    val format = SimpleDateFormat("yyyy-MM", Locale.ENGLISH)
                    val date = format.parse(year) ?: throw IllegalArgumentException("Formato de fecha no válido")
                    date.time
                }
                // Caso 3: Si el input es una fecha completa (ej: "2022-05-28")
                else if (year.length == 10 && year.matches(Regex("\\d{4}-\\d{2}-\\d{2}"))) {
                    val format = SimpleDateFormat("yyyy-MM-dd", Locale.ENGLISH)
                    val date = format.parse(year) ?: throw IllegalArgumentException("Formato de fecha no válido")
                    date.time
                } else {
                    throw IllegalArgumentException("Formato no soportado")
                }
            } catch (e: Exception) {
                throw IllegalArgumentException("Error al procesar la entrada: ${e.message}")
            }
        }

        /**
         *  Cargar imagen de portada y numero de paginas de un libro
         */
        fun loadPdfFromUrlSinglePage(pdfUrl: String, pdfView: PDFView, progressBar: ProgressBar, pagesTv: TextView?){
            val TAG = "PDF_THUMBNAIL_TAG"

            val ref = FirebaseStorage.getInstance().getReferenceFromUrl(pdfUrl)
            ref.getBytes(Constants.MAX_BYTES_PDF)
                .addOnSuccessListener {bytes ->
                    Log.d(TAG,"loadPdfFromUrlSinglePage: Sizebytes $bytes")

                    pdfView.fromBytes(bytes)
                        .pages(0)//show first page only
                        .spacing(0)
                        .swipeHorizontal(false)
                        .enableSwipe(false)
                        .onError { t->
                            progressBar.visibility = View.INVISIBLE
                            Log.d(TAG,"loadPdfFromUrlSinglePage: ${t.message}")
                        }
                        .onPageError { _, t->
                            progressBar.visibility = View.INVISIBLE
                            Log.d(TAG,"loadPdfFromUrlSinglePage: ${t.message}")

                        }
                        .onLoad{ nbPages->
                            //NO FUNCIONA SIEMPRE DEPENDE DEL DOCUMENTO
                            Log.d(TAG,"loadPdfFromUrlSinglePage: Pages: $nbPages")
                            progressBar.visibility = View.INVISIBLE

                            if(pagesTv != null){
                                pagesTv.text = "$nbPages"
                            }
                        }
                        .load()
                }
                .addOnFailureListener{ e ->
                    Log.d(TAG,"loadPdfFromUrlSinglePage: Fallo al coger portada: ${e.message}")
                }
        }

        /**
         *  Carga la categoria de un libro
         */
        fun loadCategory(categoryId: String, categoryTv: TextView){
            val ref = FirebaseDatabase.getInstance().getReference("Categories")
            ref.child(categoryId)
                .addListenerForSingleValueEvent(object: ValueEventListener {
                    override fun onDataChange(snapshot: DataSnapshot) {
                        val category = "${snapshot.child("category").value}"
                        categoryTv.text = category
                    }
                    override fun onCancelled(error: DatabaseError) {
                    }
                })
        }

        /**
         *  Eliminar archivo y datos de un libro
         */
        fun deleteBook(context: Context, bookId: String, bookUrl: String, bookTitle: String, type: String){
            val TAG = "DELETE_PDF_TAG"
            val progressDialog = ProgressDialog(context)

            progressDialog.setTitle("Por favor, espere")
            progressDialog.setMessage("Borrando $bookTitle...")
            progressDialog.setCanceledOnTouchOutside(false)
            progressDialog.show()

            Log.d(TAG,"deleteBook in storage")

            val storageReference = FirebaseStorage.getInstance().getReferenceFromUrl(bookUrl)
            storageReference.delete()
                .addOnSuccessListener {
                    Log.d(TAG,"deleteBook: eliminado del storage")
                    Log.d(TAG,"deleteBook: eliminando de db")

                    val ref = FirebaseDatabase.getInstance().getReference(type)
                    ref.child(bookId)
                        .removeValue()
                        .addOnSuccessListener {
                            Log.d(TAG,"deleteBook: Eliminado con exito")
                            progressDialog.dismiss()
                            Toast.makeText(context, "Eliminado con exito", Toast.LENGTH_SHORT).show()
                        }
                        .addOnFailureListener{ e ->
                            Log.d(TAG,"deleteBook: Fallo al eliminar de la db: ${e.message}")
                            progressDialog.dismiss()
                            Toast.makeText(context, "Fallo al eliminar de la db: ${e.message}", Toast.LENGTH_LONG).show()
                        }

                }
                .addOnFailureListener { e ->
                    progressDialog.dismiss()
                    Log.d(TAG,"deleteBook: fallo al eliminar del storage: ${e.message}")
                    Toast.makeText(context, "Fallo al eliminar del storage: ${e.message}", Toast.LENGTH_LONG).show()
                }
        }

        /**
         *  Eliminar usuario e imagen de storage
         */
        fun deleteUser(context: Context, uid: String, email: String, profileImage: String){
            val TAG = "DELETE_USER_TAG"
            val progressDialog = ProgressDialog(context)

            progressDialog.setTitle("Por favor, espere")
            progressDialog.setMessage("Borrando $email...")
            progressDialog.setCanceledOnTouchOutside(false)
            progressDialog.show()

            Log.d(TAG,"deleteUser in storage")

            val storageReference = FirebaseStorage.getInstance().getReferenceFromUrl(profileImage)
            storageReference.delete()
                .addOnSuccessListener {
                    Log.d(TAG,"deleteUser: eliminado del storage")
                    Log.d(TAG,"deleteUser: eliminando de db")

                    val ref = FirebaseDatabase.getInstance().getReference("Users")
                    ref.child(uid)
                        .removeValue()
                        .addOnSuccessListener {
                            Log.d(TAG,"deleteUser: Eliminado con exito")
                            progressDialog.dismiss()
                            Toast.makeText(context, "Eliminado con exito", Toast.LENGTH_SHORT).show()
                        }
                        .addOnFailureListener{ e ->
                            Log.d(TAG,"deleteUser: Fallo al eliminar de la db: ${e.message}")
                            progressDialog.dismiss()
                            Toast.makeText(context, "Fallo al eliminar de la db: ${e.message}", Toast.LENGTH_LONG).show()
                        }

                }
                .addOnFailureListener { e ->
                    progressDialog.dismiss()
                    Log.d(TAG,"deleteUser: fallo al eliminar del storage: ${e.message}")
                    Toast.makeText(context, "Fallo al eliminar del storage: ${e.message}", Toast.LENGTH_LONG).show()
                }
        }

        /**
         *  Aumentar las vistas del libro
         */
        fun incrementBookViewCount(bookId: String, type: String){
            val ref = FirebaseDatabase.getInstance().getReference(type).child(bookId)
            ref.runTransaction(object : Transaction.Handler {
                override fun doTransaction(mutableData: MutableData): Transaction.Result {
                    val currentBook = mutableData.getValue(object : GenericTypeIndicator<HashMap<String, Any>>() {})
                        ?: return Transaction.abort()

                    // Obtenemos el valor actual de viewsCount
                    val viewsCount = when (val currentViews = currentBook["viewsCount"]) {
                        is Long -> currentViews
                        is Int -> currentViews.toLong()
                        is String -> currentViews.toLongOrNull() ?: 0L
                        else -> 0L
                    }

                    // Incrementamos viewsCount
                    val newViewsCount = viewsCount + 1

                    // Actualizamos el valor en el mapa
                    currentBook["viewsCount"] = newViewsCount

                    // Establecemos el valor actualizado
                    mutableData.value = currentBook

                    return Transaction.success(mutableData)
                }

                override fun onComplete(
                    error: DatabaseError?,
                    committed: Boolean,
                    currentData: DataSnapshot?
                ) {
                    if (error != null) {
                        Log.e("Firebase", "Error al actualizar viewsCount", error.toException())
                    } else {
                        Log.d("Firebase", "viewsCount actualizado exitosamente")
                    }
                }
            })
        }

        /**
         * Eliminar de favoritos
         */
        fun removeFromfavorite(context: Context, bookId: String){
            val TAG = "REMOVE_FAV_TAG"
             val firebaseAuth = FirebaseAuth.getInstance()

            Log.d(TAG, "removeFromfavorite")
            val ref = FirebaseDatabase.getInstance().getReference("Users")
            ref.child(firebaseAuth.uid!!).child("Favorites").child(bookId)
                .removeValue()
                .addOnSuccessListener {
                    Log.d(TAG, "removeFromfavorite: Success")
                    Toast.makeText(context, "Eliminado de favoritos", Toast.LENGTH_SHORT)
                        .show()
                }
                .addOnFailureListener{ e ->
                    Log.d(TAG, "removeFromfavorite: Fallo al eliminar de favoritos: ${e.message}")
                    Toast.makeText(context, "Fallo al eliminar de favoritos: ${e.message}", Toast.LENGTH_LONG)
                        .show()
                }
        }
    }
}
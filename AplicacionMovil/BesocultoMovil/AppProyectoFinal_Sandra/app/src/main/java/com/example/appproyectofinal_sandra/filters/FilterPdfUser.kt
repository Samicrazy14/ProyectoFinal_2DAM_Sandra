package com.example.appproyectofinal_sandra.filters

import android.util.Log
import android.widget.Filter
import com.example.appproyectofinal_sandra.adapters.AdapterPdfUser
import com.example.appproyectofinal_sandra.models.ModelPdf

class FilterPdfUser : Filter {

    private var filterList: ArrayList<ModelPdf>
    private var adapterPdfUser: AdapterPdfUser

    //Constructor
    constructor(filterList: ArrayList<ModelPdf>, adapterPdfUser: AdapterPdfUser):super() {
        this.filterList = filterList
        this.adapterPdfUser = adapterPdfUser
    }

    /**
     * Filtrar Libros por busqueda
     */
    override fun performFiltering(constraint: CharSequence): FilterResults {
        var constraint:CharSequence? = constraint
        val results = FilterResults()
        Log.d("Filtering", "performFiltering: $constraint")
        if(!constraint.isNullOrEmpty()){
            constraint = constraint.toString().uppercase()
            var filteredModels = ArrayList<ModelPdf>()
            for (i in filterList.indices){
                if(filterList[i].title.uppercase().contains(constraint)){
                  filteredModels.add(filterList[i])
                }
            }
            results.count = filteredModels.size
            results.values = filteredModels
        }
        else{
            results.count = filterList.size
            results.values = filterList
        }
        return results
    }

    /**
     * Mostrar los libros filtrados
     */
    override fun publishResults(constraint: CharSequence, results: FilterResults) {
        adapterPdfUser.pdfArrayList = results.values as ArrayList<ModelPdf>
        adapterPdfUser.notifyDataSetChanged()
    }
}
package com.example.appproyectofinal_sandra.filters

import android.widget.Filter
import com.example.appproyectofinal_sandra.adapters.AdapterPdfAdmin
import com.example.appproyectofinal_sandra.models.ModelPdf

class FilterPdfAdmin : Filter {

    private var filterList: ArrayList<ModelPdf>
    private var adapterPdfAdmin: AdapterPdfAdmin

    //Constructor
    constructor(filterList: ArrayList<ModelPdf>, adapterPdfAdmin: AdapterPdfAdmin) {
        this.filterList = filterList
        this.adapterPdfAdmin = adapterPdfAdmin
    }

    /**
     * Filtrar Libros por busqueda
     */
    override fun performFiltering(constraint: CharSequence?): FilterResults {
        var constraint:CharSequence? = constraint
        val results = FilterResults()

        if(!constraint.isNullOrEmpty()){
            constraint = constraint.toString().lowercase()
            var filteredModels = ArrayList<ModelPdf>()
            for (i in filterList.indices){
                if(filterList[i].title.lowercase().contains(constraint)){
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
        adapterPdfAdmin.pdfArrayList = results.values as ArrayList<ModelPdf>
        adapterPdfAdmin.notifyDataSetChanged()
    }
}
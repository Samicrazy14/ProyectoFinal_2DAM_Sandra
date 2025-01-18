package com.example.appproyectofinal_sandra.filters

import android.widget.Filter
import com.example.appproyectofinal_sandra.adapters.AdapterCategory
import com.example.appproyectofinal_sandra.models.ModelCategory
import kotlin.collections.ArrayList

class FilterCategory: Filter {

    private var filterList: ArrayList<ModelCategory>
    private var adapterCategory: AdapterCategory

    //Constructor
    constructor(filterList: ArrayList<ModelCategory>, adapterCategory: AdapterCategory) : super() {
        this.filterList = filterList
        this.adapterCategory = adapterCategory
    }

    /**
     * Filtrar Categoria por busqueda
     */
    override fun performFiltering(constraint: CharSequence?): FilterResults {
       var constraint = constraint
       val results = FilterResults()

        if (!constraint.isNullOrEmpty()){
            constraint = constraint.toString().uppercase()
            val filteredModels:ArrayList<ModelCategory> = ArrayList()
            for (i in 0 until filterList.size){
                if (filterList[i].category.uppercase().contains(constraint)){
                    filteredModels.add(filterList[i])
                }
            }
            results.count = filteredModels.size
            results.values = filteredModels
        }
        else {
            results.count = filterList.size
            results.values = filterList
        }
        return results
    }

    /**
     * Mostrar las categorías filtradas
     */
    override fun publishResults(categoryArrayList: CharSequence?, results: FilterResults) {
        adapterCategory.categoryArrayList = results.values as ArrayList<ModelCategory>
        adapterCategory.notifyDataSetChanged()
    }
}
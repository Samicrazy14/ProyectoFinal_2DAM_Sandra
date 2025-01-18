package com.example.appproyectofinal_sandra.filters

import android.util.Log
import android.widget.Filter
import com.example.appproyectofinal_sandra.adapters.AdapterUser
import com.example.appproyectofinal_sandra.models.ModelUser

class FilterUser : Filter {

    private var filterList: ArrayList<ModelUser>
    private var adapterUser: AdapterUser

    //Constructor
    constructor(filterList: ArrayList<ModelUser>, adapterUser: AdapterUser):super() {
        this.filterList = filterList
        this.adapterUser = adapterUser
    }

    /**
     * Filtrar usuarios por busqueda
     */
    override fun performFiltering(constraint: CharSequence): FilterResults {
        var constraint:CharSequence? = constraint
        val results = FilterResults()
        Log.d("Filtering", "performFiltering: $constraint")
        if(!constraint.isNullOrEmpty()){
            constraint = constraint.toString().uppercase()
            var filteredModels = ArrayList<ModelUser>()
            for (i in filterList.indices){
                if(filterList[i].name.uppercase().contains(constraint)){
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
     * Mostrar los usuarios filtrados
     */
    override fun publishResults(constraint: CharSequence, results: FilterResults) {
        adapterUser.userArrayList = results.values as ArrayList<ModelUser>
        adapterUser.notifyDataSetChanged()
    }
}
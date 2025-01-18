package com.example.appproyectofinal_sandra.models

class ModelCategory {

    var id:String = ""
    var category:String = ""
    var timestamp:Long = 0
    var uid:String = ""

    //Constructor vacío de firebase
    constructor()

    //Constructor
    constructor(id:String, category:String, timestamp:Long, uid:String){
        this.id = id
        this.category = category
        this.timestamp = timestamp
        this.uid = uid
    }
}
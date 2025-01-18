package com.example.appproyectofinal_sandra.models

class ModelComment {
    var id = ""
    var bookId = ""
    var timestamp = ""
    var comment = ""
    var uid = ""

    //Constructor vacio de firebase
    constructor()

    //Constructor
    constructor(id:String, bookId:String, timestamp:String, comment:String, uid:String){
        this.id = id
        this.bookId = bookId
        this.timestamp = timestamp
        this.comment = comment
        this.uid = uid
    }
}
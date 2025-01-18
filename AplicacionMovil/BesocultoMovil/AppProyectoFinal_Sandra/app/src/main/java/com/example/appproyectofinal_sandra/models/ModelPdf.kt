package com.example.appproyectofinal_sandra.models

class ModelPdf {

    var author:String = ""
    var categoryId:String = ""
    var description:String = ""
    var id:String = ""
    var imagenUrl:String = ""
    var pagecount:Long = 0
    var timestamp:Long = 0
    var title:String = ""
    var uid:String = ""
    var url:String = ""
    var viewsCount:Long = 0

    constructor()

    //Constructor firebase
    constructor(
        uid: String,
        id: String,
        title: String,
        description: String,
        categoryId: String,
        pagecount: Long,
        url: String,
        timestamp: Long,
        viewsCount: Long
    ) {
        this.uid = uid
        this.id = id
        this.title = title
        this.description = description
        this.categoryId = categoryId
        this.pagecount = pagecount
        this.url = url
        this.timestamp = timestamp
        this.viewsCount = viewsCount
    }

    //Constructor gutemberg api
    constructor(
        author: String,
        categoryId: String,
        description: String,
        id: String,
        imagenUrl: String,
        pagecount: Long,
        timestamp: Long,
        title: String,
        uid: String,
        url: String,
        viewsCount: Long
    ) {
        this.author = author
        this.categoryId = categoryId
        this.description = description
        this.id = id
        this.imagenUrl = imagenUrl
        this.pagecount = pagecount
        this.timestamp = timestamp
        this.title = title
        this.uid = uid
        this.url = url
        this.viewsCount = viewsCount
    }


}
package com.example.appproyectofinal_sandra.models

class ModelUser {
    var uid:String = ""
    var email:String = ""
    var name:String = ""
    var profileImage:String = ""
    var timestamp:Long = 0
    var userType:String = ""

    //Constructor vacio de firebase
    constructor()

    //Constructor
    constructor(uid:String, email:String, name:String, profileImage:String, timestamp: Long, userType:String){
        this.uid = uid
        this.email = email
        this.name = name
        this.profileImage = profileImage
        this.timestamp = timestamp
        this.userType = userType
    }
}
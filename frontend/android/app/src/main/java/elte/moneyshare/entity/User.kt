package elte.moneyshare.entity

data class User (
    var _id: String,
    var email: String,
    var password: String,
    var dispalyname: String? = null,
    var state: String? = null,
    var groups: ArrayList<String>? = null
)
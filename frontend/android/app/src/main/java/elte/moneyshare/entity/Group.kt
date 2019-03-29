package elte.moneyshare.entity

data class Group(
    var creator: String,
    var name: String,
    var balance: Int,
    var memberCount: Int,
    var members: ArrayList<String>
)

package elte.moneyshare.entity

data class GroupInfo (
    var id: Int,
    var name: String,
    var creator: String,
    var memberCount: Int,
    var myCurrentBalance: Int
)
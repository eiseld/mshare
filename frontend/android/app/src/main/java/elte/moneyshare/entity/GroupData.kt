package elte.moneyshare.entity

data class GroupData (
    var id: Int,
    var name: String,
    var creator: Member,
    var members: ArrayList<Member>,
    var memberCount: Int,
    var myCurrentBalance: Int
)

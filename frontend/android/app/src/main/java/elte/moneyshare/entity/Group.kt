package elte.moneyshare.entity

data class Group(
    var id: Int,
    var name: String,
    var creatorUser: GroupUser,
    var members: ArrayList<GroupUser>,
    var memberCount: Int,
    var balance: Int
)

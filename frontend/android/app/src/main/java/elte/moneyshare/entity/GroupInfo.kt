package elte.moneyshare.entity

import java.util.*

data class GroupInfo (
    var id: Int,
    var name: String,
    var creator: String,
    var memberCount: Int,
    var myCurrentBalance: Int,
    var lastModified : String
)
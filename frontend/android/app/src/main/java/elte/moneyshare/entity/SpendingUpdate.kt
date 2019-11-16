package elte.moneyshare.entity

data class SpendingUpdate (
    var groupId: Int,
    var name: String,
    var id: Int,
    var creditorUserId : Int,
    var moneySpent: Int,
    var debtors: ArrayList<Debtor>,
    var date: String
)
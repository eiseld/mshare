package elte.moneyshare.entity

data class NewSpending (
    var groupId: Int,
    var name: String,
    var moneySpent: Int,
    var debtors: ArrayList<Debtor>
)
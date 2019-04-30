package elte.moneyshare.entity

data class SpendingData (
    var name: String,
    var moneyOwed: Int,
    var debtors: ArrayList<DebtorData>
)
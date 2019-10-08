package elte.moneyshare.entity

data class SpendingData (
    var id: Int,
    var name: String,
    var creditor : UserData,
    var creditorUserId : Int,
    var moneyOwed: Int,
    var debtors: ArrayList<DebtorData>
)
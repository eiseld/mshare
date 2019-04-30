package elte.moneyshare.entity

data class OptimizedDebtData (
    var Debtor : UserData,
    var Creditor : UserData,
    var OptimisedDebtAmount : Long
)
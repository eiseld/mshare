package elte.moneyshare.entity

data class OptimizedDebtData (
    var debtor : UserData,
    var creditor : UserData,
    var optimisedDebtAmount : Long
)
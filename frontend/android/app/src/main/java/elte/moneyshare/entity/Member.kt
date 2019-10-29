package elte.moneyshare.entity

data class Member(
    var id: Int,
    var name: String,
    var balance: Int,
    var bankAccountNumber: String
)
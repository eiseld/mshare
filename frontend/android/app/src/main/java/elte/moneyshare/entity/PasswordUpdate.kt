package elte.moneyshare.entity

data class PasswordUpdate(
    var email: String,
    var token: String,
    var password: String
)
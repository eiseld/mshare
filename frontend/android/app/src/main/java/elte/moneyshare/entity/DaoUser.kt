package elte.moneyshare.entity

data class DaoUser (
    var id: Int,
    var email: String,
    var password: String,
    var displayName: String,
    var creationDate: String
)
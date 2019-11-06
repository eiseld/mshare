package elte.moneyshare.entity

import elte.moneyshare.SharedPreferences

data class LoginCred (
    var email: String,
    var password: String,
    val lang : String = SharedPreferences.lang
)
package elte.moneyshare

import android.content.Context
import android.content.SharedPreferences

object SharedPreferences {

    private val prefKey = "prefKey"

    private val ACCESS_TOKEN = "access_token"
    private val USER_LOGGED_IN = "user_logged_in"

    private lateinit var sharedPreferences: SharedPreferences

    //TODO CHECK ACCESS_TOKEN HANDLING TO STILL STORE AFTER APP CLOSED (in APIClient headers creation)
    fun init(context: Context) {
        sharedPreferences = context.getSharedPreferences(prefKey, Context.MODE_PRIVATE)
        accessToken = ""
    }

    var accessToken: String
        get() = sharedPreferences.getString(ACCESS_TOKEN, "")
        set(accessToken) {
            with(sharedPreferences.edit()) {
                val bearerToken = "Bearer $accessToken"
                putString(ACCESS_TOKEN, bearerToken)
                apply()
            }
        }

    var isUserLoggedIn: Boolean
        get() = sharedPreferences.getBoolean(USER_LOGGED_IN, false)
        set(isUserLoggedIn) {
            with(sharedPreferences.edit()) {
                putBoolean(USER_LOGGED_IN, isUserLoggedIn)
                apply()
            }
        }
}
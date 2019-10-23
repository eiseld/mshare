package elte.moneyshare

import android.content.Context
import android.content.SharedPreferences
import elte.moneyshare.entity.User
import java.util.*

object SharedPreferences {

    private val prefKey = "prefKey"

    private val ACCESS_TOKEN = "access_token"
    private val USER_LOGGED_IN = "user_logged_in"
    private val STAY_LOGGED_IN = "stay_logged_in"

    private val USER_ID = "user_id"
    private val USER_NAME = "user_name"
    private val USER_EMAIL = "user_email"

    private val LANG = "lang"

    private val DELETE_MEMBER_ENABLED = "delete_member_enabled"

    private lateinit var user: User
    private lateinit var sharedPreferences: SharedPreferences

    //TODO CHECK ACCESS_TOKEN HANDLING TO STILL STORE AFTER APP CLOSED (in APIClient headers creation)
    fun init(context: Context) {
        sharedPreferences = context.getSharedPreferences(prefKey, Context.MODE_PRIVATE)
        //accessToken = ""
        //userId = -1
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
    var userId : Int
        get() = sharedPreferences.getInt(USER_ID, -1)
        set(id) {
            with(sharedPreferences.edit()) {
                putInt(USER_ID, id)
                apply()
            }
        }

    var userName : String?
        get() = sharedPreferences.getString(USER_NAME, "")
        set(name) {
            with(sharedPreferences.edit()) {
                putString(USER_NAME, name)
                apply()
            }
        }

    var email : String?
        get() = sharedPreferences.getString(USER_EMAIL, "")
        set(email) {
            with(sharedPreferences.edit()) {
                putString(USER_EMAIL, email)
                apply()
            }
        }

    //todo should be enum
    var lang : String
        get() = sharedPreferences.getString(
            LANG,
            if (Locale.getDefault().language == Locale("en").language) "EN" else "HU"
        ) ?: "HU"
        set(lang) {
            with(sharedPreferences.edit()) {
                putString(LANG, lang)
                apply()
            }
        }

    var isDeleteMemberEnabled : Boolean
        get() = sharedPreferences.getBoolean(DELETE_MEMBER_ENABLED, false)
        set(enabled)
        {
            with(sharedPreferences.edit()){
                putBoolean(DELETE_MEMBER_ENABLED,enabled)
                apply()
            }
        }
    var stayLoggedIn : Boolean
        get() = sharedPreferences.getBoolean(STAY_LOGGED_IN, false)
        set(enabled)
        {
            with(sharedPreferences.edit()){
                putBoolean(STAY_LOGGED_IN,enabled)
                apply()
            }
        }
}
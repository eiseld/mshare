package elte.moneyshare.entity

import android.os.Parcelable
import elte.moneyshare.SharedPreferences
import kotlinx.android.parcel.Parcelize

@Parcelize
data class RegistrationData (
    val email : String,
    val password : String,
    val displayName : String,
    val lang : String = SharedPreferences.lang
) : Parcelable
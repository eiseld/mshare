package elte.moneyshare.entity

import android.os.Parcelable
import elte.moneyshare.SharedPreferences
import kotlinx.android.parcel.Parcelize

@Parcelize
data class ForgottenPasswordData (
    var email: String,
    val lang : String = SharedPreferences.lang
) : Parcelable
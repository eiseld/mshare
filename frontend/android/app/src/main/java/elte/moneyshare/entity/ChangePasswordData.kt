package elte.moneyshare.entity

import android.os.Parcelable
import elte.moneyshare.SharedPreferences
import kotlinx.android.parcel.Parcelize

@Parcelize
data class ChangePasswordData (
    var oldPassword: String,
    var newPassword: String,
    var id: Int = SharedPreferences.userId,
    val lang : String = SharedPreferences.lang
) : Parcelable
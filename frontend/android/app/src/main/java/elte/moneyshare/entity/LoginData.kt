package elte.moneyshare.entity

import android.os.Parcelable
import kotlinx.android.parcel.Parcelize

@Parcelize
data class LoginData(
    var state: Int,
    var token: String
) : Parcelable
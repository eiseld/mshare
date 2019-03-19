package elte.moneyshare.entity

import android.os.Parcelable
import kotlinx.android.parcel.Parcelize

@Parcelize
data class RegisterData (
    val email : String,
    val password : String,
    val displayname : String
) : Parcelable
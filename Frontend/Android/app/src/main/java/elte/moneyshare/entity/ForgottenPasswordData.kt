package elte.moneyshare.entity

import android.os.Parcelable
import kotlinx.android.parcel.Parcelize

@Parcelize
data class ForgottenPasswordData (
    var email: String
) : Parcelable
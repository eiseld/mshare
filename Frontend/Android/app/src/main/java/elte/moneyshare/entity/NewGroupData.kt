package elte.moneyshare.entity

import android.os.Parcelable
import kotlinx.android.parcel.Parcelize

@Parcelize
data class NewGroupData (
    val name : String
) : Parcelable
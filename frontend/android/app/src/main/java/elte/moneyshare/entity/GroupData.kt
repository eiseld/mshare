package elte.moneyshare.entity

import android.os.Parcel
import android.os.Parcelable

data class GroupData (
    var id: Int,
    var name: String,
    var creator: Member,
    var members: ArrayList<Member>,
    var memberCount: Int,
    var myCurrentBalance: Int
)


data class GroupDataParc(
    var id: Int,
    var name: String
) : Parcelable {
    constructor(parcel: Parcel) : this(
        parcel.readInt(),
        parcel.readString()
    )

    override fun writeToParcel(parcel: Parcel, flags: Int) {
        parcel.writeInt(id)
        parcel.writeString(name)
    }

    override fun describeContents(): Int {
        return 0
    }

    companion object CREATOR : Parcelable.Creator<GroupDataParc> {
        override fun createFromParcel(parcel: Parcel): GroupDataParc {
            return GroupDataParc(parcel)
        }

        override fun newArray(size: Int): Array<GroupDataParc?> {
            return arrayOfNulls(size)
        }
    }
}
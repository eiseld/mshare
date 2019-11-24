package elte.moneyshare.entity

import android.content.Context
import com.google.gson.annotations.SerializedName
import elte.moneyshare.R

data class GroupHistoryEvent(
    val id: String,
    val userId: Int,
    val groupId: Int,
    val affectedIds: List<Int>,
    val date: String,
    val type: HistoryType,
    val subType: HistorySubType,
    val serializedLog: String
)

enum class HistoryType {
    @SerializedName("1")
    UPDATE,
    @SerializedName("2")
    CREATE,
    @SerializedName("3")
    DELETE,
    @SerializedName("4")
    ADD,
    @SerializedName("5")
    REMOVE;

    fun toString(context: Context): String {
        val stringId = when (this) {
            UPDATE -> {
                R.string.history_update
            }
            CREATE -> {
                R.string.history_create
            }
            DELETE -> {
                R.string.history_delete
            }
            ADD -> {
                R.string.history_add
            }
            REMOVE -> {
                R.string.history_remove
            }
        }
        return context.getString(stringId)
    }
}

enum class HistorySubType {
    @SerializedName("1")
    SPENDING,
    @SerializedName("2")
    SETTLEMENT,
    @SerializedName("3")
    DEBT,
    @SerializedName("4")
    GROUP,
    @SerializedName("5")
    MEMBER;

    fun toString(context: Context): String {
        val stringId = when (this) {
            SPENDING -> {
                R.string.spending
            }
            SETTLEMENT -> {
                R.string.history_settlement
            }
            DEBT -> {
                R.string.history_debt
            }
            GROUP -> {
                R.string.history_group
            }
            MEMBER -> {
                R.string.history_member
            }
        }
        return context.getString(stringId)
    }
}
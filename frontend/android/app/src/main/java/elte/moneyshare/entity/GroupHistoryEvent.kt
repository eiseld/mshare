package elte.moneyshare.entity

import android.content.Context
import elte.moneyshare.R
import java.util.*

data class GroupHistoryEvent(
    val id: String,
    val userId: Int,
    val groupId: Int,
    val affectedIds: List<Int>,
    val date: Date,
    val type: HistoryType,
    val subType: HistorySubType,
    val serializedLog: String
)

enum class HistoryType constructor(
    private val mResourceId: Int
) {
    UPDATE(R.string.update),
    CREATE(R.string.create),
    DELETE(R.string.delete),
    ADD(R.string.add),
    REMOVE(R.string.remove);

    fun toString(context: Context): String {
        return context.getString(mResourceId)
    }
}

enum class HistorySubType {
    SPENDING, SETTLEMENT, DEBT, GROUP, MEMBER
}
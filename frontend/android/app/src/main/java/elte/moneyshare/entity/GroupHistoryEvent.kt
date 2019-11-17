package elte.moneyshare.entity

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

enum class HistoryType {
    UPDATE, CREATE, DELETE, ADD, REMOVE
}

enum class HistorySubType {
    SPENDING, SETTLEMENT, DEBT, GROUP, MEMBER
}
package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import android.content.Context
import com.google.gson.Gson
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.*
import elte.moneyshare.model.APIClient
import elte.moneyshare.util.convertToCalendar
import elte.moneyshare.util.formatDate

class GroupViewModel : ViewModel() {

    var groupId: Int = 0
    var isDeleteMemberEnabled: Boolean = false // by Delegates.observable(false, onChange = {})
    var currentGroupData: GroupData? = null

    fun getGroupData(id: Int, completion: (group: GroupData?, error: String?) -> Unit) {
        APIClient.getRepository().getGroupData(id) { groupData, error ->
            if (groupData != null) {
                currentGroupData = groupData
                completion(groupData, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun deleteGroup(groupId: Int, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().deleteGroup(groupId) { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun getMembersToSpending(id: Int, completion: (members: ArrayList<Member>?, error: String?) -> Unit) {
        APIClient.getRepository().getGroupData(id) { groupData, error ->
            if (groupData != null) {
                groupData.members.remove(groupData.members.find { it.id == SharedPreferences.userId })
                for (member in groupData.members) {
                    member.balance = 0
                }
                completion(groupData.members, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun getSpendings(groupId: Int, completion: (response: ArrayList<SpendingData>?, error: String?) -> Unit) {
        APIClient.getRepository().getSpendings(groupId) { spendings, error ->
            if (spendings != null) {
                completion(spendings, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun postSpending(newSpending: NewSpending, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().postSpending(newSpending) { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun deleteSpending(spendingId: Int, groupId: Int, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().deleteSpending(spendingId, groupId) { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun deleteMember(groupId: Int, memberId: Int, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().deleteMember(groupId, memberId) { response, error ->
            if (response != null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun getOptimizedDebtData(groupId: Int, completion: (response: ArrayList<OptimizedDebtData>?, error: String?) -> Unit) {
        APIClient.getRepository().getOptimizedDebt(groupId) { debtData, error ->
            if (debtData != null) {
                completion(debtData, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun postSpendingUpdate(newSpending: SpendingUpdate, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().postSpendingUpdate(newSpending) { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun doDebitEqualization(groupId: Int, ownId: Int, memberId: Int, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().putDebitEqualization(groupId, ownId, memberId) { groupData, error ->
            if (groupData != null) {
                completion(groupData, null)
            } else {
                completion(null, error)
            }
        }
    }

    data class LogAddRemoveSpending(val Name: String, val Money: Int)
    data class LogSettleSpending(val From: Int, val To: Int, val Money: Int)
    data class LogSettleUpdate(
        val oldMoney: Int, val newMoney: Int,
        val oldName: String, val newName: String,
        val oldDate: String, val newDate: String
    )

    fun getGroupHistory(
        context: Context,
        groupId: Int,
        startIndex: Int,
        count: Int, completion: (groupHistory: List<HistoryItem>?, error: String?) -> Unit
    ) {
        APIClient.getRepository().getGroupHistory(groupId, startIndex, count) { groupHistory, error ->
            // @todo: fix history display before removing "false &&"
            if (false && error == null) {
                val historyItems = ArrayList<HistoryItem>()

                val members = currentGroupData?.members?.plus(currentGroupData?.creator)
                groupHistory?.forEach { history ->
                    if (history.subType == HistorySubType.MEMBER && (history.type == HistoryType.ADD || history.type == HistoryType.REMOVE)) {
                        historyItems.add(
                            HistoryItem.AddOrRemoveMember(
                                date = history.date.convertToCalendar().formatDate(),
                                creator = members?.find { it?.id == history.userId }?.name ?: context.getString(R.string.former_member),
                                type = "${history.subType.toString(context)} ${history.type.toString(context)}",
                                memberName = members?.find { it?.id == history.affectedIds.firstOrNull() }?.name
                                    ?: context.getString(R.string.former_member)
                            )
                        )
                    } else if (history.subType == HistorySubType.SPENDING && (history.type == HistoryType.CREATE || history.type == HistoryType.DELETE)) {
                        val log = Gson().fromJson(history.serializedLog, LogAddRemoveSpending::class.java)
                        historyItems.add(
                            HistoryItem.AddOrRemoveSpending(
                                date = history.date.convertToCalendar().formatDate(),
                                creator = members?.find { it?.id == history.userId }?.name
                                    ?: context.getString(R.string.former_member),
                                type = "${history.subType.toString(context)} ${history.type.toString(context)}",
                                spendingName = log.Name,
                                spendingValue = log.Money
                            )
                        )
                    } else if (history.subType == HistorySubType.SPENDING && (history.type == HistoryType.UPDATE)) {
                        val log = Gson().fromJson(history.serializedLog, LogSettleUpdate::class.java)
                        historyItems.add(
                            HistoryItem.ModifySpending(
                                date = history.date.convertToCalendar().formatDate(),
                                creator = members?.find { it?.id == history.userId }?.name ?: context.getString(R.string.former_member),
                                type = "${history.subType.toString(context)} ${history.type.toString(context)}",
                                moneyOld = log.oldMoney,
                                moneyNew = log.newMoney,
                                nameOld = log.oldName,
                                nameNew = log.newName,
                                dateOld = log.oldDate,
                                dateNew = log.newDate
                            )
                        )
                    } else if (history.subType == HistorySubType.SETTLEMENT && (history.type == HistoryType.CREATE)) {
                        val log = Gson().fromJson(history.serializedLog, LogSettleSpending::class.java)
                        historyItems.add(
                            HistoryItem.SettleSpending(
                                date = history.date.convertToCalendar().formatDate(),
                                creator = members?.find { it?.id == history.userId }?.name
                                    ?: context.getString(R.string.former_member),
                                type = "${history.subType.toString(context)} ${history.type.toString(context)}",
                                settleFromName = members?.find { it?.id == log.From }?.name
                                    ?: context.getString(R.string.former_member),
                                settleToName = members?.find { it?.id == log.To }?.name
                                    ?: context.getString(R.string.former_member),
                                settleValue = log.Money
                            )
                        )
                    }
                }
                completion(historyItems, null)
            } else {
                completion(null, error)
            }
        }
    }

    sealed class HistoryItem(open val date: String, open val creator: String, open val type: String) {

        data class AddOrRemoveMember(
            override val date: String,
            override val creator: String,
            override val type: String,
            val memberName: String
        ) : HistoryItem(date, creator, type)

        data class AddOrRemoveSpending(
            override val date: String,
            override val creator: String,
            override val type: String,
            val spendingName: String,
            val spendingValue: Int
        ) : HistoryItem(date, creator, type)

        data class ModifySpending(
            override val date: String,
            override val creator: String,
            override val type: String,
            val moneyOld: Int?,
            val moneyNew: Int?,
            val nameOld: String?,
            val nameNew: String?,
            val dateOld: String?,
            val dateNew: String?
        ) : HistoryItem(date, creator, type)

        data class SettleSpending(
            override val date: String,
            override val creator: String,
            override val type: String,
            val settleFromName: String,
            val settleToName: String,
            val settleValue: Int
        ) : HistoryItem(date, creator, type)
    }
}
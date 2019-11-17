package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.*
import elte.moneyshare.model.APIClient

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
                completion(null , error)
            }
        }
    }

    fun getGroupHistory(
        groupId: Int,
        startIndex: Int,
        count: Int, completion: (groupHistory: List<GroupHistoryEvent>?, error: String?) -> Unit
    ) {
        APIClient.getRepository().getGroupHistory(groupId, startIndex, count) {  groupHistory, error ->
            if (error == null) {
                completion(groupHistory, null)
            } else {
                completion(null, error)
            }
        }
    }
}
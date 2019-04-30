package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.*
import elte.moneyshare.model.APIClient

class GroupsViewModel : ViewModel() {

    var currentGroup: Group? = null
    //var currentBills: Bills? = null

    fun postNewGroup(name: String, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().postNewGroup(NewGroup(name)) { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun getProfileGroups(completion: (groups: ArrayList<GroupInfo>?, error: String?) -> Unit) {
        APIClient.getRepository().getProfileGroups { groupsInfo, error ->
            if (groupsInfo != null) {
                completion(groupsInfo, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun getGroupData(id: Int, completion: (group: GroupData?, error: String?) -> Unit) {
        APIClient.getRepository().getGroupData(id) { groupData, error ->
            if (groupData != null) {
                completion(groupData, null)
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
}
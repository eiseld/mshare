package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.GroupData
import elte.moneyshare.entity.NewSpending
import elte.moneyshare.entity.SpendingData
import elte.moneyshare.model.APIClient
import kotlin.properties.Delegates

class GroupViewModel: ViewModel() {

    var groupId: Int = 0
//    var isDeleteMemberEnabled: Boolean = false // by Delegates.observable(false, onChange = {})
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

    fun deleteMember(groupId: Int, memberId: Int, completion: (response: String?, error: String?) -> Unit)
    {
        APIClient.getRepository().deleteMember(groupId,memberId) { groupData, error ->
            if (groupData != null) {
                completion(groupData, null)
            } else {
                completion(null , error)
            }
        }
    }
}
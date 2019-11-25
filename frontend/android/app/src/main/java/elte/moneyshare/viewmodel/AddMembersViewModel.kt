package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.FilteredUserData
import elte.moneyshare.entity.GroupData
import elte.moneyshare.model.APIClient

class AddMembersViewModel : ViewModel() {

    var currentGroupData: GroupData? = null

    fun postMember(groupId: Int, memberId: Int, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().postMember(groupId, memberId) { response, error ->
            if (response != null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun getSearchedUsers(filter: String, completion: (filteredUsers: ArrayList<FilteredUserData>?, error: String?) -> Unit) {
        APIClient.getRepository().getSearchedUsers(filter) { filteredUsers, error ->
            if (error == null) {
                completion(filteredUsers, null)
            } else {
                completion(null, error)
            }
        }
    }

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

}
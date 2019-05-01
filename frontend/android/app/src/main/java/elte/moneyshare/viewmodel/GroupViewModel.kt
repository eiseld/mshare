package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.GroupData
import elte.moneyshare.model.APIClient

class GroupViewModel: ViewModel() {

    var groupId: Int = 0
    var isDeleteMemberEnabled: Boolean = false
    var groupData: GroupData? = null

    fun getGroupData(id: Int, completion: (group: GroupData?, error: String?) -> Unit) {
        APIClient.getRepository().getGroupData(id) { groupData, error ->
            if (groupData != null) {
                completion(groupData, null)
            } else {
                completion(null, error)
            }
        }
    }
}
package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.*
import elte.moneyshare.model.APIClient

class GroupsViewModel : ViewModel() {

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
}
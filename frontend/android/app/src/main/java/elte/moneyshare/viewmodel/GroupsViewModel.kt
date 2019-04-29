package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.Group
import elte.moneyshare.entity.GroupData
import elte.moneyshare.entity.GroupInfo
import elte.moneyshare.entity.NewGroup
import elte.moneyshare.model.APIClient

class GroupsViewModel :ViewModel(){

    var currentGroup: Group? = null
    //var currentBills: Bills? = null

    fun postNewGroup(name: String, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().postNewGroup(NewGroup(name)) { response, error ->
            if (error == null) {
                //have to update groups after new added
                postUpdateGroups { _, error ->
                    if (error == null)
                        completion(response, null)
                    else {
                        completion(null, error)
                    }
                }
            } else {
                completion(null, error)
            }
        }
    }

    private fun postUpdateGroups(completion: (response: Any?, error: String?) -> Unit) {
        APIClient.getRepository().postUpdateGroups { response, error ->
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
                completion(null , error)
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
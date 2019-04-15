package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.Group
import elte.moneyshare.entity.NewGroup
import elte.moneyshare.model.APIClient

class GroupsViewModel :ViewModel(){

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

    /*//TODO BETTER LOGIC WAIT ASYNC CALLS
    fun getGroups(completion: (response: ArrayList<Group>?, error: String?) -> Unit) {
        APIClient.getRepository().getGroupIds { ids, error ->
            val groups = ArrayList<Group>()
            if (ids != null) {
                for (id in ids) {
                    APIClient.getRepository().getGroup(id) { group, error ->
                        if (group != null) {
                            groups.add(group)
                        } else {
                            Log.d("GroupsViewModel/getGroups", "getGroup error")
                        }
                        if (groups.size == ids.size) {
                            completion(groups, null)
                        }
                    }
                }
            } else {
                completion(null, error)
            }
        }
    }*/

    fun getGroups(completion: (response: ArrayList<Group>?, error: String?) -> Unit) {
        APIClient.getRepository().getGroups { groups, error ->
            if (groups != null) {
                completion(groups, null)
            } else {
                completion(null, error)
            }
        }
    }
}
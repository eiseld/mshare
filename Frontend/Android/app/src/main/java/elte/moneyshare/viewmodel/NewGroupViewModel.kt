package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.model.APIClient

class NewGroupViewModel :ViewModel(){
    fun postNewGroup(name: String, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().postNewGroup(name) { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }
}
package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.RegisterData
import elte.moneyshare.model.APIClient

class RegisterViewModel :ViewModel(){
    fun postRegisterUser(body: RegisterData, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().postRegisterUser(body) { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }
}
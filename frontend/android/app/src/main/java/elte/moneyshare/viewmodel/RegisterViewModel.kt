package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.RegistrationData
import elte.moneyshare.model.APIClient

class RegisterViewModel :ViewModel(){
    fun postRegisterUser(registrationData: RegistrationData, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().postRegisterUser(registrationData) { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }
}
package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.model.APIClient

class LoginViewModel: ViewModel() {

    fun postLoginUser(email: String, password: String, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().postLoginUser(email, password) { response, error ->
            if(error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }
}
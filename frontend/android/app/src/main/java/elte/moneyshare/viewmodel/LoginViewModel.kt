package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.LoginCred
import elte.moneyshare.entity.User
import elte.moneyshare.model.APIClient

class LoginViewModel: ViewModel() {

    fun putLoginUser(email: String, password: String, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().putLoginUser(LoginCred(email, password)) { response, error ->
            if(error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun getUsers(completion: (response: ArrayList<User>?, error: String?) -> Unit) {
        APIClient.getRepository().getUsers { users, error ->
            if (error == null) {
                completion(users, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun updateLang(lang: String, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().updateLang(lang) { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }
}
package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.*
import elte.moneyshare.model.APIClient
import okhttp3.ResponseBody

class ProfileViewModel : ViewModel() {

    fun getProfile(completion: (response: UserData?, error: String?) -> Unit) {
        APIClient.getRepository().getProfile() { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }

    fun updateProfile(
        bankAccountNumberUpdate: BankAccountNumberUpdate,
        completion: (response: String?, error: String?) -> Unit
    ) {
        APIClient.getRepository().updateProfile(bankAccountNumberUpdate) { response, error ->
            if (error == null) {
                completion(response, null)
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

    fun passwordUpdate(passwordUpdate: PasswordUpdate, completion: (response: String?, error: String?) -> Unit) {
        APIClient.getRepository().postPasswordUpdate(passwordUpdate) { response, error ->
            if(error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }
}
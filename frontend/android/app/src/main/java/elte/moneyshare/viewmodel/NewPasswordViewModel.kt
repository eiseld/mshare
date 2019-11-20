package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.PasswordUpdate
import elte.moneyshare.model.APIClient

class NewPasswordViewModel : ViewModel() {

    fun putNewPassword(newPassword: String, token: String, completion: (response: String?, error: String?) -> Unit) {
        SharedPreferences.forgotPasswordEmail?.let {
            val passwordUpdate = PasswordUpdate(
                password = newPassword,
                oldPassword = "",
                token = token,
                email = it
            )
            APIClient.getRepository().postPasswordUpdate(passwordUpdate) { response, error ->
                if (error == null) {
                    completion(response, null)
                } else {
                    completion(null, error)
                }
            }
        }
    }
}
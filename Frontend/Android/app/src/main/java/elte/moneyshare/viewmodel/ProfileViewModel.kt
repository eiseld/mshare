package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
import elte.moneyshare.entity.BankAccountNumberUpdate
import elte.moneyshare.entity.NewGroup
import elte.moneyshare.entity.UserData
import elte.moneyshare.model.APIClient

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
        completion: (response: UserData?, error: String?) -> Unit
    ) {
        APIClient.getRepository().updateProfile(bankAccountNumberUpdate) { response, error ->
            if (error == null) {
                completion(response, null)
            } else {
                completion(null, error)
            }
        }
    }
}
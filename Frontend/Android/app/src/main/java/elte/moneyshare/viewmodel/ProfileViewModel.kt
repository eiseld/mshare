package elte.moneyshare.viewmodel

import android.arch.lifecycle.ViewModel
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

    //TODO IMPL
    fun updateProfile(userData: UserData,  completion: (response: Any , error: String?) -> Unit) {
//        APIClient.getRepository().updateProfile( _ ) { response, error ->
//            if (error == null) {
//                completion(response, null)
//            } else {
//                completion(null, error)
//            }
//        }
    }

}
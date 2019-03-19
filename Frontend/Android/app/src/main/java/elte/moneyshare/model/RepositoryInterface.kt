package elte.moneyshare.model

import elte.moneyshare.entity.RegistrationData
import elte.moneyshare.entity.User

interface RepositoryInterface {

    fun postLoginUser(email: String, password: String, completion: (response: String?, error: String?) -> Unit)

    fun postRegisterUser(registrationData: RegistrationData, completion: (response: String?, error: String?) -> Unit)

    fun postNewGroup(name: String, completion: (response: String?, error: String?) -> Unit)

    fun getUsers(completion: (response: ArrayList<User>?, error: String?) -> Unit)
}
package elte.moneyshare.model

import elte.moneyshare.entity.LoginData
import elte.moneyshare.entity.RegistrationData
import elte.moneyshare.entity.User
import elte.moneyshare.entity.RegisterData

interface RepositoryInterface {

    fun postLoginUser(email: String, password: String, completion: (response: String?, error: String?) -> Unit)
    fun postRegisterUser(body:RegisterData ,completion: (response: String?, error: String?) -> Unit)
    fun postNewGroup(name: String, completion: (response: String?, error: String?) -> Unit)

    fun getUsers(completion: (response: ArrayList<User>?, error: String?) -> Unit)

    fun RegisterUser(registrationData: RegistrationData, completion: (response: String?, error: String?) -> Unit)
}
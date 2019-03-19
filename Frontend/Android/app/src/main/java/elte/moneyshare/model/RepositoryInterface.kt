package elte.moneyshare.model

import elte.moneyshare.entity.RegisterData

interface RepositoryInterface {

    fun postLoginUser(email: String, password: String, completion: (response: String?, error: String?) -> Unit)
    fun postRegisterUser(body:RegisterData ,completion: (response: String?, error: String?) -> Unit)
    fun postNewGroup(name: String, completion: (response: String?, error: String?) -> Unit)
}
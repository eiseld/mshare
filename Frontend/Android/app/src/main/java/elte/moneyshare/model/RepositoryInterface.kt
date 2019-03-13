package elte.moneyshare.model

import elte.moneyshare.entity.LoginData

interface RepositoryInterface {

    fun postLoginUser(email: String, password: String, completion: (response: String?, error: String?) -> Unit)
}
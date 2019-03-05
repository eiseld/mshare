package elte.moneyshare.model

import elte.moneyshare.entity.LoginData
import elte.moneyshare.entity.RegistrationData

interface RepositoryInterface {

    fun putLoginData(userName: String?, password: String?, completion: (loginData: LoginData?, error: String?) -> Unit)

    fun postRegisterUser(registrationData: RegistrationData, completion: (error: String?) -> Unit)

    fun postForgottenPasswordRequest(confirmationEmail: String, completion: (error: String?) -> Unit)
}
package elte.moneyshare.model

import elte.moneyshare.entity.LoginData
import elte.moneyshare.entity.RegistrationData

class Repository(private val apiDefinition: APIDefinition) : RepositoryInterface {


    override fun putLoginData(userName: String?, password: String?, completion: (LoginData?, error: String?) -> Unit) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun postRegisterUser(registrationData: RegistrationData, completion: (error: String?) -> Unit) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }

    override fun postForgottenPasswordRequest(confirmationEmail: String, completion: (error: String?) -> Unit) {
        TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
    }
}
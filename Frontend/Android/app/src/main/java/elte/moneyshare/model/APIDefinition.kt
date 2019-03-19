package elte.moneyshare.model


import elte.moneyshare.entity.LoginData
import elte.moneyshare.entity.RegistrationData
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.Header
import retrofit2.http.POST

interface APIDefinition {

    @POST("/users/login")
    fun postLoginUser(@Header("email") email: String, @Header("password") password: String): Call<LoginData>

    @POST("/users/createUser")
    fun createUser(@Body registrationData: RegistrationData): Call<RegistrationData>

}
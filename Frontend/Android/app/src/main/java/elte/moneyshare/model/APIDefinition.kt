package elte.moneyshare.model


import elte.moneyshare.entity.LoginData
import elte.moneyshare.entity.RegistrationData
import elte.moneyshare.entity.User
import elte.moneyshare.entity.RegisterData
import elte.moneyshare.entity.NewGroupData
import retrofit2.Call
import retrofit2.http.Body
import retrofit2.http.GET
import retrofit2.http.Header
import retrofit2.http.POST

interface APIDefinition {

    @POST("/users/login")
    fun postLoginUser(@Header("email") email: String, @Header("password") password: String): Call<LoginData>

    @POST("/users/createUser")
    fun postRegisterUser(@Body registrationData: RegisterData): Call<RegisterData>

    @POST("/users/createUser")
    fun createUser(@Body registrationData: RegistrationData): Call<RegistrationData>

    @POST("groups/newgroup")
    fun postNewGroup(@Header("name") name : String) : Call<NewGroupData>

    @GET("/users/listUsers")
    fun getUsers(): Call<ArrayList<User>>
}
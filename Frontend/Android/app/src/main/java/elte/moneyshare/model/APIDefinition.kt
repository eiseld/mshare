package elte.moneyshare.model


import elte.moneyshare.entity.*
import retrofit2.Call
import retrofit2.http.*

interface APIDefinition {

    @POST("/users/login")
    fun postLoginUser(@Header("email") email: String, @Header("password") password: String): Call<LoginData>

    @POST("/users/createUser")
    fun postRegisterUser(@Body registrationData: RegistrationData): Call<RegistrationData>

    @POST("groups/newgroup")
    fun postNewGroup(@Query("name") name : String) : Call<NewGroupData>

    @GET("/users/listUsers")
    fun getUsers(): Call<ArrayList<User>>

    @GET("/users/listUsers")
    fun getGroupIds(): Call<ArrayList<String>>

    @GET("/groups/{groupId}")
    fun getGroup(@Path("groupId") groupId: String): Call<Group>

}
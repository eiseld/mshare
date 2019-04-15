package elte.moneyshare.model


import elte.moneyshare.entity.*
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.*

interface APIDefinition {

    //ASP BACKEND
    //AUTH
    @PUT("/api/Auth/login")
    fun putLoginUser(@Body loginCred: LoginCred): Call<LoginResponse>

    @POST("/api//Auth/register")
    fun postRegisterUser(@Body registrationData: RegistrationData): Call<Any>

    //GROUP
    @POST("/api/Group/create")
    fun postNewGroup(@Body groupName : NewGroup) : Call<ResponseBody>

    @GET("/api/Group/")
    fun getGroups(): Call<ArrayList<Group>>

    @GET("/api/Group/{id}")
    fun getGroup(@Path("id") groupId: String): Call<Group>

    @GET("/api/Group/{id}/members/{limit}")
    fun getGroup(@Path("id") groupId: String, @Path("limit") limit: String): Call<Group>

    //OLD
    @POST("users/updategroups")
    fun postUpdateGroups() : Call<Any>

    @GET("/users/listgroups")
    fun getGroupIds(): Call<ArrayList<String>>

    //test method
    @GET("/api/Auth")
    fun getUsers(): Call<ArrayList<User>>
}
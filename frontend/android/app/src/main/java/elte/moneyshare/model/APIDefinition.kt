package elte.moneyshare.model


import elte.moneyshare.entity.*
import retrofit2.Call
import retrofit2.http.*

interface APIDefinition {

    @POST("/api/users/login")
    fun postLoginUser(@Header("email") email: String, @Header("password") password: String): Call<LoginData>

    @POST("/api/users/createUser")
    fun postRegisterUser(@Body registrationData: RegistrationData): Call<RegistrationData>

    @POST("/api/groups/newgroup/{groupName}")
    fun postNewGroup(@Path("groupName") groupName : String) : Call<NewGroupData>

    @POST("/api/users/updategroups")
    fun postUpdateGroups() : Call<Any>

    @GET("/api/users/listUsers")
    fun getUsers(): Call<ArrayList<User>>

    @GET("/api/users/listgroups")
    fun getGroupIds(): Call<ArrayList<String>>

    @GET("/api/groups/{groupId}")
    fun getGroup(@Path("groupId") groupId: String): Call<Group>

}
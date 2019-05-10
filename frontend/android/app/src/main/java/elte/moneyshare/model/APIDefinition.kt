package elte.moneyshare.model


import elte.moneyshare.entity.*
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.*

interface APIDefinition {

    //AUTH
    @PUT("/api/Auth/login")
    fun putLoginUser(@Body loginCred: LoginCred): Call<LoginResponse>

    @POST("/api//Auth/register")
    fun postRegisterUser(@Body registrationData: RegistrationData): Call<Any>

    @GET("api/Profile")
    fun getUserId(): Call<UserData>

    //GROUP
    @GET("/api/Group/{id}")
    fun getGroup(@Path("id") groupId: Int): Call<Group>

    @GET("/api/Group/{groupId}/info")
    fun getGroupInfo(@Path("groupId") groupId: Int): Call<GroupInfo>

    @GET("/api/Group/{groupId}/data")
    fun getGroupData(@Path("groupId") groupId: Int): Call<GroupData>

    @POST("/api/Group/create")
    fun postNewGroup(@Body groupName : NewGroup) : Call<ResponseBody>

    @DELETE ("api/Group/{groupId}/members/remove/{memberId}")
    fun deleteMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    //PROFILE
    @GET("/api/Profile/groups")
    fun getProfileGroups(): Call<ArrayList<GroupInfo>>

    //SPENDING
    @GET("api/Spending/{id}")
    fun getSpendings(@Path("id") groupId: Int): Call<ArrayList<SpendingData>>

    @POST("api/Spending/create")
    fun postSpending(@Body newSpending: NewSpending): Call<Any>

    //TEST METHOD
    @GET("/api/Group/")
    fun getGroups(): Call<ArrayList<Group>>

    @GET("/api/Auth")
    fun getUsers(): Call<ArrayList<User>>

    @DELETE ("api/Group/{groupId}/members/remove/{memberId}")
    fun deleteMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    @GET("api/Profile")
    fun getUserId(): Call<UserData>
}
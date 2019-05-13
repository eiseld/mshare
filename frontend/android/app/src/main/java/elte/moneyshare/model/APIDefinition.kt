package elte.moneyshare.model


import elte.moneyshare.entity.*
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.*

interface APIDefinition {

    //AUTH
    @PUT("test/api/Auth/login")
    fun putLoginUser(@Body loginCred: LoginCred): Call<LoginResponse>

    @POST("test/api//Auth/register")
    fun postRegisterUser(@Body registrationData: RegistrationData): Call<Any>

    @GET("test/api/Profile")
    fun getUserId(): Call<UserData>

    //GROUP
    @GET("test/api/Group/{groupId}/info")
    fun getGroupInfo(@Path("groupId") groupId: Int): Call<GroupInfo>

    @GET("test/api/Group/{groupId}/data")
    fun getGroupData(@Path("groupId") groupId: Int): Call<GroupData>

    @POST("test/api/Group/create")
    fun postNewGroup(@Body groupName : NewGroup) : Call<ResponseBody>

//    @DELETE ("test/api/Group/{groupId}/members/remove/{memberId}")
//    fun deleteMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    @POST ("test/api/Group/{groupId}/members/remove/{memberId}")
    fun deleteMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    @POST ("test/api/Group/{groupId}/members/add/{memberId}")
    fun postMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    @POST("test/api/group/{groupId}/settledebt/{data}/{selectedMember}")
    fun putDebitEqualization(@Path("groupId") groupId: Int,@Path("data") data: Int,@Path("selectedMember") selectedMember: Int): Call<Any>

    @GET("test/api/Group/searchinallusers/{filter}")
    fun getSearchedUsers(@Path("filter") filter: String): Call<ArrayList<FilteredUserData>>

    //PROFILE
    @GET("test/api/Profile/groups")
    fun getProfileGroups(): Call<ArrayList<GroupInfo>>

    @POST("test/api/profile/password/forgot")
    fun postForgotPassword(@Body email: ForgottenPasswordData): Call<String>


    //SPENDING
    @GET("test/api/Spending/{id}")
    fun getSpendings(@Path("id") groupId: Int): Call<ArrayList<SpendingData>>

    @POST("test/api/Spending/create")
    fun postSpending(@Body newSpending: NewSpending): Call<ResponseBody>

    @GET("test/api/Spending/{groupId}/optimised")
    fun getOptimizedDebt(@Path("groupId") groupId: Int) : Call<ArrayList<OptimizedDebtData>>


    //TEST METHOD
    @GET("test/api/Group/")
    fun getGroups(): Call<ArrayList<Group>>

    @GET("test/api/Auth")
    fun getUsers(): Call<ArrayList<User>>
}
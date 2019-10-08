package elte.moneyshare.model


import elte.moneyshare.entity.*
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.*

interface APIDefinition {

    //AUTH
    @PUT("api/Auth/login")
    fun putLoginUser(@Body loginCred: LoginCred): Call<LoginResponse>

    @POST("api//Auth/register")
    fun postRegisterUser(@Body registrationData: RegistrationData): Call<ResponseBody>

    @GET("api/Profile")
    fun getUserId(): Call<UserData>

    //GROUP
    @GET("api/Group/{groupId}/info")
    fun getGroupInfo(@Path("groupId") groupId: Int): Call<GroupInfo>

    @GET("api/Group/{groupId}/data")
    fun getGroupData(@Path("groupId") groupId: Int): Call<GroupData>

    @POST("api/Group/create")
    fun postNewGroup(@Body groupName : NewGroup) : Call<ResponseBody>

//    @DELETE ("api/Group/{groupId}/members/remove/{memberId}")
//    fun deleteMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    @POST ("api/Group/{groupId}/members/remove/{memberId}")
    fun deleteMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    @POST ("api/Group/{groupId}/members/add/{memberId}")
    fun postMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    @POST("api/group/{groupId}/settledebt/{data}/{selectedMember}")
    fun putDebitEqualization(@Path("groupId") groupId: Int,@Path("data") data: Int, @Path("selectedMember") selectedMember: Int): Call<ResponseBody>

    @GET("api/Group/searchinallusers/{filter}")
    fun getSearchedUsers(@Path("filter") filter: String): Call<ArrayList<FilteredUserData>>

    //PROFILE
    @GET("api/Profile/groups")
    fun getProfileGroups(): Call<ArrayList<GroupInfo>>

    @GET("api/Profile")
    fun getProfile(): Call<UserData>

    //TODO IMPL UPDATE
    @POST("api/profile/bankAccountNumber/update")
    fun postBankAccountNumber(@Body bankAccountNumber: BankAccountNumberUpdate): Call<UserData>

    @POST("api/profile/password/forgot")
    fun postForgotPassword(@Body email: ForgottenPasswordData): Call<ResponseBody>

    @PUT("api/Profile/lang")
    fun updateLang(@Body lang: String): Call<ResponseBody>

    //SPENDING
    @GET("api/Spending/{id}")
    fun getSpendings(@Path("id") groupId: Int): Call<ArrayList<SpendingData>>

    @POST("api/Spending/create")
    fun postSpending(@Body newSpending: NewSpending): Call<ResponseBody>

    @GET("api/Spending/{groupId}/optimised")
    fun getOptimizedDebt(@Path("groupId") groupId: Int) : Call<ArrayList<OptimizedDebtData>>

    @POST("api/Spending/update/")
    fun postSpendingUpdate(@Body updatedSpending : SpendingUpdate) : Call<ResponseBody>

    //TEST METHOD
    @GET("api/Group/")
    fun getGroups(): Call<ArrayList<Group>>

    @GET("api/Auth")
    fun getUsers(): Call<ArrayList<User>>
}
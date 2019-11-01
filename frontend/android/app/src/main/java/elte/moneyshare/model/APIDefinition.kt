package elte.moneyshare.model


import elte.moneyshare.entity.*
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.*

interface APIDefinition {

    //AUTH
    @PUT("Auth/login")
    fun putLoginUser(@Body loginCred: LoginCred): Call<LoginResponse>

    @POST("Auth/validate/{token}")
    fun postValidateRegistration(@Path("token") token: String): Call<ResponseBody>

    @POST("Auth/register")
    fun postRegisterUser(@Body registrationData: RegistrationData): Call<ResponseBody>

    @GET("Profile")
    fun getUserId(): Call<UserData>

    //GROUP
    @GET("Group/{groupId}/info")
    fun getGroupInfo(@Path("groupId") groupId: Int): Call<GroupInfo>

    @GET("Group/{groupId}/data")
    fun getGroupData(@Path("groupId") groupId: Int): Call<GroupData>

    @POST("Group/create")
    fun postNewGroup(@Body groupName : NewGroup) : Call<ResponseBody>

//    @DELETE ("Group/{groupId}/members/remove/{memberId}")
//    fun deleteMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    @POST ("Group/{groupId}/members/remove/{memberId}")
    fun deleteMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    @POST ("Group/{groupId}/members/add/{memberId}")
    fun postMember(@Path("groupId") groupId: Int, @Path("memberId") memberId: Int): Call<ResponseBody>

    @POST("group/{groupId}/settledebt/{data}/{selectedMember}")
    fun putDebitEqualization(@Path("groupId") groupId: Int,@Path("data") data: Int, @Path("selectedMember") selectedMember: Int): Call<ResponseBody>

    @GET("Group/searchinallusers/{filter}")
    fun getSearchedUsers(@Path("filter") filter: String): Call<ArrayList<FilteredUserData>>

    //PROFILE
    @GET("Profile/groups")
    fun getProfileGroups(): Call<ArrayList<GroupInfo>>

    @GET("Profile")
    fun getProfile(): Call<UserData>

    @POST("profile/bankAccountNumber/update")
    fun postBankAccountNumber(@Body bankAccountNumber: BankAccountNumberUpdate): Call<ResponseBody>

    @POST("profile/password/forgot")
    fun postForgotPassword(@Body email: ForgottenPasswordData): Call<ResponseBody>

    @POST("profile/password/change")
    fun postChangePassword(@Body changePasswordData: ChangePasswordData): Call<ResponseBody>

    @POST("Profile/password/update")
    fun postPasswordUpdate(@Body passwordUpdate: PasswordUpdate): Call<ResponseBody>

    @PUT("Profile/lang")
    fun updateLang(@Body lang: Lang): Call<ResponseBody>

    //SPENDING
    @GET("Spending/{id}")
    fun getSpendings(@Path("id") groupId: Int): Call<ArrayList<SpendingData>>

    @POST("Spending/create")
    fun postSpending(@Body newSpending: NewSpending): Call<ResponseBody>

    @POST("Spending/{groupId}/delete/{spendingId}")
    fun deleteSpending(@Path("spendingId") spendingId: Int, @Path("groupId") groupId: Int): Call<ResponseBody>

    @GET("Spending/{groupId}/optimised")
    fun getOptimizedDebt(@Path("groupId") groupId: Int) : Call<ArrayList<OptimizedDebtData>>

    @POST("Spending/update/")
    fun postSpendingUpdate(@Body updatedSpending : SpendingUpdate) : Call<ResponseBody>

    //TEST METHOD
    @GET("Group/")
    fun getGroups(): Call<ArrayList<Group>>

    @GET("Auth")
    fun getUsers(): Call<ArrayList<User>>
}
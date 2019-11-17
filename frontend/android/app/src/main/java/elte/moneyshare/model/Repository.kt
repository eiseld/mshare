package elte.moneyshare.model


import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.*
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class Repository(private val apiDefinition: APIDefinition, private val onFailureMessage: String) : RepositoryInterface {

    //region AUTH
    override fun putLoginUser(loginCred: LoginCred, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.putLoginUser(loginCred).enqueue(object : Callback<LoginResponse> {
            override fun onResponse(call: Call<LoginResponse>, response: Response<LoginResponse>) {
                when (response?.code()) {
                    in (200..300) -> {
                        SharedPreferences.isUserLoggedIn = true
                        SharedPreferences.accessToken = response?.body()?.token ?: ""
                        SharedPreferences.email = loginCred.email
                        getUserId({ response, error -> })
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<LoginResponse>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun postValidateRegistration(token: String, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postValidateRegistration(token).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun postRegisterUser(
        registrationData: RegistrationData,
        completion: (response: String?, error: String?) -> Unit
    ) {
        apiDefinition.postRegisterUser(registrationData).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }


    override fun getUserId(completion: (response: UserData?, error: String?) -> Unit) {
        apiDefinition.getUserId().enqueue(object : Callback<UserData> {
            override fun onResponse(call: Call<UserData>, response: Response<UserData>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val user = response.body()
                        SharedPreferences.userId = user?.id ?: 0
                        SharedPreferences.userName = user?.name ?: ""
                        completion(user, null)
                    }
                    else -> {
                        completion(null, onFailureMessage)
                    }
                }
            }

            override fun onFailure(call: Call<UserData>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }


    override fun postForgotPassword(email: String, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postForgotPassword(ForgottenPasswordData(email)).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    //endregion

    //region GROUP
    override fun getGroupInfo(groupId: Int, completion: (response: GroupInfo?, error: String?) -> Unit) {
        apiDefinition.getGroupInfo(groupId).enqueue(object : Callback<GroupInfo> {
            override fun onResponse(call: Call<GroupInfo>, response: Response<GroupInfo>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val groupInfo = response.body()
                        completion(groupInfo, null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<GroupInfo>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun getGroupData(groupId: Int, completion: (response: GroupData?, error: String?) -> Unit) {
        apiDefinition.getGroupData(groupId).enqueue(object : Callback<GroupData> {
            override fun onResponse(call: Call<GroupData>, response: Response<GroupData>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val groupData = response.body()
                        completion(groupData, null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<GroupData>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun postNewGroup(name: NewGroup, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postNewGroup(name).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun deleteGroup(id: Int, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.deleteGroup(id).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun deleteMember(groupId: Int, memberId: Int, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.deleteMember(groupId, memberId).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun postMember(groupId: Int, memberId: Int, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postMember(groupId, memberId).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun getGroupHistory(
        groupId: String,
        startIndex: String,
        count: String,
        completion: (response: List<GroupHistoryEvent>?, error: String?) -> Unit
    ) {
        apiDefinition.getGroupHistory(groupId, startIndex, count).enqueue(object : Callback<List<GroupHistoryEvent>> {
            override fun onResponse(call: Call<List<GroupHistoryEvent>>, response: Response<List<GroupHistoryEvent>>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.body(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<List<GroupHistoryEvent>>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    //endregion

    //region PROFILE
    override fun getProfileGroups(completion: (response: ArrayList<GroupInfo>?, error: String?) -> Unit) {
        apiDefinition.getProfileGroups().enqueue(object : Callback<ArrayList<GroupInfo>> {
            override fun onResponse(call: Call<ArrayList<GroupInfo>>, response: Response<ArrayList<GroupInfo>>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val groupsInfo = response.body()
                        completion(groupsInfo, null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ArrayList<GroupInfo>>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun getProfile(completion: (response: UserData?, error: String?) -> Unit) {
        apiDefinition.getProfile().enqueue(object : Callback<UserData> {
            override fun onResponse(call: Call<UserData>, response: Response<UserData>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val userData = response.body()
                        completion(userData, null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<UserData>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun updateProfile(
        bankAccountNumberUpdate: BankAccountNumberUpdate,
        completion: (response: String?, error: String?) -> Unit
    ) {
        apiDefinition.postBankAccountNumber(bankAccountNumberUpdate).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun postPasswordUpdate(
        passwordUpdate: PasswordUpdate,
        completion: (response: String?, error: String?) -> Unit
    ) {
        apiDefinition.postPasswordUpdate(passwordUpdate).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun updateLang(lang: String, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.updateLang(Lang(lang)).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }
    //endregion

    //region SPENDING
    override fun getSpendings(groupId: Int, completion: (response: ArrayList<SpendingData>?, error: String?) -> Unit) {
        apiDefinition.getSpendings(groupId).enqueue(object : Callback<ArrayList<SpendingData>> {
            override fun onResponse(call: Call<ArrayList<SpendingData>>, response: Response<ArrayList<SpendingData>>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val spendings = response.body()
                        completion(spendings, null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ArrayList<SpendingData>>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun postSpending(newSpending: NewSpending, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postSpending(newSpending).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun deleteSpending(
        spendingId: Int,
        groupId: Int,
        completion: (response: String?, error: String?) -> Unit
    ) {
        apiDefinition.deleteSpending(spendingId, groupId).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun postSpendingUpdate(
        spendingUpdate: SpendingUpdate,
        completion: (response: String?, error: String?) -> Unit
    ) {
        apiDefinition.postSpendingUpdate(spendingUpdate).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun getOptimizedDebt(
        groupId: Int,
        completion: (response: ArrayList<OptimizedDebtData>?, error: String?) -> Unit
    ) {
        apiDefinition.getOptimizedDebt(groupId).enqueue(object : Callback<ArrayList<OptimizedDebtData>> {
            override fun onResponse(call: Call<ArrayList<OptimizedDebtData>>, response: Response<ArrayList<OptimizedDebtData>>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val debtData = response.body()
                        completion(debtData, null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ArrayList<OptimizedDebtData>>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun putDebitEqualization(
        groupId: Int,
        ownId: Int,
        selectedMember: Int,
        completion: (response: String?, error: String?) -> Unit
    ) {
        apiDefinition.putDebitEqualization(groupId, ownId, selectedMember).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun getSearchedUsers(
        filter: String,
        completion: (response: ArrayList<FilteredUserData>?, error: String?) -> Unit
    ) {
        apiDefinition.getSearchedUsers(filter).enqueue(object : Callback<ArrayList<FilteredUserData>> {
            override fun onResponse(
                call: Call<ArrayList<FilteredUserData>>,
                response: Response<ArrayList<FilteredUserData>>
            ) {
                when (response?.code()) {
                    in (200..300) -> {
                        val filteredUsers = response.body()
                        completion(filteredUsers, null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ArrayList<FilteredUserData>>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }
    //endregion

    //region TEST METHODS
    override fun getGroups(completion: (response: ArrayList<Group>?, error: String?) -> Unit) {
        apiDefinition.getGroups().enqueue(object : Callback<ArrayList<Group>> {
            override fun onResponse(call: Call<ArrayList<Group>>, response: Response<ArrayList<Group>>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val groups = response.body()
                        completion(groups, null)
                    }
                    else -> {
                        completion(null, onFailureMessage)
                    }
                }
            }

            override fun onFailure(call: Call<ArrayList<Group>>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    override fun getUsers(completion: (response: ArrayList<User>?, error: String?) -> Unit) {
        apiDefinition.getUsers().enqueue(object : Callback<ArrayList<User>> {
            override fun onResponse(call: Call<ArrayList<User>>, response: Response<ArrayList<User>>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val users = response.body()
                        completion(users, null)
                    }
                    else -> {
                        completion(null, response.code().toString())
                    }
                }
            }

            override fun onFailure(call: Call<ArrayList<User>>, t: Throwable) {
                completion(null, onFailureMessage)
            }
        })
    }

    //endregion
}

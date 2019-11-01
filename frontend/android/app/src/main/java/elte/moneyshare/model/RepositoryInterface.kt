package elte.moneyshare.model

import elte.moneyshare.entity.*
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.http.Body

interface RepositoryInterface {

    //AUTH
    fun putLoginUser(loginCred: LoginCred, completion: (response: String?, error: String?) -> Unit)

    fun postForgotPassword(email: String, completion: (response: String?, error: String?) -> Unit)

    fun postRegisterUser(registrationData: RegistrationData, completion: (response: String?, error: String?) -> Unit)

    fun getUserId(completion: (response: UserData?, error: String?) -> Unit)


    //GROUP
    fun getGroupInfo(groupId: Int, completion: (response: GroupInfo?, error: String?) -> Unit)

    fun getGroupData(groupId: Int, completion: (response: GroupData?, error: String?) -> Unit)

    fun postNewGroup(name: NewGroup, completion: (response: String?, error: String?) -> Unit)

    fun deleteMember(groupId: Int, memberId : Int, completion: (response: String?, error: String?) -> Unit)

    fun postMember(groupId: Int, memberId : Int, completion: (response: String?, error: String?) -> Unit)

    fun putDebitEqualization(groupId: Int, ownId: Int,selectedMember: Int, completion: (response: String?, error: String?) -> Unit)

    fun getSearchedUsers(filter: String, completion: (response: ArrayList<FilteredUserData>?, error: String?) -> Unit)

    //PROFILE
    fun getProfileGroups(completion: (response: ArrayList<GroupInfo>?, error: String?) -> Unit)

    fun getProfile(completion: (response: UserData?, error: String?) -> Unit)

    fun updateProfile(bankAccountNumberUpdate: BankAccountNumberUpdate, completion: (response: String?, error: String?) -> Unit)

    fun updateLang(lang: String, completion: (response: String?, error: String?) -> Unit)

    //SPENDING
    fun getSpendings(groupId: Int, completion: (response: ArrayList<SpendingData>?, error: String?) -> Unit)

    fun postSpending(newSpending: NewSpending, completion: (response: String?, error: String?) -> Unit)

    fun deleteSpending(spendingId: Int, groupId: Int, completion: (response: String?, error: String?) -> Unit)

    fun getOptimizedDebt(groupId: Int, completion: (response: ArrayList<OptimizedDebtData>?, error: String?) -> Unit)

    fun postSpendingUpdate(spendingUpdate : SpendingUpdate, completion: (response: String?, error: String?) -> Unit)

    //TEST METHOD
    fun getUsers(completion: (response: ArrayList<User>?, error: String?) -> Unit)
  
    fun getGroups(completion: (response: ArrayList<Group>?, error: String?) -> Unit)
}

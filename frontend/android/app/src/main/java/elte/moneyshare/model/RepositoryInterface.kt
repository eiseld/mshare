package elte.moneyshare.model

import elte.moneyshare.entity.*

interface RepositoryInterface {

    //AUTH
    fun putLoginUser(LoginCred: LoginCred, completion: (response: String?, error: String?) -> Unit)

    fun postRegisterUser(registrationData: RegistrationData, completion: (response: String?, error: String?) -> Unit)


    //GROUP
    fun getGroup(groupId: Int, completion: (response: Group?, error: String?) -> Unit)

    fun getGroupInfo(groupId: Int, completion: (response: GroupInfo?, error: String?) -> Unit)

    fun getGroupData(groupId: Int, completion: (response: GroupData?, error: String?) -> Unit)

    fun postNewGroup(name: NewGroup, completion: (response: String?, error: String?) -> Unit)


    //PROFILE
    fun getProfileGroups(completion: (response: ArrayList<GroupInfo>?, error: String?) -> Unit)


    //SPENDING
    fun getSpendings(groupId: Int, completion: (response: ArrayList<SpendingData>?, error: String?) -> Unit)

    fun postSpending(newSpending: NewSpending, completion: (response: String?, error: String?) -> Unit)

    //TEST METHOD
    fun getUsers(completion: (response: ArrayList<User>?, error: String?) -> Unit)

    fun getGroups(completion: (response: ArrayList<Group>?, error: String?) -> Unit)
}
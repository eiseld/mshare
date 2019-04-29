package elte.moneyshare.model

import elte.moneyshare.entity.*

interface RepositoryInterface {

    fun putLoginUser(LoginCred: LoginCred, completion: (response: String?, error: String?) -> Unit)

    fun postRegisterUser(registrationData: RegistrationData, completion: (response: String?, error: String?) -> Unit)

    fun postNewGroup(name: NewGroup, completion: (response: String?, error: String?) -> Unit)

    fun getUsers(completion: (response: ArrayList<User>?, error: String?) -> Unit)

    fun getGroupIds(completion: (response: ArrayList<String>?, error: String?) -> Unit)

    fun getGroups(completion: (response: ArrayList<Group>?, error: String?) -> Unit)

    fun getProfileGroups(completion: (response: ArrayList<GroupInfo>?, error: String?) -> Unit)

    fun getGroup(groupId: Int, completion: (response: Group?, error: String?) -> Unit)

    fun getGroupInfo(groupId: Int, completion: (response: GroupInfo?, error: String?) -> Unit)

    fun getGroupData(groupId: Int, completion: (response: GroupData?, error: String?) -> Unit)

    fun postUpdateGroups(completion: (response: Any?, error: String?) -> Unit)

    fun deleteMember(groupId: Int,memberId : Int, completion: (response: String?, error: String?) -> Unit)
}
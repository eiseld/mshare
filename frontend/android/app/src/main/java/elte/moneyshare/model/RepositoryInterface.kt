package elte.moneyshare.model

import elte.moneyshare.entity.Group
import elte.moneyshare.entity.RegistrationData
import elte.moneyshare.entity.User

interface RepositoryInterface {

    fun postLoginUser(email: String, password: String, completion: (response: String?, error: String?) -> Unit)

    fun postRegisterUser(registrationData: RegistrationData, completion: (response: String?, error: String?) -> Unit)

    fun postNewGroup(name: String, completion: (response: String?, error: String?) -> Unit)

    fun getUsers(completion: (response: ArrayList<User>?, error: String?) -> Unit)

    fun getGroupIds(completion: (response: ArrayList<String>?, error: String?) -> Unit)

    fun getGroup(groupId: String, completion: (response: Group?, error: String?) -> Unit)

    fun postUpdateGroups(completion: (response: Any?, error: String?) -> Unit)
}
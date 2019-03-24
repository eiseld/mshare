package elte.moneyshare.model


import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.*
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class Repository(private val apiDefinition: APIDefinition) : RepositoryInterface {

    override fun postLoginUser(email: String, password: String, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postLoginUser(email, password).enqueue(object : Callback<LoginData> {
            override fun onResponse(call: Call<LoginData>, response: Response<LoginData>) {
                when (response?.code()) {
                    in (200..300) -> {
                        SharedPreferences.isUserLoggedIn = true
                        SharedPreferences.accessToken = response?.body()?.token ?: ""
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, "login error")
                    }
                }
            }

            override fun onFailure(call: Call<LoginData>, t: Throwable) {
                completion(null, "login error")
            }
        })
    }

    override fun postRegisterUser(registrationData: RegistrationData, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postRegisterUser(registrationData).enqueue(object : Callback<RegistrationData> {
            override fun onResponse(call: Call<RegistrationData>, response: Response<RegistrationData>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, "register error")
                    }
                }
            }

            override fun onFailure(call: Call<RegistrationData>, t: Throwable) {
                completion(null, "registration error")
            }
        })
    }

    override fun postNewGroup(name: String , completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postNewGroup(name).enqueue(object : Callback<NewGroupData> {
            override fun onResponse(call: Call<NewGroupData>, response: Response<NewGroupData>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, "group creation error")
                    }
                }
            }

            override fun onFailure(call: Call<NewGroupData>, t: Throwable) {
                completion(null, "group creation error")
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
                        completion(null, "get users error")
                    }
                }
            }

            override fun onFailure(call: Call<ArrayList<User>>, t: Throwable) {
                completion(null, "get users error")
            }
        })
    }

    override fun getGroupIds(completion: (response: ArrayList<String>?, error: String?) -> Unit) {
        apiDefinition.getGroupIds().enqueue(object : Callback<ArrayList<String>> {
            override fun onResponse(call: Call<ArrayList<String>>, response: Response<ArrayList<String>>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val groupIds = response.body()
                        completion(groupIds, null)
                    }
                    else -> {
                        completion(null, "get groupIds error")
                    }
                }
            }

            override fun onFailure(call: Call<ArrayList<String>>, t: Throwable) {
                completion(null, "get groupIds error")
            }
        })
    }

    override fun getGroup(groupId: String, completion: (response: Group?, error: String?) -> Unit) {
        apiDefinition.getGroup(groupId).enqueue(object : Callback<Group> {
            override fun onResponse(call: Call<Group>, response: Response<Group>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val group = response.body()
                        completion(group, null)
                    }
                    else -> {
                        completion(null, "get group error")
                    }
                }
            }

            override fun onFailure(call: Call<Group>, t: Throwable) {
                completion(null, "get group error")
            }
        })
    }
}
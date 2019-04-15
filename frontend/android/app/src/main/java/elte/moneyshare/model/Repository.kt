package elte.moneyshare.model


import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.*
import okhttp3.ResponseBody
import retrofit2.Call
import retrofit2.Callback
import retrofit2.Response

class Repository(private val apiDefinition: APIDefinition) : RepositoryInterface {

    //AUTH
    override fun putLoginUser(loginCred: LoginCred, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.putLoginUser(loginCred).enqueue(object : Callback<LoginResponse> {
            override fun onResponse(call: Call<LoginResponse>, response: Response<LoginResponse>) {
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

            override fun onFailure(call: Call<LoginResponse>, t: Throwable) {
                completion(null, "login error")
            }
        })
    }

    override fun postRegisterUser(registrationData: RegistrationData, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postRegisterUser(registrationData).enqueue(object : Callback<Any> {
            override fun onResponse(call: Call<Any>, response: Response<Any>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, "register error")
                    }
                }
            }

            override fun onFailure(call: Call<Any>, t: Throwable) {
                completion(null, "registration error")
            }
        })
    }

    //GROUP
    override fun postNewGroup(name: NewGroup , completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postNewGroup(name).enqueue(object : Callback<ResponseBody> {
            override fun onResponse(call: Call<ResponseBody>, response: Response<ResponseBody>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, "group creation error")
                    }
                }
            }

            override fun onFailure(call: Call<ResponseBody>, t: Throwable) {
                completion(null, "group creation error")
            }
        })
    }

    override fun postUpdateGroups(completion: (response: Any?, error: String?) -> Unit) {
        apiDefinition.postUpdateGroups().enqueue(object : Callback<Any> {
            override fun onResponse(call: Call<Any>, response: Response<Any>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.body(), null)
                    }
                    else -> {
                        completion(null, "update groups error")
                    }
                }
            }

            override fun onFailure(call: Call<Any>, t: Throwable) {
                completion(null, "update groups error")
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

    override fun getGroups(completion: (response: ArrayList<Group>?, error: String?) -> Unit) {
        apiDefinition.getGroups().enqueue(object : Callback<ArrayList<Group>> {
            override fun onResponse(call: Call<ArrayList<Group>>, response: Response<ArrayList<Group>>) {
                when (response?.code()) {
                    in (200..300) -> {
                        val groups = response.body()
                        completion(groups, null)
                    }
                    else -> {
                        completion(null, "get group error")
                    }
                }
            }

            override fun onFailure(call: Call<ArrayList<Group>>, t: Throwable) {
                completion(null, "get group error")
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
}
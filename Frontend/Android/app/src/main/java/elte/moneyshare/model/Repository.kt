package elte.moneyshare.model


import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.LoginData
import elte.moneyshare.entity.RegistrationData
import elte.moneyshare.entity.User
import elte.moneyshare.entity.NewGroupData
import elte.moneyshare.entity.RegisterData
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
                        SharedPreferences.accessToken = response?.body()?.accessToken ?: ""
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

    override fun RegisterUser(registrationData: RegistrationData, completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.createUser(registrationData).enqueue(object : Callback<RegistrationData> {
            override fun onResponse(call: Call<RegistrationData>, response: Response<RegistrationData>) {
                TODO("not implemented") //To change body of created functions use File | Settings | File Templates.
            }

            override fun onFailure(call: Call<RegistrationData>, t: Throwable) {
                completion(null, "registration error")
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

    override fun postRegisterUser(body: RegisterData , completion: (response: String?, error: String?) -> Unit) {
        apiDefinition.postRegisterUser(body).enqueue(object : Callback<RegisterData> {
            override fun onResponse(call: Call<RegisterData>, response: Response<RegisterData>) {
                when (response?.code()) {
                    in (200..300) -> {
                        completion(response.code().toString(), null)
                    }
                    else -> {
                        completion(null, "register error")
                    }
                }
            }

            override fun onFailure(call: Call<RegisterData>, t: Throwable) {
                completion(null, "register error")
            }
        })
    }
    override fun postNewGroup(name : String , completion: (response: String?, error: String?) -> Unit) {
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
}
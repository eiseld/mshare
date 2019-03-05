package elte.moneyshare.model

import android.content.Context
import elte.moneyshare.R
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object APIClient {

    private lateinit var baseUrlValue: String
    private lateinit var apiKeyValue: String
    private lateinit var retrofit: Retrofit
    private lateinit var apiDefinition: APIDefinition
    private lateinit var repository: Repository

    fun init(context: Context) {
        baseUrlValue = context.getString(R.string.base_url_value)

        val interceptor = HttpLoggingInterceptor()
        interceptor.level = HttpLoggingInterceptor.Level.BODY
        val client = OkHttpClient.Builder().addInterceptor(interceptor).build()


        retrofit = Retrofit.Builder()
            .baseUrl(baseUrlValue)
            .addConverterFactory(GsonConverterFactory.create())
            .client(client)
            .build()


        apiDefinition = retrofit.create(APIDefinition::class.java)
        repository = Repository(apiDefinition)
    }

    fun getRepository(): Repository{
        return repository
    }
}
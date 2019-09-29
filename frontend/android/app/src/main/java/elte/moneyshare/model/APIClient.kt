package elte.moneyshare.model

import android.content.Context
import com.google.gson.GsonBuilder
import elte.moneyshare.BuildConfig
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import okhttp3.Headers
import okhttp3.Interceptor
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.converter.gson.GsonConverterFactory

object APIClient {

    private lateinit var baseUrlValue: String
    private lateinit var accessTokenKey: String
    private lateinit var retrofit: Retrofit
    private lateinit var apiDefinition: APIDefinition
    private lateinit var repository: Repository

    fun init(context: Context) {
        baseUrlValue = BuildConfig.BASE_URL
        accessTokenKey = context.getString(R.string.access_token_key)
        val onFailureMessage = context.resources.getString(R.string.on_failure_message)

        val interceptorForLogging = HttpLoggingInterceptor()
        interceptorForLogging.level = HttpLoggingInterceptor.Level.BODY

        val interceptorForHeaders = createHeadersInterceptor()

        val client = OkHttpClient.Builder()
            .addInterceptor(interceptorForHeaders)
            .addInterceptor(interceptorForLogging)
            .build()

        val gson = GsonBuilder().setLenient().create()

        retrofit = Retrofit.Builder()
            .baseUrl(baseUrlValue)
            .client(client)
            .addConverterFactory(GsonConverterFactory.create(gson))
            .build()

        apiDefinition = retrofit.create(APIDefinition::class.java)
        repository = Repository(apiDefinition, onFailureMessage)
    }

    fun getRepository(): Repository{
        return repository
    }

    private val headersForRetrofit: Headers
        get() = if (SharedPreferences.accessToken.isNotEmpty()) {
            accessTokenHeaders
        } else {
            apiSecretHeaders
        }

    /*private fun addTokenToHeader(originalHeaders: Headers){
        originalHeaders.names()
        Headers.Builder()
            .add(originalHeaders)
            .add(accessTokenKey, SharedPreferences.token)
            .build()

    }*/

    private val accessTokenHeaders: Headers
        get() = Headers.Builder()
            .add(accessTokenKey, SharedPreferences.accessToken)
            .add("Content-Type", "application/json")
            .build()

    private val apiSecretHeaders: Headers
        get() = Headers.Builder()
            .add("Content-Type", "application/json")
            .build()

    private fun createHeadersInterceptor(): Interceptor {
        return Interceptor { chain ->
            //interceptor for headers
            if (SharedPreferences.accessToken.isNotEmpty()) {
                val requestBuilder = chain.request().newBuilder()
                requestBuilder.headers(headersForRetrofit)
                    .method(chain.request().method(), chain.request().body())
                chain.proceed(requestBuilder.build())
            } else {
                chain.proceed(chain.request())
            }
        }
    }
}
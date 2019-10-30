package elte.moneyshare

import android.app.Application
import elte.moneyshare.model.APIClient

class MoneyShareApplication: Application() {

    override fun onCreate() {
        super.onCreate()
        APIClient.init(baseUrl = null, context = applicationContext){}
        SharedPreferences.init(applicationContext)
    }
}
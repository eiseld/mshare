package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.Toolbar
import android.widget.Toast
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.model.APIClient
import elte.moneyshare.util.showAsDialog
import elte.moneyshare.util.showToast
import elte.moneyshare.viewmodel.LoginViewModel
import java.util.*


class LoginActivity : AppCompatActivity() {

    private var isBackPressedOnce = false
    private lateinit var loginViewModel: LoginViewModel
    private val CONFIRM_REGISTRATION = "confirmregistration"
    private val FORGOT_PASSWORD = "forgotpassword"

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        updateLang(SharedPreferences.lang)

        setContentView(R.layout.activity_login)
        val toolbar: Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(toolbar)

        supportFragmentManager.beginTransaction().replace(R.id.frame_container, LoginFragment())
            .commit()

        loginViewModel = ViewModelProviders.of(this).get(LoginViewModel::class.java)

        handleDeepLinkIntent()
    }

    private fun handleDeepLinkIntent() {
        val data = this.intent.data
        if (data != null && data.isHierarchical) {
            val intentData = data.encodedSchemeSpecificPart.split(regex = Regex("/"), limit = 4)
            val intentFrom = intentData[2]
            val sentToken = intentData[3]
            when (intentFrom) {
                CONFIRM_REGISTRATION -> {
                    loginViewModel.postValidateRegistration(sentToken) { _, error ->
                        if (error == null) {
                            DialogManager.showInfoDialog(getString(R.string.successful_registration_confirmation), baseContext)
                        } else {
                            error.showAsDialog(this)
                        }
                    }
                }
                FORGOT_PASSWORD -> {
                    val fragment = NewPasswordFragment()
                    val args = Bundle()
                    args.putString(FragmentDataKeys.NEW_PASSWORD_TOKEN.value, sentToken)
                    fragment.arguments = args
                    supportFragmentManager.beginTransaction().replace(R.id.frame_container, fragment).addToBackStack(null).commit()
                }
            }
        }
    }

    fun updateUrl(baseUrl: String) {
        APIClient.init(baseUrl = "http://$baseUrl:8081/", context = applicationContext) { setUrl ->
            setUrl.showToast(this)
        }
    }

    fun updateLang(lang: String) {
        val dm = resources.displayMetrics
        val conf = resources.configuration
        conf.setLocale(Locale(lang))
        resources.updateConfiguration(conf, dm)
    }

    override fun onBackPressed() {
        if (supportFragmentManager?.backStackEntryCount == 0 && isBackPressedOnce) {
            super.onBackPressed()
            return
        } else if (supportFragmentManager?.backStackEntryCount == 0 && !isBackPressedOnce) {

            isBackPressedOnce = true
            Toast.makeText(this, getString(R.string.press_back_again_to_exit), Toast.LENGTH_SHORT)
                .show()
            Handler().postDelayed({
                isBackPressedOnce = false
            }, 2000)
        } else {
            super.onBackPressed()
        }
    }

    fun refresh() {
        finish()
        val refresh = Intent(this, LoginActivity::class.java)
        startActivity(refresh)
    }
}

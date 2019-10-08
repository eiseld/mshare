package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.os.Handler
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.Toolbar
import android.widget.Toast
import elte.moneyshare.R
import elte.moneyshare.viewmodel.GroupsViewModel

class LoginActivity : AppCompatActivity() {

    private var isBackPressedOnce = false
    private lateinit var groupsViewModel: GroupsViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.activity_login)
        val toolbar: Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(toolbar)

        supportFragmentManager.beginTransaction().replace(R.id.frame_container, LoginFragment())
            .commit()

        groupsViewModel = ViewModelProviders.of(this).get(GroupsViewModel::class.java)
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
}

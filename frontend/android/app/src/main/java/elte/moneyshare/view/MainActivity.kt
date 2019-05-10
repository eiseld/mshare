package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import elte.moneyshare.R
import elte.moneyshare.viewmodel.GroupsViewModel
import android.widget.Toast
import android.os.Handler

class MainActivity : AppCompatActivity() {

    private var isBackPressedOnce = false
    private lateinit var groupsViewModel: GroupsViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        supportActionBar?.setDisplayShowHomeEnabled(true)
        supportActionBar?.setLogo(R.mipmap.ic_launcher)
        supportActionBar?.setDisplayUseLogoEnabled(true)
        setContentView(R.layout.activity_main)

        supportFragmentManager.beginTransaction().replace(R.id.frame_container, LoginFragment()).commit()

        groupsViewModel = ViewModelProviders.of(this).get(GroupsViewModel::class.java)
    }

    override fun onBackPressed() {
        if (supportFragmentManager?.backStackEntryCount == 0 && isBackPressedOnce) {
            super.onBackPressed()
            return
        } else if (supportFragmentManager?.backStackEntryCount == 0 && !isBackPressedOnce) {

            isBackPressedOnce = true
            Toast.makeText(this, "Please click BACK again to exit", Toast.LENGTH_SHORT).show()
            Handler().postDelayed({
                isBackPressedOnce = false
            }, 2000)
        } else {
            super.onBackPressed()
        }
    }
}

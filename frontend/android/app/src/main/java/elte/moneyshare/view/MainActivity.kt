package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.os.Handler
import android.support.design.widget.NavigationView
import android.support.v4.view.GravityCompat
import android.support.v4.widget.DrawerLayout
import android.support.v7.app.ActionBarDrawerToggle
import android.support.v7.app.AppCompatActivity
import android.support.v7.widget.Toolbar
import android.view.MenuItem
import android.view.View
import android.widget.Toast
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.viewmodel.GroupsViewModel
import kotlinx.android.synthetic.main.activity_main.*
import kotlinx.android.synthetic.main.nav_header_main.view.*

class MainActivity : AppCompatActivity(), NavigationView.OnNavigationItemSelectedListener {

    private var isBackPressedOnce = false
    private lateinit var groupsViewModel: GroupsViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)

        setContentView(R.layout.activity_main)
        val toolbar: Toolbar = findViewById(R.id.toolbar)
        setSupportActionBar(toolbar)

        val drawerLayout: DrawerLayout = findViewById(R.id.drawerLayout)
        val navView: NavigationView = findViewById(R.id.navView)
        val toggle = ActionBarDrawerToggle(
            this, drawerLayout, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close
        )
        drawerLayout.addDrawerListener(toggle)
        toggle.syncState()

        //TODO FIND BETTER SOLUTION TO REFRESH HEADER TEXTS
        val drawerStateChangedListener = (object : DrawerLayout.DrawerListener{
            override fun onDrawerStateChanged(newState: Int) {
                if (newState == DrawerLayout.STATE_SETTLING) {
                    navView.loggedInUserNameTextView?.text = SharedPreferences.userName
                    navView.loggedInUserEmailTextView?.text = SharedPreferences.email
                    invalidateOptionsMenu()

                    //drawerLayout.removeDrawerListener(this)
                }
            }

            override fun onDrawerSlide(drawerView: View, slideOffset: Float) {
            }

            override fun onDrawerClosed(drawerView: View) {
            }

            override fun onDrawerOpened(drawerView: View) {
            }
        })

        drawerLayout.addDrawerListener(drawerStateChangedListener)
        navView.setNavigationItemSelectedListener(this)

        //supportActionBar?.setDisplayShowHomeEnabled(true)
        //supportActionBar?.setLogo(R.mipmap.ic_launcher)
        //supportActionBar?.setDisplayUseLogoEnabled(true)

        supportFragmentManager.beginTransaction().replace(R.id.frame_container, LoginFragment()).commit()

        groupsViewModel = ViewModelProviders.of(this).get(GroupsViewModel::class.java)
    }

    override fun onNavigationItemSelected(item: MenuItem): Boolean {
        // Handle navigation view item clicks here.
        when (item.itemId) {
            R.id.navHome -> {

            }
            R.id.navSettings -> {

            }
            R.id.navProfile -> {

            }
        }
        drawerLayout.closeDrawer(GravityCompat.START)
        return true
    }

    override fun onBackPressed() {
        val drawerLayout: DrawerLayout = findViewById(R.id.drawerLayout)
        if (drawerLayout.isDrawerOpen(GravityCompat.START)) {
            drawerLayout.closeDrawer(GravityCompat.START)
        } else {
            if (supportFragmentManager?.backStackEntryCount == 0 && isBackPressedOnce) {
                super.onBackPressed()
                return
            } else if (supportFragmentManager?.backStackEntryCount == 0 && !isBackPressedOnce) {

                isBackPressedOnce = true
                Toast.makeText(this, getString(R.string.press_back_again_to_exit), Toast.LENGTH_SHORT).show()
                Handler().postDelayed({
                    isBackPressedOnce = false
                }, 2000)
            } else {
                super.onBackPressed()
            }
        }
    }
}

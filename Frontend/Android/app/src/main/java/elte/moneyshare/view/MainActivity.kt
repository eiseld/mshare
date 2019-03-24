package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import android.view.Menu
import android.view.MenuItem
import elte.moneyshare.R
import elte.moneyshare.viewmodel.GroupsViewModel

class MainActivity : AppCompatActivity() {

    private lateinit var groupsViewModel: GroupsViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        supportFragmentManager.beginTransaction().replace(R.id.frame_container, LoginFragment()).commit()

        groupsViewModel = ViewModelProviders.of(this).get(GroupsViewModel::class.java)
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        return when (item.itemId) {
            R.id.createGroup -> {
                supportFragmentManager.beginTransaction().replace(R.id.frame_container, NewGroupFragment()).commit()
                true
            }

            else -> {
                super.onOptionsItemSelected(item)
            }
        }
    }
}

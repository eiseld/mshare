package elte.moneyshare.view

import android.support.v7.app.AppCompatActivity
import android.os.Bundle
import elte.moneyshare.R

class MainActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        val transaction = supportFragmentManager.beginTransaction()
        transaction.replace(R.id.frame_container, LoginFragment()).commit()
    }

}

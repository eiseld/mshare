package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import elte.moneyshare.SharedPreferences

import elte.moneyshare.R
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.showAsDialog
import elte.moneyshare.viewmodel.LoginViewModel
import kotlinx.android.synthetic.main.fragment_login.*
import kotlinx.android.synthetic.main.nav_header_main.*

class LoginFragment : Fragment() {

    private lateinit var viewModel: LoginViewModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_login, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        viewModel = ViewModelProviders.of(this).get(LoginViewModel::class.java)

        loginButton.setOnClickListener {
            val email = emailEditText.text.toString()
            val password = passwordEditText.text.toString()
            //viewModel.putLoginUser("test1@test.hu", "default") { response, error ->
            viewModel.putLoginUser(email, password) { response, error ->
                if(error == null) {
                    activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, GroupsFragment())?.commit()
                } else {
                    DialogManager.showInfoDialog(error, context)
                }
            }
        }

        registrationButton.setOnClickListener {
            activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, RegisterFragment())?.addToBackStack(null)?.commit()
        }
        forgottenPasswordButton.setOnClickListener {
            activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, ForgotPasswordFragment())?.addToBackStack(null)?.commit()
        }
    }
}
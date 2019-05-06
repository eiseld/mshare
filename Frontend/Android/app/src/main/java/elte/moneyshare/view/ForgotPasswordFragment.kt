package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import elte.moneyshare.R
import elte.moneyshare.viewmodel.ForgotPasswordModel
import elte.moneyshare.viewmodel.LoginViewModel
import kotlinx.android.synthetic.main.fragment_login.*

class ForgotPasswordFragment : Fragment() {

    private lateinit var viewModel: ForgotPasswordModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_forgotpassword, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        viewModel = ViewModelProviders.of(this).get(ForgotPasswordModel::class.java)

        forgottenPasswordButton.setOnClickListener {
            //viewModel.postLoginUser("test1@test.hu", "default") { response, error ->
            viewModel.putForgotPassword(emailEditText.text.toString()) { response, error ->
                if(error == null) {
                    Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                    activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, GroupsFragment())?.commit()
                } else {
                    Toast.makeText(context, error, Toast.LENGTH_SHORT).show()
                }
            }
        }
    }
}
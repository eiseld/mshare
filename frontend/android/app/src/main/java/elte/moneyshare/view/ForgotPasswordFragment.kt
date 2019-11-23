package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.text.Editable
import android.text.TextWatcher
import android.util.Patterns
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.viewmodel.ForgotPasswordViewModel
import kotlinx.android.synthetic.main.fragment_login.*

class ForgotPasswordFragment : Fragment() {

    private lateinit var viewModel: ForgotPasswordViewModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_forgot_password, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        viewModel = ViewModelProviders.of(this).get(ForgotPasswordViewModel::class.java)

        emailEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val email = emailEditText.text.toString()
                var emailValid = Patterns.EMAIL_ADDRESS.matcher(email).matches()
                if(!emailValid)
                {
                    emailEditText.error = context?.getString(R.string.email_not_correct)
                    forgottenPasswordButton.isEnabled = false
                } else {
                    emailEditText.error = null
                    forgottenPasswordButton.isEnabled = true
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        forgottenPasswordButton.setOnClickListener {
            viewModel.putForgotPassword(emailEditText.text.toString()) { _, error ->
                if(error == null) {
                    DialogManager.showInfoDialog(getString(R.string.email_sent), context)
                    activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, LoginFragment())?.commit()
                } else {
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.PROFILE_RESET,context), context)
                }
                emailEditText.text.clear()
                forgottenPasswordButton.isEnabled = false
            }
        }
    }
}
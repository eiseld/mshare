package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.text.Editable
import android.text.TextWatcher
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.disable
import elte.moneyshare.enable
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.util.showToast
import elte.moneyshare.viewmodel.NewPasswordViewModel
import kotlinx.android.synthetic.main.fragment_new_password.*
import kotlinx.android.synthetic.main.fragment_new_password.passwordAgainEditText
import kotlinx.android.synthetic.main.fragment_new_password.passwordEditText
import kotlinx.android.synthetic.main.fragment_register.*

class NewPasswordFragment : Fragment() {

    private lateinit var viewModel: NewPasswordViewModel
    private var token: String? = null

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        token = arguments?.getString(FragmentDataKeys.NEW_PASSWORD_TOKEN.value)
        return inflater.inflate(R.layout.fragment_new_password, container, false)
    }

    fun passwordValidator(txt: String): String {
        var pwdError = ""
        val uppercaseRegex = """[A-Z]""".toRegex()
        val lowercaseRegex = """[a-z]""".toRegex()
        val numberRegex    = """[0-9]""".toRegex()
        context?.let {
            if(txt.length <6)
            {
                pwdError = it.getString(R.string.minimum_characters).plus('\n')
            }
            if(!txt.contains(uppercaseRegex))
            {
                pwdError = pwdError.plus(it.getString(R.string.uppercase_required).plus('\n'))
            }
            if(!txt.contains(lowercaseRegex))
            {
                pwdError = pwdError.plus(it.getString(R.string.lowercase_required).plus('\n'))
            }
            if(!txt.contains(numberRegex))
            {
                pwdError = pwdError.plus(it.getString(R.string.number_required).plus('\n'))
            }
        }

        return pwdError
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        viewModel = ViewModelProviders.of(this).get(NewPasswordViewModel::class.java)

        passwordEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val txt = passwordEditText.text.toString()
                val txtAgain = passwordAgainEditText.text.toString()
                val pwdError = passwordValidator(txt)
                if (pwdError.length > 1) {
                    passwordEditText.error = pwdError
                    confirmPasswordButton.isEnabled = false
                } else if (txt != txtAgain) {
                    passwordEditText.error = context?.getString(R.string.password_not_matching)
                    confirmPasswordButton.isEnabled = false
                } else {
                    passwordEditText.error = null
                    passwordAgainEditText.error = null
                    confirmPasswordButton.isEnabled = true
                }
            }
            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        passwordAgainEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val txt = passwordEditText.text.toString()
                val txtAgain = passwordAgainEditText.text.toString()
                val pwdError = passwordValidator(txtAgain)
                if(pwdError.length > 1) {
                    passwordAgainEditText.error = pwdError
                    confirmPasswordButton.isEnabled = false
                } else if(txt != txtAgain) {
                    passwordAgainEditText.error = context?.getString(R.string.password_not_matching)
                    confirmPasswordButton.isEnabled = false
                } else {
                    passwordEditText.error = null
                    passwordAgainEditText.error = null
                    confirmPasswordButton.isEnabled = true
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        confirmPasswordButton.setOnClickListener {
            token?.let {
                viewModel.putNewPassword(passwordEditText.text.toString(), it) { _, error ->
                    if (error == null) {
                        DialogManager.showInfoDialog(getString(R.string.new_password_updated), context)
                        activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, LoginFragment())?.commit()
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.AUTH_LOGIN, context), context)
                    }
                }
            }
        }
    }
}
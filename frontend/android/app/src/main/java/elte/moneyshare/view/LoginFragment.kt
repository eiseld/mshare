package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.content.Intent
import android.os.Bundle
import android.support.v4.app.Fragment
import android.text.Editable
import android.text.TextWatcher
import android.util.Patterns
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.*
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.viewmodel.LoginViewModel
import kotlinx.android.synthetic.main.fragment_login.*


class LoginFragment : Fragment() {

    private lateinit var viewModel: LoginViewModel

    fun passwordValidator(txt: String): String {
        var pwdError = ""
        context?.let {
            if(txt.length <6)
            {
                pwdError = it.getString(R.string.minimum_characters).plus('\n')
            }
        }

        return pwdError
    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_login, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        if(SharedPreferences.stayLoggedIn)
        {
            val intent = Intent(context, MainActivity::class.java)
            startActivity(intent)
            activity?.finish()
        }
        viewModel = ViewModelProviders.of(this).get(LoginViewModel::class.java)

        emailEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val email = emailEditText.text.toString()
                val password = passwordEditText.text.toString()
                var emailValid = Patterns.EMAIL_ADDRESS.matcher(email).matches()
                if(!emailValid)
                {
                    emailEditText.error = context?.getString(R.string.email_not_correct)
                    loginButton.isEnabled = false
                } else if (passwordValidator(password).isEmpty()){
                    emailEditText.error = null
                    loginButton.isEnabled = true
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        passwordEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val txt = passwordEditText.text.toString()
                val email = emailEditText.text.toString()
                val pwdError = passwordValidator(txt)
                if (pwdError.length > 1) {
                    passwordEditText.error = pwdError
                    loginButton.isEnabled = false
                } else if (Patterns.EMAIL_ADDRESS.matcher(email).matches()){
                    passwordEditText.error = null
                    loginButton.isEnabled = true
                }
            }
            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        loginButton.setOnClickListener {
            val email = emailEditText.text.toString()
            val password = passwordEditText.text.toString()
            //viewModel.putLoginUser("test1@test.hu", "default") { response, error ->
            viewModel.putLoginUser(email, password) { _, error ->
                if (error == null) {
                    if(stayLoggedInCheckBox.isChecked)
                    {
                        SharedPreferences.stayLoggedIn = true
                    }

                    val intent = Intent(context, MainActivity::class.java)
                    startActivity(intent)
                    activity?.finish()
                    //activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, GroupsFragment())?.commit()
                } else {
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.AUTH_LOGIN, context), context)
                }
            }
        }

        if (BuildConfig.FLAVOR == "local") {
            urlEditText.visible()
            updateUrlButton.visible()
        } else {
            urlEditText.gone()
            updateUrlButton.gone()
        }

        urlEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(text: Editable?) {
                if (text.isNullOrEmpty()) {
                    updateUrlButton.disable()
                } else {
                    updateUrlButton.enable()
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {}
            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {}
        })
        updateUrlButton.setOnClickListener {
            (activity as LoginActivity).updateUrl(urlEditText.text.toString())
        }

        registrationButton.setOnClickListener {
            activity?.supportFragmentManager?.beginTransaction()
                ?.replace(R.id.frame_container, RegisterFragment())?.addToBackStack(null)?.commit()
        }
        forgottenPasswordButton.setOnClickListener {
            activity?.supportFragmentManager?.beginTransaction()
                ?.replace(R.id.frame_container, ForgotPasswordFragment())?.addToBackStack(null)
                ?.commit()
        }

        huButton.setOnClickListener {
            SharedPreferences.lang = "hu"
            (activity as LoginActivity).updateLang(SharedPreferences.lang)
            (activity as LoginActivity).refresh()
        }

        enButton.setOnClickListener {
            SharedPreferences.lang = "en"
            (activity as LoginActivity).updateLang(SharedPreferences.lang)
            (activity as LoginActivity).refresh()
        }
    }
}
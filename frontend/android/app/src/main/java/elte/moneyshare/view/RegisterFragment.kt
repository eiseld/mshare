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
import android.widget.Toast
import elte.moneyshare.R
import elte.moneyshare.entity.RegistrationData
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.viewmodel.RegisterViewModel
import kotlinx.android.synthetic.main.fragment_register.*

class RegisterFragment : Fragment() {
    private lateinit var viewModel: RegisterViewModel

    private var displayNameCorrect: Boolean = false;
    private var emailCorrect: Boolean = false;
    private var passwordCorrect: Boolean = false;

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_register, container, false)
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

    fun checkValidity() {
        if(displayNameCorrect && emailCorrect && passwordCorrect ) {
            registerButton.isEnabled = true
        }
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        viewModel = ViewModelProviders.of(this).get(RegisterViewModel::class.java)

        registerButton.setOnClickListener {
            viewModel.postRegisterUser(
                registrationData = RegistrationData(
                    emailEditText.text.toString(),
                    passwordEditText.text.toString(),
                    displayNameEditText.text.toString()
                )
            ) { response, error ->
                if (error == null) {
                    if(response == "201") {
                        DialogManager.showInfoDialog(
                            context?.getString(R.string.successful_registration), context
                        )
                        activity?.supportFragmentManager?.beginTransaction()
                            ?.replace(R.id.frame_container, LoginFragment())?.commit()
                    }
                    else {
                        Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                    }
                } else {
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.AUTH_REGISTER,context), context)
                }
            }
        }

        displayNameEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                if(displayNameEditText.text.isEmpty()) {
                    displayNameCorrect = false
                } else {
                    displayNameCorrect = true
                    checkValidity()
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
                val txtAgain = passwordAgainEditText.text.toString()
                val pwdError = passwordValidator(txt)
                if (pwdError.length > 1) {
                    passwordEditText.error = pwdError
                    passwordCorrect = false
                } else if (txt != txtAgain) {
                    passwordEditText.error = context?.getString(R.string.password_not_matching)
                    passwordCorrect = false
                } else {
                    passwordEditText.error = null
                    passwordAgainEditText.error = null
                    passwordCorrect = true
                }
                checkValidity()
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
                    passwordCorrect = false
                } else if(txt != txtAgain) {
                    passwordAgainEditText.error = context?.getString(R.string.password_not_matching)
                    passwordCorrect = false
                } else {
                    passwordEditText.error = null
                    passwordAgainEditText.error = null
                    passwordCorrect = true
                }
                checkValidity()
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        emailEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val email = emailEditText.text.toString()
                val emailAgain = emailAgainEditText.text.toString()
                var emailValid = Patterns.EMAIL_ADDRESS.matcher(email).matches()
                if(!emailValid) {
                    emailEditText.error = context?.getString(R.string.email_not_correct)
                    emailCorrect = false
                } else if(email != emailAgain) {
                    emailEditText.error = context?.getString(R.string.email_not_matching)
                    emailCorrect = false
                } else {
                    emailEditText.error = null
                    emailAgainEditText.error = null
                    emailCorrect = true
                }
                checkValidity()
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        emailAgainEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val email = emailEditText.text.toString()
                val emailAgain = emailAgainEditText.text.toString()
                var emailValid = Patterns.EMAIL_ADDRESS.matcher(emailAgain).matches()
                if(!emailValid)
                {
                    emailAgainEditText.error = context?.getString(R.string.email_not_correct)
                    emailCorrect = false
                }
                else if(email != emailAgain)
                {
                    emailAgainEditText.error = context?.getString(R.string.email_not_matching)
                    emailCorrect = false
                }
                else {
                    emailEditText.error = null
                    emailAgainEditText.error = null
                    emailCorrect = true
                }
                checkValidity()
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })
    }
}
package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.text.Editable
import android.text.TextWatcher
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast

import elte.moneyshare.R
import elte.moneyshare.entity.RegistrationData
import elte.moneyshare.viewmodel.RegisterViewModel
import kotlinx.android.synthetic.main.fragment_register.*

class RegisterFragment : Fragment() {
    private lateinit var viewModel: RegisterViewModel

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
        if(txt.length <6)
        {
            pwdError = "Minimum length = 6\n"
        }
        if(!txt.contains(uppercaseRegex))
        {
            pwdError = pwdError.plus("At least 1 uppercase character is required\n")
        }
        if(!txt.contains(lowercaseRegex))
        {
            pwdError = pwdError.plus("At least 1 lowercase character is required\n")
        }
        if(!txt.contains(numberRegex))
        {
            pwdError = pwdError.plus("At least 1 number is required")
        }
        return pwdError
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
                        Toast.makeText(context, "Successful registration!", Toast.LENGTH_SHORT).show()
                        activity?.supportFragmentManager?.beginTransaction()
                            ?.replace(R.id.frame_container, LoginFragment())?.commit()
                    }
                    else
                    {
                        Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                    }
                } else {
                    Toast.makeText(context, error, Toast.LENGTH_SHORT).show()
                }
            }
        }
        /*passwordEditText.addTextChangedListener(object: TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                val content = s?.text.toString()
                s?.error = if (content.length >= 6) null else "Minimum length = 6"
            }
            override fun beforeTextChanged(s: Editable?) { }
            override fun onTextChanged(s: Editable?) { }
        })*/
        passwordEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val txt = passwordEditText.text.toString()
                val txtAgain = passwordAgainEditText.text.toString()
                val pwdError = passwordValidator(txt)
                if(pwdError.length>1)
                {
                    passwordEditText.error = pwdError
                    registerButton.isClickable = false
                }
                else if(txt != txtAgain)
                {
                    passwordEditText.error = "Passwords not matching"
                    registerButton.isClickable = false
                }
                else {
                    passwordEditText.error = null
                    passwordAgainEditText.error = null
                    registerButton.isClickable = true
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
                if(pwdError.length>1)
                {
                    passwordAgainEditText.error = pwdError
                    registerButton.isClickable = false
                }
                else if(txt != txtAgain)
                {
                    passwordAgainEditText.error = "Passwords not matching"
                    registerButton.isClickable = false
                }
                else {
                    passwordEditText.error = null
                    passwordAgainEditText.error = null
                    registerButton.isClickable = true
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })


    }
}
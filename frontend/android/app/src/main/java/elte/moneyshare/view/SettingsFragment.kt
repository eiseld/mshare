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
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.ChangePasswordData
import elte.moneyshare.entity.PasswordUpdate
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.viewmodel.ProfileViewModel
import kotlinx.android.synthetic.main.fragment_settings.*

class SettingsFragment : Fragment() {

    private lateinit var viewModel: ProfileViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        // Retain this fragment across configuration changes.
        retainInstance = true
    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_settings, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        viewModel = ViewModelProviders.of(this).get(ProfileViewModel::class.java)

        huButton.setOnClickListener {
            viewModel.updateLang("HU") { _, error ->
                handleUpdateLangResponse("HU", error)
            }
        }

        enButton.setOnClickListener {
            viewModel.updateLang("EN") { _, error ->
                handleUpdateLangResponse("EN", error)
            }
        }

        fun passwordValidator(txt: String): String {
            var pwdError = ""

            if(txt.isEmpty()) {
                return pwdError
            }

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

            newPasswordText.addTextChangedListener(object : TextWatcher {
                override fun afterTextChanged(p0: Editable?) {
                    val txt = newPasswordText.text.toString()
                    val txtAgain = newPasswordAgainText.text.toString()
                    val pwdError = passwordValidator(txt)
                    if (pwdError.length > 1) {
                        newPasswordText.error = pwdError
                        passwordSaveButton.isClickable = false
                    } else if (txt != txtAgain) {
                        newPasswordText.error = context?.getString(R.string.password_not_matching)
                        passwordSaveButton.isClickable = false
                    } else {
                        newPasswordText.error = null
                        newPasswordAgainText.error = null
                        passwordSaveButton.isClickable = true
                    }
                }
                override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
                }

                override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
                }
            })

        newPasswordAgainText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val txt = newPasswordText.text.toString()
                val txtAgain = newPasswordAgainText.text.toString()
                val pwdError = passwordValidator(txtAgain)
                if(pwdError.length>1)
                {
                    newPasswordAgainText.error = pwdError
                    passwordSaveButton.isClickable = false
                }
                else if(txt != txtAgain)
                {
                    newPasswordAgainText.error = context?.getString(R.string.password_not_matching)
                    passwordSaveButton.isClickable = false
                }
                else {
                    newPasswordText.error = null
                    newPasswordAgainText.error = null
                    passwordSaveButton.isClickable = true
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        passwordSaveButton.setOnClickListener {
            viewModel.passwordUpdate (
                passwordUpdate = PasswordUpdate(null, null, oldPasswordText.text.toString(), newPasswordText.text.toString())
            ) { response, error ->
                if (error == null) {
                    if(response == "200") {
                        oldPasswordText.setText("")
                        newPasswordText.setText("")
                        newPasswordAgainText.setText("")
                        Toast.makeText(context, context?.getString(R.string.passwordSuccessfullyChanged), Toast.LENGTH_SHORT).show()
                    }
                    else {
                        Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                    }
                } else {
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.CHANGE_PASSWORD,context), context)
                }
            }
        }
    }

    private fun handleUpdateLangResponse(lang: String, error: String?) {
        if (error == null) {
            SharedPreferences.lang = lang
            (activity as MainActivity).updateLang(SharedPreferences.lang)
            (activity as MainActivity).refresh()
        } else {
            DialogManager.showInfoDialog(error, context)
        }
    }
}
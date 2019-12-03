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
import elte.moneyshare.*
import elte.moneyshare.entity.BankAccountNumberUpdate
import elte.moneyshare.entity.PasswordUpdate
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.viewmodel.ProfileViewModel
import kotlinx.android.synthetic.main.app_bar_main.*
import kotlinx.android.synthetic.main.fragment_profile.*

class ProfileFragment : Fragment() {

    private lateinit var viewModel: ProfileViewModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_profile, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        var storedBankAccountNumber = ""

        viewModel = ViewModelProviders.of(this).get(ProfileViewModel::class.java)

        viewModel.getProfile { userData, error ->
            nameTextView.text = userData?.name
            accountEditText?.setText(formatBankAccountNumber(userData?.bankAccountNumber))
        }

        modifyButton.setOnClickListener {
            viewModel.getProfile { userData, error ->
                accountEditText?.setText(userData?.bankAccountNumber)
                storedBankAccountNumber = accountEditText.text.toString()
                modifyButton.visibility = View.INVISIBLE
                saveButton.visible()
                saveButton.disable()
            }
            accountEditText.enable()
        }

        accountEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                saveButton.isEnabled =
                    accountEditText.text.length == 24 && accountEditText.text.matches("^\\d+$".toRegex()) && accountEditText.text.toString() != storedBankAccountNumber
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        saveButton.setOnClickListener {
            val accountNumber = accountEditText.text.toString()

            viewModel.updateProfile(BankAccountNumberUpdate(SharedPreferences.userId, accountNumber)) { response, error ->
                if (error == null) {
                    accountEditText?.disable()
                    modifyButton.visibility = View.VISIBLE
                    saveButton.visibility = View.GONE
                } else {
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.PROFILE_UPDATE, context), context)
                }
            }
        }

        fun passwordValidator(txt: String): String {
            var pwdError = ""

            if (txt.isEmpty()) {
                return pwdError
            }

            val uppercaseRegex = """[A-Z]""".toRegex()
            val lowercaseRegex = """[a-z]""".toRegex()
            val numberRegex = """[0-9]""".toRegex()
            context?.let {
                if (txt.length < 6) {
                    pwdError = it.getString(R.string.minimum_characters).plus('\n')
                }
                if (!txt.contains(uppercaseRegex)) {
                    pwdError = pwdError.plus(it.getString(R.string.uppercase_required).plus('\n'))
                }
                if (!txt.contains(lowercaseRegex)) {
                    pwdError = pwdError.plus(it.getString(R.string.lowercase_required).plus('\n'))
                }
                if (!txt.contains(numberRegex)) {
                    pwdError = pwdError.plus(it.getString(R.string.number_required).plus('\n'))
                }
            }

            return pwdError
        }

        oldPasswordText.addTextChangedListener(object : TextWatcher {

            override fun afterTextChanged(p0: Editable?) {
                val oldPassword = oldPasswordText.text.toString()
                val newPassword = newPasswordText.text.toString()
                val newPasswordAgain = newPasswordAgainText.text.toString()
                passwordSaveButton.isEnabled = (
                        oldPassword.isNotEmpty() &&
                                passwordValidator(newPassword).isEmpty() &&
                                newPassword.isNotEmpty() &&
                                passwordValidator(newPasswordAgain).isEmpty() &&
                                newPasswordAgain.isNotEmpty()
                        )
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        newPasswordText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val oldPassword = oldPasswordText.text.toString()
                val newPassword = newPasswordText.text.toString()
                val newPasswordAgain = newPasswordAgainText.text.toString()
                val pwdError = passwordValidator(newPassword)
                if (pwdError.length > 1) {
                    newPasswordTextInputLayout.error = pwdError
                    passwordSaveButton.isEnabled = false
                } else if (newPassword != newPasswordAgain) {
                    newPasswordTextInputLayout.error = context?.getString(R.string.password_not_matching)
                    passwordSaveButton.isEnabled = false
                } else if (oldPassword.isEmpty()) {
                    passwordSaveButton.isEnabled = false
                } else {
                    newPasswordTextInputLayout.error = null
                    newPasswordAgainTextInputLayout.error = null
                    passwordSaveButton.isEnabled = true
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        newPasswordAgainText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                val oldPassword = oldPasswordText.text.toString()
                val newPassword = newPasswordText.text.toString()
                val newPasswordAgain = newPasswordAgainText.text.toString()
                val pwdError = passwordValidator(newPasswordAgain)
                if (pwdError.length > 1) {
                    newPasswordAgainTextInputLayout.error = pwdError
                    passwordSaveButton.isEnabled = false
                } else if (newPassword != newPasswordAgain) {
                    newPasswordAgainTextInputLayout.error = context?.getString(R.string.password_not_matching)
                    passwordSaveButton.isEnabled = false
                } else if (oldPassword.isEmpty()) {
                    passwordSaveButton.isEnabled = false
                } else {
                    newPasswordTextInputLayout.error = null
                    newPasswordAgainTextInputLayout.error = null
                    passwordSaveButton.isEnabled = true
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        passwordSaveButton.setOnClickListener {
            passwordSaveButton.disable()
            viewModel.passwordUpdate(
                passwordUpdate = PasswordUpdate(null, null, oldPasswordText.text.toString(), newPasswordText.text.toString())
            ) { response, error ->
                if (error == null) {
                    if (response == "200") {
                        oldPasswordText.setText("")
                        newPasswordText.setText("")
                        newPasswordAgainText.setText("")
                        Toast.makeText(context, context?.getString(R.string.passwordSuccessfullyChanged), Toast.LENGTH_SHORT).show()
                    } else {
                        Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                    }
                } else {
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.CHANGE_PASSWORD, context), context)
                }
                passwordSaveButton.enable()
            }
        }
    }

    private fun formatBankAccountNumber(bankAccountNumber: String?): String {
        if (!((bankAccountNumber.isNullOrEmpty())))
            return bankAccountNumber.substring(0, 8) + "-" + bankAccountNumber.substring(8, 16) + "-" + bankAccountNumber.substring(16, 24)
        else
            return ""
    }

    override fun onResume() {
        super.onResume()
        (activity as MainActivity).toolbar.title = getString(R.string.profile)
    }
}
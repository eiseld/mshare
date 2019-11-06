package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.text.Editable
import android.text.TextWatcher
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.*
import elte.moneyshare.entity.BankAccountNumberUpdate
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.viewmodel.ProfileViewModel
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
                modifyButton.gone()
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
                if(error == null) {
                    activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, ProfileFragment())?.commit()
                    modifyButton.visibility = View.VISIBLE
                    saveButton.visibility = View.GONE
                } else {
                    DialogManager.showInfoDialog(
                        error.convertErrorCodeToString(
                            Action.PROFILE_UPDATE,
                            context
                        ), context
                    )
                }
            }
            viewModel.getProfile { userData, error ->
                accountEditText?.setText(formatBankAccountNumber(userData?.bankAccountNumber))
            }
        }
    }

    private fun formatBankAccountNumber(bankAccountNumber: String?) : String {
        if (!((bankAccountNumber.isNullOrEmpty())))
            return bankAccountNumber.substring(0, 8) + "-" + bankAccountNumber.substring(8, 16) + "-" + bankAccountNumber.substring(16, 24)
        else
            return ""
    }

}
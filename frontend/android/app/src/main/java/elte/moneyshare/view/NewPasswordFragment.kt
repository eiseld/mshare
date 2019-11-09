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

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        viewModel = ViewModelProviders.of(this).get(NewPasswordViewModel::class.java)

        passwordEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(pw: Editable?) {
                if (pw.toString().length < 6) {
                    passwordEditText.error = getString(R.string.minimum_characters)
                    confirmPasswordButton.disable()
                } else {
                    passwordEditText.error = null
                    confirmPasswordButton.enable()
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
                        context?.let { getString(R.string.new_password_updated).showToast(it) }
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.AUTH_LOGIN, context), context)
                    }
                }
            }
        }
    }
}
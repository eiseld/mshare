package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.enable
import elte.moneyshare.entity.UserData
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.viewmodel.LoginViewModel
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

        viewModel = ViewModelProviders.of(this).get(ProfileViewModel::class.java)

        viewModel.getProfile { userData, error ->
            nameTextView.text = userData?.name
        }

        modifyButton.setOnClickListener {
            accountEditText.enable()
        }

        saveButton.setOnClickListener {
            //TODO update profile CALL VIEWMODEL PROPER METHOD
            viewModel.updateProfile(UserData(0,"name")) { response, error ->
                if(error == null) {
                    //TODO
                } else {
                    DialogManager.showInfoDialog(error, context)
                }
            }
        }
    }
}
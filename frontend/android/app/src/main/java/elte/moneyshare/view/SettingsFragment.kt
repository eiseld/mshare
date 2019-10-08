package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.viewmodel.ProfileViewModel
import kotlinx.android.synthetic.main.fragment_settings.*

class SettingsFragment : Fragment() {

    private lateinit var viewModel: ProfileViewModel

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
            viewModel.updateLang("HU") { response, error ->
                if (error == null) {
                    SharedPreferences.lang = "HU"
                    (activity as MainActivity).updateLang(SharedPreferences.lang)
                    (activity as MainActivity).refresh()
                } else {
                    DialogManager.showInfoDialog(error, context)
                }
            }
        }

        enButton.setOnClickListener {
            viewModel.updateLang("EN") { response, error ->
                if (error == null) {
                    SharedPreferences.lang = "EN"
                    (activity as MainActivity).updateLang(SharedPreferences.lang)
                } else {
                    DialogManager.showInfoDialog(error, context)
                }
            }
        }
    }
}
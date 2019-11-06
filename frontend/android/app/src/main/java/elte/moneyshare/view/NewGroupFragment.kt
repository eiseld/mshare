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
import elte.moneyshare.disable
import elte.moneyshare.enable
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.viewmodel.GroupsViewModel
import kotlinx.android.synthetic.main.fragment_groupcreation.*

class NewGroupFragment : Fragment() {

    private lateinit var viewModel: GroupsViewModel

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_groupcreation, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        groupNameEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(text: Editable?) {
                if (text.isNullOrEmpty()) {
                    createButton.disable()
                } else {
                    createButton.enable()
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {
            }
        })

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupsViewModel::class.java)
        }

        createButton.setOnClickListener {
            viewModel.postNewGroup(groupNameEditText.text.toString()) { response, error ->
                if (error == null) {
                    Toast.makeText(context, getString(R.string.group_created), Toast.LENGTH_SHORT).show()
                    activity?.supportFragmentManager?.popBackStackImmediate()
                } else {
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS_CREATE,context), context)
                }
            }
        }
    }
}
package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast

import elte.moneyshare.R
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

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupsViewModel::class.java)
        }

        createButton.setOnClickListener {
            viewModel.postNewGroup(groupNameEditText.text.toString()) { response, error ->
                if(error == null) {
                    Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                } else {
                    Toast.makeText(context, error, Toast.LENGTH_SHORT).show()
                }
            }
        }
    }
}
package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.support.v4.app.Fragment
import android.os.Bundle
import android.support.v7.widget.LinearLayoutManager
import android.text.Editable
import android.text.TextWatcher
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.entity.FilteredUserData
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.Adapter.SearchResultsRecyclerViewAdapter
import elte.moneyshare.viewmodel.AddMembersViewModel
import kotlinx.android.synthetic.main.fragment_add_members.*

class AddMembersFragment : Fragment(), SearchResultsRecyclerViewAdapter.MemberInvitedListener {
    override fun onInvited() {
    }

    private lateinit var viewModel: AddMembersViewModel
    lateinit var adapter : SearchResultsRecyclerViewAdapter
    private var groupId: Int? = null

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        groupId = arguments?.getInt(FragmentDataKeys.ADD_MEMBERS_FRAGMENT.value)
        return inflater.inflate(R.layout.fragment_add_members, container, false)

    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(AddMembersViewModel::class.java)

            groupId?.let { groupId ->
                viewModel.getGroupData(groupId) { groupData, error ->
                    if (groupData != null) {
                        var arrayList: ArrayList<FilteredUserData> = ArrayList()
                        adapter = SearchResultsRecyclerViewAdapter(context!!, arrayList, groupId!!, this@AddMembersFragment, viewModel)
                        searchResultsRecycleView.adapter = adapter
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS,context), context)
                    }
                }
            }
        }

        searchResultsRecycleView?.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)

        nameOrEmailEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(p0: Editable?) {
                var newText = nameOrEmailEditText.text.toString()
                if (newText.length > 3) {
                    viewModel.getSearchedUsers(newText) { filteredUsers, error ->

                        adapter.filteredUsers = filteredUsers!!
                        adapter.notifyDataSetChanged()

                    }
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {}

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {}

        })

    }

}
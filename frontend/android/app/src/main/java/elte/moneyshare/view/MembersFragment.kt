package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v7.widget.LinearLayoutManager
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.GroupData
import elte.moneyshare.entity.Member
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.Adapter.MembersRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.fragment_members.*


class MembersFragment : Fragment() {

    private lateinit var viewModel: GroupViewModel
    private lateinit var groupDataStored : GroupData
    lateinit var adapter : MembersRecyclerViewAdapter
    private var groupId: Int? = null

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        groupId = arguments?.getInt(FragmentDataKeys.MEMBERS_FRAGMENT.value)
        return inflater.inflate(R.layout.fragment_members, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupViewModel::class.java)

            groupId?.let { groupId ->
                viewModel.getGroupData(groupId) { groupData, error ->
                    if (groupData != null) {

                        val member: Member? = groupData.members.find { it.id == SharedPreferences.userId }
                        groupData.members.remove(member)

                        groupDataStored = groupData

                        adapter = MembersRecyclerViewAdapter(it, groupData, myBalanceTextView, viewModel)
                        member?.let {
                            myNameTextView?.text = it.name
                            myBalanceTextView?.text = it.balance.toString()
                        }

                        membersRecyclerView.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                        membersRecyclerView.adapter = adapter
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS,context), context)
                    }
                }
            }
        }
    }

    override fun onPause() {
        super.onPause()
        viewModel.isDeleteMemberEnabled = false
    }
}

package elte.moneyshare.view


import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v7.widget.LinearLayoutManager
import android.view.*
import android.widget.Toast
import elte.moneyshare.FragmentDataKeys

import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.Member
import elte.moneyshare.view.Adapter.GroupsRecyclerViewAdapter
import elte.moneyshare.view.Adapter.MembersRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupsViewModel
import kotlinx.android.synthetic.main.fragment_members.*

class MembersFragment : Fragment() {

    private lateinit var viewModel: GroupsViewModel
    private var groupId: Int? = null

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        groupId = arguments?.getInt(FragmentDataKeys.MEMBERS_FRAGMENT.value)
        return inflater.inflate(R.layout.fragment_members, container, false)
    }

    override fun onCreateOptionsMenu(menu: Menu, inflater: MenuInflater) {
        inflater.inflate(R.menu.menu, menu)
        super.onCreateOptionsMenu(menu, inflater)
    }

    override fun onOptionsItemSelected(item: MenuItem?): Boolean {
        when(item?.itemId){
            R.id.createGroup -> {
                activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, NewGroupFragment())?.addToBackStack(null)?.commit()
                return true
            }
            else -> return super.onOptionsItemSelected(item)
        }
    }
    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupsViewModel::class.java)

            groupId?.let { groupId ->
                viewModel.getGroupData(groupId) { groupData, error ->
                    if (groupData != null) {
                        val adapter = MembersRecyclerViewAdapter(it, groupData, viewModel)
                        val user: Member? = groupData.members.find { it.id == SharedPreferences.userId }
                        if(user == null)
                        {
                            my_balanceTextView2.text = "##"
                        }
                        else {
                            my_balanceTextView2.text = user.balance.toString()
                        }
                        membersRecyclerView.layoutManager =
                            LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                        membersRecyclerView.adapter = adapter
                    } else {
                        Toast.makeText(context, error.toString(), Toast.LENGTH_SHORT).show()
                    }
                }
            }
        }

    }
}

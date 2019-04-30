package elte.moneyshare.view


import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v7.widget.LinearLayoutManager
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import elte.moneyshare.FragmentDataKeys

import elte.moneyshare.R
import elte.moneyshare.entity.Debtor
import elte.moneyshare.entity.NewSpending
import elte.moneyshare.view.Adapter.SelectMembersRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupsViewModel
import kotlinx.android.synthetic.main.fragment_add_spending.*

class AddSpendingFragment : Fragment() {

    private lateinit var viewModel: GroupsViewModel
    private var groupId: Int? = null

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        groupId = arguments?.getInt(FragmentDataKeys.MEMBERS_FRAGMENT.value)
        return inflater.inflate(R.layout.fragment_add_spending, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupsViewModel::class.java)

            groupId?.let { groupId ->
                viewModel.getGroupData(groupId) { groupData, error ->
                    if (groupData != null) {
                        val adapter = SelectMembersRecyclerViewAdapter(it, groupData.members)
                        selectMembersRecyclerView.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                        selectMembersRecyclerView.adapter = adapter
                    } else {
                        Toast.makeText(context, error.toString(), Toast.LENGTH_SHORT).show()
                    }
                }
            }
        }

        //TODO REFACTOR after backend updated
        addButton.setOnClickListener {
            val debtors: ArrayList<Debtor> = ArrayList()
            val moneySpend = Integer.valueOf(spendingEditText.editableText.toString())
            val memberIds = (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).getSelectedMembersIds()
            val debt = moneySpend.div(memberIds.size)
            val mod = moneySpend - (debt * memberIds.size)

            for (id in memberIds) {
                debtors.add(Debtor(
                    debtorId = id,
                    debt = debt
                ))
            }
            debtors[0].debt += mod

            val spending = NewSpending(
                groupId = groupId!!,
                moneySpent = Integer.valueOf(spendingEditText.editableText.toString()),
                name = nameEditText.editableText.toString(),
                debtors = debtors
            )

            viewModel.postSpending(spending) { response, error ->
                if (error == null) {
                    (context as MainActivity).onBackPressed()
                } else {
                    Toast.makeText(context, error.toString(), Toast.LENGTH_SHORT).show()
                }
            }
        }
    }
}
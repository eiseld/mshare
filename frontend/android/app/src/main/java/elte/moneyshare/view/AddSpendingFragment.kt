package elte.moneyshare.view


import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v7.widget.LinearLayoutManager
import android.text.TextUtils
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import elte.moneyshare.*
import elte.moneyshare.entity.Debtor
import elte.moneyshare.entity.Member
import elte.moneyshare.entity.NewSpending
import elte.moneyshare.entity.SpendingUpdate
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.Adapter.SelectMembersRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.fragment_add_spending.*

class AddSpendingFragment : Fragment() {

    private lateinit var viewModel: GroupViewModel
    private var groupId: Int? = null
    private var spendingId = -1
    private var isModify : Boolean = false
    private var members = ArrayList<Member>()

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        groupId = arguments?.getInt(FragmentDataKeys.MEMBERS_FRAGMENT.value)
        var spendingIdTemp = arguments?.getInt(FragmentDataKeys.BILLS_FRAGMENT.value)
        if(spendingIdTemp != null && spendingIdTemp != -1)
        {
            isModify = true
            spendingId = spendingIdTemp
        }

        return inflater.inflate(R.layout.fragment_add_spending, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        var adapter: SelectMembersRecyclerViewAdapter? = null

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupViewModel::class.java)

            groupId?.let { groupId ->
                viewModel.getMembersToSpending(groupId) { members, error ->
                    if (members != null) {
                        this.members = members
                        adapter = SelectMembersRecyclerViewAdapter(it, members)
                        selectMembersRecyclerView.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                        selectMembersRecyclerView.adapter = adapter
                        if(isModify)
                        {
                            viewModel.getSpendings(groupId) { spendings, error ->
                                if (spendings != null)
                                {
                                    val spendingDataTemp = spendings.find { it.id == spendingId }
                                    spendingDataTemp?.let {
                                        spendingEditText.setText(it.moneyOwed.toString())
                                        nameEditText.setText(it.name)
                                        val debtorIds = it.debtors.map { it.id }
                                        (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).selectedIds = ArrayList(debtorIds)
                                        val selectedMembers = members.filter { it.id in debtorIds }
                                        (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).selectedMembers = ArrayList(selectedMembers)
                                        adapter?.notifyDataSetChanged()
                                    }
                                }
                                else
                                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.SPENDING, context), context)
                            }
                        }
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS,context), context)
                    }
                }
            }
        }

        if(isModify)
        {
            addButton.text = context?.getString(R.string.modify_spending)
        }
        nextButton.setOnClickListener {
            val selectedIds = (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).selectedIds
            val membersSelected = members.filter { selectedIds.contains(it.id) } as ArrayList<Member>

            val validAll = !TextUtils.isEmpty(nameEditText.editableText.toString()) &&
                    !TextUtils.isEmpty(spendingEditText.editableText.toString()) &&
                    spendingEditText.editableText.toString() != "0" &&
                    membersSelected.size > 0

            if (TextUtils.isEmpty(nameEditText.editableText.toString())) {
                nameEditText.error = context?.getString(R.string.cannot_be_empty)
            }

            if (TextUtils.isEmpty(spendingEditText.editableText.toString()) || spendingEditText.editableText.toString() == "0") {
                spendingEditText.error = context?.getString(R.string.must_be_bigger_than_0)
            }

            if (membersSelected.size == 0) {
                Toast.makeText(context, getString(R.string.select_members_missing), Toast.LENGTH_SHORT).show()
            }

            if (validAll) {
                val moneySpend = Integer.valueOf(spendingEditText.editableText.toString())
                val debt = moneySpend.div(membersSelected.size)
                val mod = moneySpend - (debt * membersSelected.size)

                for (member in membersSelected) {
                    member.balance = debt
                }

                membersSelected[0].balance += mod

                activity?.let {
                    val adapter = SelectMembersRecyclerViewAdapter(it, membersSelected, true)
                    selectMembersRecyclerView.layoutManager =
                        LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                    selectMembersRecyclerView.adapter = adapter
                }

                selectEveryoneButton.invisible()
                selectNooneButton.invisible()

                nextButton.invisible()
                addButton.visible()
            }
        }

        selectEveryoneButton.setOnClickListener {
            (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).let{
                val selectedIds: ArrayList<Int> = ArrayList()
                viewModel.currentGroupData?.members?.forEach {
                    selectedIds.add( it.id )
                }
                it.selectedIds = selectedIds
                adapter?.notifyDataSetChanged()
            }
        }

        selectNooneButton.setOnClickListener {
            (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).let {
                val selectedIds: ArrayList<Int> = ArrayList()
                it.selectedIds = selectedIds
                adapter?.notifyDataSetChanged()
            }
        }

        //TODO REFACTOR after backend updated
        addButton.setOnClickListener {
            val debtors: ArrayList<Debtor> = ArrayList()
            val members = (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).selectedMembers

            var sumSpending = 0
            for(member in members) {
                sumSpending += member.balance
            }

            val moneySpend = Integer.valueOf(spendingEditText.editableText.toString())
            if(sumSpending != moneySpend) {
                DialogManager.showInfoDialog(getString(R.string.spending_wrong_balance_sum), context)
                return@setOnClickListener
            }

            for (member in members) {
                debtors.add(Debtor(
                    debtorId = member.id,
                    debt = member.balance
                ))
            }
            if(!isModify)
            {
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
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.SPENDING_UPDATE,context), context)
                    }
                }
            }
            else
            {
                val spending = SpendingUpdate(
                    groupId = groupId!!,
                    moneySpent = Integer.valueOf(spendingEditText.editableText.toString()),
                    name = nameEditText.editableText.toString(),
                    debtors = debtors,
                    id = spendingId,
                    creditorUserId = SharedPreferences.userId
                )

                viewModel.postSpendingUpdate(spending) { response, error ->
                    if (error == null) {
                        (context as MainActivity).onBackPressed()
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.SPENDING_CREATE,context), context)
                    }
                }
            }
        }
    }
}
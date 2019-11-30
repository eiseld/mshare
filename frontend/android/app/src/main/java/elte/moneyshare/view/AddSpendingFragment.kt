package elte.moneyshare.view


import android.app.DatePickerDialog
import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v7.widget.LinearLayoutManager
import android.text.Editable
import android.text.TextUtils
import android.text.TextWatcher
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
import elte.moneyshare.util.*
import elte.moneyshare.view.Adapter.SelectMembersRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.fragment_add_spending.*
import java.util.*
import kotlin.collections.ArrayList

class AddSpendingFragment : Fragment() {

    private var onSplitScreen = false
    private lateinit var viewModel: GroupViewModel
    private var groupId: Int? = null
    private var spendingId = -1
    private var isModify : Boolean = false
    private var members = ArrayList<Member>()
    private var calendar : Calendar = Calendar.getInstance()

    private val dateSetListener = DatePickerDialog.OnDateSetListener { _, year, month, date ->
        calendar.set(year, month, date)
        dateEditText.setText(calendar.formatDate())
    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        groupId = arguments?.getInt(FragmentDataKeys.MEMBERS_FRAGMENT.value)
        var spendingIdTemp = arguments?.getInt(FragmentDataKeys.ADD_SPENDING_FRAGMENT.value)
        if(spendingIdTemp != null && spendingIdTemp != -1)
        {
            isModify = true
            spendingId = spendingIdTemp
        }

        return inflater.inflate(R.layout.fragment_add_spending, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupViewModel::class.java)

            groupId?.let { groupId ->
                viewModel.getMembersToSpending(groupId) { members, error ->
                    if (members != null) {
                        this.members = members
                        val adapter = SelectMembersRecyclerViewAdapter(it, members)
                        selectMembersRecyclerView?.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                        selectMembersRecyclerView?.adapter = adapter
                        if(isModify)
                        {
                            viewModel.getSpendings(groupId) { spendings, error ->
                                if (spendings != null)
                                {
                                    val spendingDataTemp = spendings.find { it.id == spendingId }
                                    spendingDataTemp?.let {
                                        spendingEditText.setText(it.moneyOwed.toString())
                                        nameEditText.setText(it.name)

                                        //Set Calendar
                                        calendar = it.date.convertToCalendar()
                                        dateEditText.setText(calendar.formatDate())

                                        val debtorIds = it.debtors.map { it.id }
                                        (selectMembersRecyclerView?.adapter as SelectMembersRecyclerViewAdapter).selectedIds =
                                            ArrayList(debtorIds)
                                        val selectedMembers = members.filter { it.id in debtorIds }
                                        (selectMembersRecyclerView?.adapter as SelectMembersRecyclerViewAdapter).selectedMembers =
                                            ArrayList(selectedMembers)
                                        adapter?.notifyDataSetChanged()
                                    }
                                }
                                else
                                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.SPENDING, context), context)
                            }
                        } else {
                            dateEditText.setText(calendar.formatDate())
                        }
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS,context), context)
                    }
                }
            }

            dateEditText?.setOnClickListener { _ ->
                DatePickerDialog(
                    it,
                    dateSetListener,
                    // set DatePickerDialog to point to today's date when it loads up
                    calendar.get(Calendar.YEAR),
                    calendar.get(Calendar.MONTH),
                    calendar.get(Calendar.DAY_OF_MONTH)
                ).show()
            }
        }

        if(isModify)
        {
            addButton.text = context?.getString(R.string.modify_spending)
        }

        setupNextButton()
        setupSpendingEditTextListener()
        setupAddButton()
    }

    private fun setupSpendingEditTextListener() {
        spendingEditText.addTextChangedListener(object : TextWatcher {
            override fun afterTextChanged(s: Editable?) {
                if (onSplitScreen) {
                    splitSpendingToSelectedMembers()
                    (selectMembersRecyclerView?.adapter as SelectMembersRecyclerViewAdapter).maxSpending =
                        membersSelected.sumBy { it.balance }
                    selectMembersRecyclerView?.adapter?.notifyDataSetChanged()
                }
            }

            override fun beforeTextChanged(s: CharSequence?, start: Int, count: Int, after: Int) {}

            override fun onTextChanged(s: CharSequence?, start: Int, before: Int, count: Int) {}
        })
    }

    lateinit var membersSelected: ArrayList<Member>

    private fun setupNextButton() {
        nextButton.setOnClickListener {
            val selectedIds = (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).selectedIds
            membersSelected = members.filter { selectedIds.contains(it.id) } as ArrayList<Member>

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
                splitSpendingToSelectedMembers()

                activity?.let {
                    val adapter = SelectMembersRecyclerViewAdapter(it, membersSelected, true)
                    selectMembersRecyclerView?.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                    selectMembersRecyclerView?.adapter = adapter
                }

                selectEveryoneButton.gone()
                selectNoneButton.gone()
                nextButton.invisible()

                addButton.visible()
                onSplitScreen = true
            } else {
                scrollView.smoothScrollTo(0, 0)
            }
        }

        selectEveryoneButton.setOnClickListener {
            (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).let {
                val selectedIds: ArrayList<Int> = ArrayList()
                viewModel.currentGroupData?.members?.forEach {
                    selectedIds.add(it.id)
                }
                it.selectedIds = selectedIds
                it?.notifyDataSetChanged()
            }
        }

        selectNoneButton.setOnClickListener {
            (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).let {
                val selectedIds: ArrayList<Int> = ArrayList()
                it.selectedIds = selectedIds
                it?.notifyDataSetChanged()
            }
        }
    }

    private fun splitSpendingToSelectedMembers() {
        val moneySpend = spendingEditText.editableText.toString().let { if (it.isNotEmpty()) it.toInt() else 0 }
        val debt = moneySpend.div(membersSelected.size)
        val mod = moneySpend - (debt * membersSelected.size)

        for (member in membersSelected) {
            member.balance = debt
        }

        membersSelected[0].balance += mod
    }

    private fun setupAddButton() {
        //TODO REFACTOR after backend updated
        addButton.setOnClickListener {
            val debtors: ArrayList<Debtor> = ArrayList()
            val members = (selectMembersRecyclerView.adapter as SelectMembersRecyclerViewAdapter).selectedMembers

            var sumSpending = 0
            for (member in members) {
                sumSpending += member.balance
            }

            val moneySpend = spendingEditText.editableText.toString().let { if (it.isNotEmpty()) it.toInt() else 0 }
            if (sumSpending != moneySpend) {
                DialogManager.showInfoDialog(getString(R.string.spending_wrong_balance_sum), context)
                return@setOnClickListener
            }

            for (member in members) {
                debtors.add(
                    Debtor(
                        debtorId = member.id,
                        debt = member.balance
                    )
                )
            }
            if (!isModify) {
                val spending = NewSpending(
                    groupId = groupId!!,
                    moneySpent = spendingEditText.editableText.toString().let { if (it.isNotEmpty()) it.toInt() else 0 },
                    name = nameEditText.editableText.toString(),
                    debtors = debtors,
                    date = calendar.convertToBackendFormat()
                )

                viewModel.postSpending(spending) { response, error ->
                    if (error == null) {
                        (context as MainActivity).onBackPressed()
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.SPENDING_UPDATE, context), context)
                    }
                }
            } else {
                val spending = SpendingUpdate(
                    groupId = groupId!!,
                    name = nameEditText.editableText.toString(),
                    id = spendingId,
                    creditorUserId = SharedPreferences.userId,
                    moneySpent = spendingEditText.editableText.toString().let { if (it.isNotEmpty()) it.toInt() else 0 },
                    debtors = debtors,
                    date = calendar.convertToBackendFormat()
                )

                viewModel.postSpendingUpdate(spending) { response, error ->
                    if (error == null) {
                        (context as MainActivity).onBackPressed()
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.SPENDING_CREATE, context), context)
                    }
                }
            }
        }
    }
}
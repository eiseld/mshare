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
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.TimeExtensions
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.Adapter.BillsRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.fragment_bills.*

class BillsFragment : Fragment() {

    private lateinit var viewModel: GroupViewModel
    private var groupId: Int? = null

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        groupId = arguments?.getInt(FragmentDataKeys.MEMBERS_FRAGMENT.value)
        return inflater.inflate(R.layout.fragment_bills, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupViewModel::class.java)
            billsRecyclerView.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)

            groupId?.let { groupId ->
                viewModel.getSpendings(groupId) { bills, error ->
                    if (bills != null) {

                        val orderedBills = bills.map { spendingData ->
                            val calendar = TimeExtensions.convertToCalendar(spendingData.date)
                            spendingData.date = TimeExtensions.formatDate(calendar)
                            Pair(calendar, spendingData)
                        }.sortedByDescending { pair -> pair.first }
                        .map { pair -> pair.second }
                        .toMutableList()

                        val adapter = BillsRecyclerViewAdapter(it, orderedBills, groupId, viewModel)
                        billsRecyclerView.adapter = adapter
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.SPENDING,context), context)
                    }
                }
            }
        }
    }
}

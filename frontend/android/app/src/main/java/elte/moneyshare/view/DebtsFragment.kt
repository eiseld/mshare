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
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.Adapter.OptimizedDebtRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.fragment_my_debts.*

class DebtsFragment : Fragment() {

    private lateinit var viewModel: GroupViewModel
    private var groupId: Int? = null

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        groupId = arguments?.getInt(FragmentDataKeys.MEMBERS_FRAGMENT.value)
        return inflater.inflate(R.layout.fragment_my_debts, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupViewModel::class.java)

            groupId?.let { groupId ->
                viewModel.getOptimizedDebtData(groupId) { groupData, error ->
                    if (groupData != null) {
                        val adapter = OptimizedDebtRecyclerViewAdapter(it, groupData, viewModel, groupId)
                        debtsRecyclerView.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                        debtsRecyclerView.adapter = adapter
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS,context), context)
                    }
                }
            }
        }
    }
}

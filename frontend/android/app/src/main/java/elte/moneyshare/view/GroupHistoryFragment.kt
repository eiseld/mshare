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
import elte.moneyshare.view.Adapter.GroupHistoryRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.fragment_group_history.*

class GroupHistoryFragment : Fragment() {

    private lateinit var viewModel: GroupViewModel
    private var groupId: Int? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        groupId = arguments?.getInt(FragmentDataKeys.GROUP_HISTORY_FRAGMENT.value)
        return inflater.inflate(R.layout.fragment_group_history, container, false)
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupViewModel::class.java)

            groupId?.let { groupId ->
                viewModel.getGroupHistory(it, groupId, 0, 0) { historyItems, error ->
                    historyItems?.let { items ->
                        val adapter = GroupHistoryRecyclerViewAdapter(it, items)
                        historyRecyclerView.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                        historyRecyclerView.adapter = adapter
                    }
                }
            }
        }
    }
}
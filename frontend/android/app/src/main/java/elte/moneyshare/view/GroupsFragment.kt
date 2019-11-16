package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v7.widget.LinearLayoutManager
import android.view.*
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.Adapter.GroupsRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupsViewModel
import kotlinx.android.synthetic.main.fragment_groups.*

class GroupsFragment : Fragment() {

    private lateinit var viewModel: GroupsViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setHasOptionsMenu(true)
    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_groups, container, false)
    }

    override fun onCreateOptionsMenu(menu: Menu, inflater: MenuInflater) {
        inflater.inflate(R.menu.menu, menu)
        super.onCreateOptionsMenu(menu, inflater)
    }

    override fun onOptionsItemSelected(item: MenuItem?): Boolean {
        when(item?.itemId){
            R.id.addGroup -> {
                activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, NewGroupFragment())?.addToBackStack(null)?.commit()
                return true
            }
            else -> return super.onOptionsItemSelected(item)
        }

    }

    private fun getGroups() {
        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupsViewModel::class.java)

            viewModel.getProfileGroups { groupsInfo, error ->
                if (groupsInfo != null) {
                    val adapter = GroupsRecyclerViewAdapter(it, groupsInfo)
                    groupsRecyclerView?.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                    groupsRecyclerView?.adapter = adapter
                } else {
                    if(SharedPreferences.stayLoggedIn)
                    {
                        SharedPreferences.stayLoggedIn = false
                        DialogManager.showInfoDialog(context?.getString(R.string.relog_error).toString(),context)
                        activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, LoginFragment())?.addToBackStack(null)?.commit()
                    }
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS,context), context)
                }
            }
        }
    }

    //livedata would be better to observe changes on data
    override fun onResume() {
        super.onResume()
        getGroups()
    }

}

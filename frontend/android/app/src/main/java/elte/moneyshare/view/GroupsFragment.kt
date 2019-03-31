package elte.moneyshare.view


import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v7.widget.LinearLayoutManager
import android.view.*
import android.widget.Toast

import elte.moneyshare.view.Adapter.GroupsRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupsViewModel
import kotlinx.android.synthetic.main.fragment_groups.*
import elte.moneyshare.R

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
            R.id.createGroup -> {
                activity?.supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, NewGroupFragment())?.addToBackStack(null)?.commit()
                return true
            }
            else -> return super.onOptionsItemSelected(item)
        }
    }

    fun getGroups() {
        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupsViewModel::class.java)

            viewModel.getGroups { groups, error ->
                if (groups != null) {
                    val adapter = GroupsRecyclerViewAdapter(it, groups)
                    groupsRecyclerView.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
                    groupsRecyclerView.adapter = adapter
                } else {
                    Toast.makeText(context, error.toString(), Toast.LENGTH_SHORT).show()
                }
            }
        }
    }

    override fun onResume() {
        super.onResume()
        getGroups()
    }

}

package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.view.*
import elte.moneyshare.view.Adapter.GroupPagerAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.fragment_group.*
import android.support.v4.view.MenuItemCompat
import android.support.v7.widget.LinearLayoutManager
import android.support.v7.widget.SearchView
import elte.moneyshare.*
import elte.moneyshare.view.Adapter.SearchResultsRecyclerViewAdapter

class GroupPagerFragment : Fragment() {

    private var groupId: Int? = null
    private lateinit var pagerAdapter: GroupPagerAdapter
    private lateinit var viewModel: GroupViewModel

    private var tabs: ArrayList<String> = ArrayList()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setHasOptionsMenu(true)
    }

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        groupId = arguments?.getInt(FragmentDataKeys.GROUP_PAGER_FRAGMENT.value)
        return inflater.inflate(R.layout.fragment_group, container, false)
    }

    override fun onCreateOptionsMenu(menu: Menu, inflater: MenuInflater) {
        inflater.inflate(R.menu.menu_group, menu)
        super.onCreateOptionsMenu(menu, inflater)

        val item = menu.findItem(R.id.menuSearch)
        val searchView = SearchView((context as MainActivity).supportActionBar!!.themedContext)
        MenuItemCompat.setShowAsAction(
            item,
            MenuItemCompat.SHOW_AS_ACTION_COLLAPSE_ACTION_VIEW or MenuItemCompat.SHOW_AS_ACTION_IF_ROOM
        )
        MenuItemCompat.setActionView(item, searchView)
        searchResultsRecyclerView?.layoutManager = LinearLayoutManager(context, LinearLayoutManager.VERTICAL, false)
        searchView.setOnQueryTextListener(object : SearchView.OnQueryTextListener {
            override fun onQueryTextSubmit(query: String): Boolean {
                return false
            }

            override fun onQueryTextChange(newText: String): Boolean {
                viewModel.getSearchedUsers(newText) { filteredUsers, error ->
                    if (filteredUsers != null && !filteredUsers.equals("")) {
                        val adapter = SearchResultsRecyclerViewAdapter(context!!, filteredUsers, groupId!!, viewModel)
                        searchResultsRecyclerView?.adapter = adapter
                    }
                }

                if (newText.equals("") || newText.length < 4) {
                    searchResultsRecyclerView?.invisible()
                    tabLayout?.visible()
                } else {
                    searchResultsRecyclerView?.visible()
                    tabLayout?.invisible()
                }
                return false
            }
        })

        searchView.setOnClickListener {}
        searchView.setOnFocusChangeListener { view, hasFocus ->
            if (hasFocus) {
                searchResultsRecyclerView.visible()
            } else {
                searchResultsRecyclerView.invisible()
            }
        }
    }

    override fun onOptionsItemSelected(item: MenuItem?): Boolean {
        when (item?.itemId) {
            R.id.addSpending -> {
                val fragment = AddSpendingFragment()
                val args = Bundle()
                groupId?.let {
                    args.putInt(FragmentDataKeys.MEMBERS_FRAGMENT.value, it)
                }
                args.putInt(FragmentDataKeys.BILLS_FRAGMENT.value,-1)
                fragment.arguments = args
                (context as MainActivity).supportFragmentManager?.beginTransaction()
                    ?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()

                return true
            }
            R.id.menuSearch -> {

                return true
            }

            R.id.removeMember -> {
                viewModel.isDeleteMemberEnabled = !viewModel.isDeleteMemberEnabled

                //TODO REPLACE TO ENUM KEY
                (childFragmentManager.fragments[0] as MembersFragment).adapter.notifyDataSetChanged()
                tabLayout.getTabAt(0)?.select()

                return true
            }
            R.id.myDebts -> {
                val fragment = DebtsFragment()
                val args = Bundle()
                groupId?.let {
                    args.putInt(FragmentDataKeys.MEMBERS_FRAGMENT.value, it)
                }
                fragment.arguments = args
                (context as MainActivity).supportFragmentManager?.beginTransaction()
                    ?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()
                return true
            }
            else ->
                return super.onOptionsItemSelected(item)
        }
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupViewModel::class.java)
        }

        if (tabs.isEmpty()) {
            tabs.add("Members")
            tabs.add("Bills")
        }
        initViewPager()
    }

    fun initViewPager() {
        groupId?.let {
            pagerAdapter = GroupPagerAdapter(it, tabs, childFragmentManager)
            viewPager.adapter = pagerAdapter
            viewPager.offscreenPageLimit = 1
            tabLayout.setupWithViewPager(viewPager)
            tabLayout.getTabAt(0)?.select()
        }
    }
}
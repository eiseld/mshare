package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v4.view.MenuItemCompat
import android.support.v7.widget.LinearLayoutManager
import android.support.v7.widget.SearchView
import android.util.Log
import android.view.*
import elte.moneyshare.*
import elte.moneyshare.entity.GroupDataParc
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.Adapter.GroupPagerAdapter
import elte.moneyshare.view.Adapter.SearchResultsRecyclerViewAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.app_bar_main.*
import kotlinx.android.synthetic.main.fragment_group.*

class GroupPagerFragment : Fragment(), SearchResultsRecyclerViewAdapter.MemberInvitedListener {

    private var groupId: Int? = null
    private var groupName: String? = null
    private lateinit var pagerAdapter: GroupPagerAdapter
    private lateinit var viewModel: GroupViewModel
    private lateinit var searchView: SearchView

    private var tabs: ArrayList<String> = ArrayList()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setHasOptionsMenu(true)
    }

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?, savedInstanceState: Bundle?): View? {
        groupId = arguments?.getParcelable<GroupDataParc>(FragmentDataKeys.GROUP_PAGER_FRAGMENT.value)?.id
        groupName = arguments?.getParcelable<GroupDataParc>(FragmentDataKeys.GROUP_PAGER_FRAGMENT.value)?.name
        return inflater.inflate(R.layout.fragment_group, container, false)
    }

    override fun onCreateOptionsMenu(menu: Menu, inflater: MenuInflater) {
        inflater.inflate(R.menu.menu_group, menu)
        super.onCreateOptionsMenu(menu, inflater)

        val item = menu.findItem(R.id.menuSearch)
        searchView = SearchView((context as MainActivity).supportActionBar!!.themedContext)
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
                var filteredUsersSize = 0
                if (newText.length > 3) {
                    viewModel.getSearchedUsers(newText) { filteredUsers, error ->
                        filteredUsers?.let {
                            filteredUsersSize = it.size
                            val adapter = SearchResultsRecyclerViewAdapter(context!!, it, groupId!!, this@GroupPagerFragment, viewModel)
                            searchResultsRecyclerView?.adapter = adapter
                        }

                        if (filteredUsersSize > 0) {
                            searchResultsRecyclerView?.visible()
                            tabLayout?.invisible()
                        } else {
                            searchResultsRecyclerView?.invisible()
                            tabLayout?.visible()
                        }
                    }
                } else {
                    searchResultsRecyclerView?.invisible()
                    tabLayout?.visible()
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

        val removeMemberItem = menu.findItem(R.id.removeMember)
        val deleteGroupItem = menu.findItem(R.id.deleteGroup)

        groupId?.let {
            viewModel.getGroupData(it) { groupData, _ ->
                if (SharedPreferences.userId == groupData?.creator?.id) {
                    removeMemberItem.isVisible = true
                    deleteGroupItem.isVisible = true
                    item.isVisible = true
                } else {
                    item.isVisible = true
                }
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
                args.putInt(FragmentDataKeys.BILLS_FRAGMENT.value, -1)
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
            R.id.deleteGroup -> {
                DialogManager.confirmationDialog(getString(R.string.are_you_sure_to_delete), context) {
                    viewModel.deleteGroup(groupId!!) { response, error ->
                        if (error == null) {
                            DialogManager.showInfoDialog(
                                context?.getString(R.string.api_groups_delete_group_200), context
                            )
                            activity?.supportFragmentManager?.popBackStackImmediate()
                        } else {
                            DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS_DELETE, context), context)
                        }
                    }
                }
                return true
            }
            R.id.myDebts -> {
                val fragment = DebtsFragment()
                val args = Bundle()
                groupId?.let {
                    args.putInt(FragmentDataKeys.MEMBERS_FRAGMENT.value, it)
                }
                fragment.arguments = args
                (context as MainActivity).supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, fragment)
                    ?.addToBackStack(null)?.commit()
                return true
            }
            else ->
                return super.onOptionsItemSelected(item)
        }
    }

    override fun onInvited() {
        searchView.setQuery("", false)
        searchView.clearFocus()
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupViewModel::class.java)
        }

        groupName?.let { (activity as MainActivity).toolbar.title = it }

        if (tabs.isEmpty()) {
            context?.getString(R.string.members_tab)?.let { tabs.add(it) }
            context?.getString(R.string.bills_tab)?.let { tabs.add(it) }
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
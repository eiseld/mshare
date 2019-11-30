package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.design.widget.TabLayout
import android.support.v4.app.Fragment
import android.view.*
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.GroupDataParc
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.Adapter.GroupPagerAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.app_bar_main.*
import kotlinx.android.synthetic.main.fragment_group.*

class GroupPagerFragment : Fragment() {

    private var groupCreatorId: Int? = null
    private var groupId: Int? = null
    private var groupName: String? = null
    private lateinit var pagerAdapter: GroupPagerAdapter
    private lateinit var viewModel: GroupViewModel

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

        val removeMemberItem = menu.findItem(R.id.removeMember)
        val deleteGroupItem = menu.findItem(R.id.deleteGroup)
        val menuAddItem = menu.findItem(R.id.menuAdd)

        tabLayout.addOnTabSelectedListener(object : TabLayout.OnTabSelectedListener {
            override fun onTabSelected(tab: TabLayout.Tab?) {
                if (tab == tabLayout.getTabAt(0)) {
                    menuAddItem.isVisible = SharedPreferences.userId == groupCreatorId
                    removeMemberItem.isVisible = SharedPreferences.userId == groupCreatorId
                } else if (tab == tabLayout.getTabAt(2)) {
                    menuAddItem.isVisible = false
                } else {
                    menuAddItem.isVisible = true
                }
            }

            override fun onTabUnselected(tab: TabLayout.Tab?) {}

            override fun onTabReselected(tab: TabLayout.Tab?) {}
        })

        groupId?.let {
            viewModel.getGroupData(it) { groupData, error ->
                if (groupData != null) {
                    groupCreatorId = groupData.creator.id
                    menuAddItem.isVisible = SharedPreferences.userId == groupCreatorId
                    deleteGroupItem.isVisible = SharedPreferences.userId == groupCreatorId
                    removeMemberItem.isVisible = SharedPreferences.userId == groupCreatorId
                } else {
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS,context), context)
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
                args.putInt(FragmentDataKeys.ADD_SPENDING_FRAGMENT.value, -1)
                fragment.arguments = args
                (context as MainActivity).supportFragmentManager?.beginTransaction()
                    ?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()

                return true
            }
            R.id.menuAdd -> {
                if(tabLayout.getTabAt(0)?.isSelected!!) {
                    val fragment = AddMembersFragment()
                    val args = Bundle()
                    args.putInt(FragmentDataKeys.ADD_MEMBERS_FRAGMENT.value, groupId!!)
                    fragment.arguments = args
                    (context as MainActivity).supportFragmentManager?.beginTransaction()
                        ?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()
                } else if(tabLayout.getTabAt(1)?.isSelected!!) {
                    val fragment = AddSpendingFragment()
                    val args = Bundle()
                    groupId?.let { args.putInt(FragmentDataKeys.MEMBERS_FRAGMENT.value, it) }
                    args.putInt(FragmentDataKeys.ADD_SPENDING_FRAGMENT.value, -1)
                    fragment.arguments = args
                    (context as MainActivity).supportFragmentManager?.beginTransaction()
                        ?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()
                }

                return true
            }

            R.id.removeMember -> {
                viewModel.isDeleteMemberEnabled = !viewModel.isDeleteMemberEnabled

                //TODO REPLACE TO ENUM KEY
                (childFragmentManager.fragments.first { it is MembersFragment } as MembersFragment).adapter?.notifyDataSetChanged()
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

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

        activity?.let {
            viewModel = ViewModelProviders.of(it).get(GroupViewModel::class.java)
        }

        groupName?.let { (activity as MainActivity).toolbar.title = it }

        if (tabs.isEmpty()) {
            context?.getString(R.string.members_tab)?.let { tabs.add(it) }
            context?.getString(R.string.bills_tab)?.let { tabs.add(it) }
            context?.getString(R.string.history_tab)?.let { tabs.add(it) }
        }
        initViewPager()

    }

    fun initViewPager() {
        groupId?.let {
            pagerAdapter = GroupPagerAdapter(it, tabs, childFragmentManager)
            viewPager.adapter = pagerAdapter
            viewPager.offscreenPageLimit = 2
            tabLayout.setupWithViewPager(viewPager)
            tabLayout.getTabAt(0)?.select()
        }
    }
}
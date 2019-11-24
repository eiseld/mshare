package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.view.*
import elte.moneyshare.*
import elte.moneyshare.entity.GroupDataParc
import elte.moneyshare.view.Adapter.GroupPagerAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.app_bar_main.*
import kotlinx.android.synthetic.main.fragment_group.*

class GroupPagerFragment : Fragment() {

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

        groupId?.let {
            viewModel.getGroupData(it) { groupData, _ ->
                removeMemberItem.isVisible = SharedPreferences.userId == groupData?.creator?.id
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
                    groupId?.let {
                        args.putInt(FragmentDataKeys.MEMBERS_FRAGMENT.value, it)
                    }
                    args.putInt(FragmentDataKeys.BILLS_FRAGMENT.value, -1)
                    fragment.arguments = args
                    (context as MainActivity).supportFragmentManager?.beginTransaction()
                        ?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()
                }

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

        groupName?.let {
            activity?.actionBar?.title = it
            toolbar?.setTitle(it)
        }

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
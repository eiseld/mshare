package elte.moneyshare.view

import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.Fragment
import android.view.*
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.view.Adapter.GroupPagerAdapter
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.fragment_group.*

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
    }

    override fun onOptionsItemSelected(item: MenuItem?): Boolean {
        when (item?.itemId) {
            R.id.addSpending -> {
                val fragment = AddSpendingFragment()
                val args = Bundle()
                groupId?.let {
                    args.putInt(FragmentDataKeys.MEMBERS_FRAGMENT.value, it)
                }
                fragment.arguments = args
                (context as MainActivity).supportFragmentManager?.beginTransaction()
                    ?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()

                return true
            }
            R.id.addMember -> {
                return true
            }

            R.id.removeMember -> {
                viewModel.isDeleteMemberEnabled = !viewModel.isDeleteMemberEnabled
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

    private fun initViewPager() {
        groupId?.let {
            pagerAdapter = GroupPagerAdapter(it, tabs, childFragmentManager)
            viewPager.adapter = pagerAdapter
            viewPager.offscreenPageLimit = 1
            tabLayout.setupWithViewPager(viewPager)
            tabLayout.getTabAt(0)?.select()
        }
    }
}
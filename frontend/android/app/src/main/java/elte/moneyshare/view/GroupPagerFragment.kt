package elte.moneyshare.view

import android.os.Bundle
import android.support.v4.app.Fragment
import android.view.*
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.view.Adapter.GroupPagerAdapter
import kotlinx.android.synthetic.main.fragment_group.*

class GroupPagerFragment : Fragment() {

    private var groupId: Int? = null
    private lateinit var pagerAdapter: GroupPagerAdapter

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
        inflater.inflate(R.menu.menu, menu)
        super.onCreateOptionsMenu(menu, inflater)
    }

    //TODO items add member, add bill
    override fun onOptionsItemSelected(item: MenuItem?): Boolean {
        when (item?.itemId) {
            R.id.createGroup -> {
                return true
            }
            else ->
                return super.onOptionsItemSelected(item)
        }
    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)
        tabs.add("Members")
        tabs.add("Bills")
        initViewPager()
    }

    private fun initViewPager() {
        groupId?.let {
            pagerAdapter = GroupPagerAdapter(it, tabs, fragmentManager)
            viewPager.adapter = pagerAdapter
            viewPager.offscreenPageLimit = 1
            tabLayout.setupWithViewPager(viewPager)
            tabLayout.getTabAt(0)?.select()
        }
    }
}
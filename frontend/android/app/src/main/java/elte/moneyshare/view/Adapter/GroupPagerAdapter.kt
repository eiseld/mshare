package elte.moneyshare.view.Adapter

import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v4.app.FragmentManager
import android.support.v4.app.FragmentStatePagerAdapter
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.view.BillsFragment
import elte.moneyshare.view.GroupHistoryFragment
import elte.moneyshare.view.MembersFragment

class GroupPagerAdapter(var groupId: Int, var tabs: List<String>, fragmentManager: FragmentManager?) :
    FragmentStatePagerAdapter(fragmentManager) {

    /**
     * Return the Fragment associated with a specified position.
     */
    override fun getItem(position: Int): Fragment {
        return when (position) {
            0 -> {
                MembersFragment().apply {
                    arguments = Bundle().apply { putInt(FragmentDataKeys.MEMBERS_FRAGMENT.value, groupId) }
                }
            }
            1 -> {
                BillsFragment().apply {
                    arguments = Bundle().apply { putInt(FragmentDataKeys.BILLS_FRAGMENT.value, groupId) }
                }
            }
            2 -> {
                GroupHistoryFragment().apply {
                    arguments = Bundle().apply { putInt(FragmentDataKeys.GROUP_HISTORY_FRAGMENT.value, groupId) }
                }
            }
            else -> Fragment()
        }
    }

    /**
     * Return the number of views available.
     */
    override fun getCount(): Int {
        return tabs.size
    }

    override fun getPageTitle(position: Int): CharSequence? {
        return when (position) {
            0 -> tabs[0]
            1 -> tabs[1]
            2 -> tabs[2]
            else -> super.getPageTitle(position)
        }
    }
}
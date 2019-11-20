package elte.moneyshare.view.Adapter

import android.os.Bundle
import android.support.v4.app.FragmentManager
import android.support.v4.app.Fragment
import android.support.v4.app.FragmentStatePagerAdapter
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.view.BillsFragment
import elte.moneyshare.view.MembersFragment

class GroupPagerAdapter(var groupId: Int, var tabs: List<String>, fragmentManager: FragmentManager?) : FragmentStatePagerAdapter(fragmentManager) {

    /**
     * Return the Fragment associated with a specified position.
     */
    override fun getItem(position: Int): Fragment {
        var fragment = Fragment()
        val args = Bundle()
        args.putInt(FragmentDataKeys.MEMBERS_FRAGMENT.value, groupId)

        when(position) {
            0 -> {
                fragment = MembersFragment()
            }
            1 -> {
                fragment = BillsFragment()
            }
        }

        fragment.arguments = args
        return fragment
    }

    /**
     * Return the number of views available.
     */
    override fun getCount(): Int {
        return tabs.size
    }

    override fun getPageTitle(position: Int): CharSequence? {
        return when(position) {
            0 -> tabs[0]
            1 -> tabs[1]
            else -> super.getPageTitle(position)
        }
    }
}
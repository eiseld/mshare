package elte.moneyshare

import elte.moneyshare.view.GroupPagerFragment
import elte.moneyshare.view.MembersFragment

enum class FragmentDataKeys (val value: String) {
    GROUP_PAGER_FRAGMENT(GroupPagerFragment::class.java.name),
    MEMBERS_FRAGMENT(MembersFragment::class.java.name)
}
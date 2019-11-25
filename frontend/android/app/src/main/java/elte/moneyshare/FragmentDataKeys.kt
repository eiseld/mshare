package elte.moneyshare

import elte.moneyshare.view.*

enum class FragmentDataKeys (val value: String) {
    GROUP_PAGER_FRAGMENT(GroupPagerFragment::class.java.name),
    GROUP_NAME_GROUP_PAGER_FRAGMENT(GroupPagerFragment::class.java.name),
    MEMBERS_FRAGMENT(MembersFragment::class.java.name),
    ADD_MEMBERS_FRAGMENT(AddMembersFragment::class.java.name),
    BILLS_FRAGMENT(AddSpendingFragment::class.java.name),
    NEW_PASSWORD_TOKEN(NewPasswordFragment::class.java.name)
}
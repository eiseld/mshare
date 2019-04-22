package elte.moneyshare.view.Adapter

import android.content.Context
import android.os.Bundle
import android.support.v4.app.Fragment
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.ViewGroup
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.entity.Group
import elte.moneyshare.entity.GroupInfo
import elte.moneyshare.view.GroupPagerFragment
import elte.moneyshare.view.MainActivity
import elte.moneyshare.view.viewholder.GroupViewHolder

class GroupsRecyclerViewAdapter(private val context: Context, private val groups: List<GroupInfo>): RecyclerView.Adapter<GroupViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): GroupViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_group, parent, false)
        return GroupViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return groups.size
    }

    override fun onBindViewHolder(holder: GroupViewHolder, position: Int) {
        val group = groups[position]
        holder.groupNameTextView.text = group.name
        holder.groupOwnerTextView.text = group.creator
        holder.groupMemberCountTextView.text = String.format(context.getString(R.string.group_members), group.memberCount)

        if (group.myCurrentBalance < 0) {
            holder.groupBalanceTextView.text = String.format(context.getString(R.string.group_owned), group.myCurrentBalance)
        } else if (group.myCurrentBalance > 0) {
            holder.groupBalanceTextView.text = String.format(context.getString(R.string.group_owe), group.myCurrentBalance)
        } else {
            holder.groupBalanceTextView.text = context.getString(R.string.group_settled_up)
        }

        holder.groupRootLayout.setOnClickListener {
            val fragment = GroupPagerFragment()
            val args = Bundle()
            args.putInt(FragmentDataKeys.GROUP_PAGER_FRAGMENT.value, group.id)
            fragment.arguments = args
            (context as MainActivity).supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()
        }
    }
}

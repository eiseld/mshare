package elte.moneyshare.view.Adapter

import android.content.Context
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.entity.Group
import elte.moneyshare.view.viewholder.GroupViewHolder

class GroupsRecyclerViewAdapter(private val context: Context, private val groups: List<Group>): RecyclerView.Adapter<GroupViewHolder>() {

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
        //TODO OWN LOGIC
        holder.groupBalanceTextView.text = String.format(context.getString(R.string.group_owe), group.balance)
    }
}
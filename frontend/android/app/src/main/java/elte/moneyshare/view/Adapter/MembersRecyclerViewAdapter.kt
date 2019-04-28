package elte.moneyshare.view.Adapter

import android.content.Context
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.entity.Group
import elte.moneyshare.entity.GroupData
import elte.moneyshare.view.viewholder.MemberViewHolder

class MembersRecyclerViewAdapter(private val context: Context, private val groupData: GroupData): RecyclerView.Adapter<MemberViewHolder>()  {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): MemberViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_member, parent, false)
        return MemberViewHolder(itemView)
    }


    override fun getItemCount(): Int {
        return groupData.memberCount
    }

    override fun onBindViewHolder(holder: MemberViewHolder, position: Int) {
        val member = groupData.members[position]
        holder.memberNameTextView.text = member.name
        holder.memberBalanceTextView.text = member.balance.toString()

        if (member.balance < 0) {
            holder.memberBalanceTextView.text = String.format(context.getString(R.string.group_owned), member.balance)
        } else if (member.balance > 0) {
            holder.memberBalanceTextView.text = String.format(context.getString(R.string.group_owe), member.balance)
        } else {
            holder.memberBalanceTextView.text = context.getString(R.string.group_settled_up)
        }

    }
}
package elte.moneyshare.view.Adapter

import android.content.Context
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.GroupData
import elte.moneyshare.gone
import elte.moneyshare.view.viewholder.MemberViewHolder
import elte.moneyshare.viewmodel.GroupsViewModel

class MembersRecyclerViewAdapter(private val context: Context, private val groupData: GroupData, private val Model : GroupsViewModel): RecyclerView.Adapter<MemberViewHolder>()  {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): MemberViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_member, parent, false)
        return MemberViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return groupData.members.size
    }

    override fun onBindViewHolder(holder: MemberViewHolder, position: Int) {
        val member = groupData.members[position]

        if(member.id == SharedPreferences.userId) {
            holder.memberRootLayout.gone()
        }

        holder.memberNameTextView.text = member.name
        holder.memberBalanceTextView.text = member.balance.toString()
        //TODO real creator

        if (member.balance < 0) {
            holder.memberBalanceTextView.text = String.format(context.getString(R.string.group_owned), member.balance)
        } else if (member.balance > 0) {
            holder.memberBalanceTextView.text = String.format(context.getString(R.string.group_owe), member.balance)
        } else {
            holder.memberBalanceTextView.text = context.getString(R.string.group_settled_up)
        }

        if(groupData.creator.id == SharedPreferences.userId) {
            holder.removeButton.visibility = View.VISIBLE
        }

        holder.removeButton.setOnClickListener()
        {
            val memberId = member.id
            Model.deleteMember(groupData.id ,member.id) { response, error ->
                if(error == null) {
                    val index = groupData.members.indexOf(member)
                    notifyItemRemoved(index)
                    groupData.members.removeAt(index)
                    Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                } else {
                    Toast.makeText(context, error, Toast.LENGTH_SHORT).show()
                }
            }
        }
    }
}
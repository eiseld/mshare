package elte.moneyshare.view.Adapter

import android.content.Context
import android.support.v7.app.AlertDialog
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.ViewGroup
import android.widget.*
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.GroupData
import elte.moneyshare.gone
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.viewholder.MemberViewHolder
import elte.moneyshare.viewmodel.GroupViewModel
import elte.moneyshare.visible

class MembersRecyclerViewAdapter(private val context: Context, private val groupData: GroupData, private val model : GroupViewModel): RecyclerView.Adapter<MemberViewHolder>()  {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): MemberViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_member, parent, false)
        return MemberViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return groupData.members.size
    }

    override fun onBindViewHolder(holder: MemberViewHolder, position: Int) {
        val member = groupData.members[position]
        val loggedInUserId = SharedPreferences.userId

        holder.memberNameTextView.text = member.name

        if (member.balance < 0) {
            holder.memberBalanceTextView.text = String.format(context.getString(R.string.member_owed), member.balance)
        } else if (member.balance > 0) {
            holder.memberBalanceTextView.text = String.format(context.getString(R.string.member_owe), member.balance)
        } else {
            holder.memberBalanceTextView.text = context.getString(R.string.group_settled_up)
        }

        if (groupData.creator.id == member.id) {
            holder.memberOwnerTextView.visible()
        } else {
            holder.memberOwnerTextView.gone()
        }

        if (groupData.creator.id == loggedInUserId && model.isDeleteMemberEnabled) {
            holder.removeButton.visible()
        } else {
            holder.removeButton.gone()
        }

        holder.removeButton.setOnClickListener()
        {
            model.deleteMember(groupData.id ,member.id) { response, error ->
                if(error == null) {
                    val index = groupData.members.indexOf(member)
                    notifyItemRemoved(index)
                    groupData.members.removeAt(index)
                } else {
                    DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS,context), context)
                }
            }
        }
    }
}
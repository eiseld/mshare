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
import elte.moneyshare.view.viewholder.MemberViewHolder
import elte.moneyshare.viewmodel.GroupViewModel
import elte.moneyshare.visible

class MembersRecyclerViewAdapter(private val context: Context, private val groupData: GroupData, private val Model : GroupViewModel): RecyclerView.Adapter<MemberViewHolder>()  {

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
        holder.memberBalanceTextView.text = member.balance.toString()

        if (groupData.creator.id == member.id) {
            holder.memberOwnerTextView.visible()
        } else {
            holder.memberOwnerTextView.gone()
        }

        if(groupData.creator.id == loggedInUserId && SharedPreferences.isDeleteMemberEnabled) {
            holder.removeButton.visible()
        } else {
            holder.removeButton.gone()
        }

        holder.removeButton.setOnClickListener()
        {
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

        holder.debitButton.setOnClickListener()
        {
            val builder = AlertDialog.Builder(context)
            builder.setTitle(context.getString(R.string.popup_title))
            builder.setMessage(context.getString(R.string.popup_message))

            builder.setPositiveButton(context.getString(R.string.yes)) { dialog, which ->
                if (member.balance < 0) {
                    Model.doDebitEqualization(groupData.id, loggedInUserId, member.id) { response, error ->
                        if (error == null) {
                            val index = groupData.members.indexOf(member)
                            notifyItemChanged(index)
                            Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                        } else {
                            Toast.makeText(context, error, Toast.LENGTH_SHORT).show()
                        }
                    }
                } else if (member.balance > 0) {
                    Model.doDebitEqualization(groupData.id, member.id, loggedInUserId) { response, error ->
                        if (error == null) {
                            val index = groupData.members.indexOf(member)
                            notifyItemChanged(index)
                            Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                        } else {
                            Toast.makeText(context, error, Toast.LENGTH_SHORT).show()
                        }
                    }
                }
                builder.setNeutralButton(context.getString(R.string.no)) { _, _ ->
                }
                val dialog: AlertDialog = builder.create()
                dialog.show()
            }
        }
    }
}
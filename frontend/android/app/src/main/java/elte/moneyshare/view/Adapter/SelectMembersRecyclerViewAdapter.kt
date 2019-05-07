package elte.moneyshare.view.Adapter

import android.content.Context
import android.support.v4.content.ContextCompat
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.entity.Member
import elte.moneyshare.view.viewholder.SelectMemberViewHolder

class SelectMembersRecyclerViewAdapter(private val context: Context, private val members: ArrayList<Member>): RecyclerView.Adapter<SelectMemberViewHolder>()  {

    var selectedMemberIds: ArrayList<Int> = ArrayList()

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): SelectMemberViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_select_member, parent, false)
        return SelectMemberViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return members.size
    }

    override fun onBindViewHolder(holder: SelectMemberViewHolder, position: Int) {
        val member = members[position]

        if (selectedMemberIds.contains(member.id)) {
            holder.memberRootLayout.background = ContextCompat.getDrawable(context, R.color.colorSubBackground)
        } else {
            holder.memberRootLayout.background = ContextCompat.getDrawable(context, R.color.colorBackground)
        }

        holder.memberNameTextView.text = member.name

        holder.memberRootLayout.setOnClickListener {
            if (selectedMemberIds.contains(member.id)) {
                selectedMemberIds.remove(member.id)
                holder.memberRootLayout.background = ContextCompat.getDrawable(context, R.color.colorBackground)
            } else {
                selectedMemberIds.add(member.id)
                holder.memberRootLayout.background = ContextCompat.getDrawable(context, R.color.colorSubBackground)
            }
        }
    }

    fun getSelectedMembersIds(): List<Int> = selectedMemberIds

}
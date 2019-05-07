package elte.moneyshare.view.viewholder

import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.TextView
import elte.moneyshare.R

class GroupViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {

    var groupNameTextView: TextView = itemView.findViewById(R.id.groupNameTextView)
    var groupMemberCountTextView: TextView = itemView.findViewById(R.id.groupMemberCountTextView)
    var groupOwnerTextView: TextView = itemView.findViewById(R.id.groupOwnerTextView)
    var groupBalanceTextView: TextView = itemView.findViewById(R.id.groupBalanceTextView)
}
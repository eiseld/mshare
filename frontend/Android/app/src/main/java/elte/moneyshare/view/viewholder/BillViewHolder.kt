package elte.moneyshare.view.viewholder

import android.support.constraint.ConstraintLayout
import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.TextView
import elte.moneyshare.R

class BillViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {

    var billRootConstraintLayout: ConstraintLayout = itemView.findViewById(R.id.billRootConstraintLayout)
    var billNameTextView: TextView = itemView.findViewById(R.id.billNameTextView)
    var billMoneyTextView: TextView = itemView.findViewById(R.id.billMoneyTextView)
    var billMembersRecyclerView: RecyclerView = itemView.findViewById(R.id.billMembersRecyclerView)
}
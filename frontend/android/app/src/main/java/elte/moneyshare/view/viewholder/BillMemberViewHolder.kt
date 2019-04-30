package elte.moneyshare.view.viewholder

import android.support.constraint.ConstraintLayout
import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.TextView
import elte.moneyshare.R

class BillMemberViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {

    var billMemberRootLayout: ConstraintLayout = itemView.findViewById(R.id.billMemberRootLayout)
    var billMemberNameTextView: TextView = itemView.findViewById(R.id.billMemberNameTextView)
    var billMemberMoneyTextView: TextView = itemView.findViewById(R.id.billMemberMoneyTextView)
}
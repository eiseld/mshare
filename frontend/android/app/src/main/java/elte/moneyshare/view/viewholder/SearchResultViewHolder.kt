package elte.moneyshare.view.viewholder

import android.support.constraint.ConstraintLayout
import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.Button
import android.widget.TextView
import elte.moneyshare.R

class SearchResultViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {

    var rootConstraintLayout: ConstraintLayout = itemView.findViewById(R.id.rootConstraintLayout)
    var nameTextView: TextView = itemView.findViewById(R.id.nameTextView)
    var emailTextView: TextView = itemView.findViewById(R.id.emailTextView)
    var inviteButton: Button = itemView.findViewById(R.id.inviteButton)
}
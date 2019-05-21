package elte.moneyshare.view.viewholder

import android.support.constraint.ConstraintLayout
import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.TextView
import elte.moneyshare.R

class SearchResultViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {

    var rootConstraintLayout: ConstraintLayout = itemView.findViewById(R.id.rootConstraintLayout)
    var nameEmailTextView: TextView = itemView.findViewById(R.id.nameEmailTextView)
}
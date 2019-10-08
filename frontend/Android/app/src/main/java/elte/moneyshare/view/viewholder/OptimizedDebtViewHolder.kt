package elte.moneyshare.view.viewholder

import android.support.constraint.ConstraintLayout
import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.Button
import android.widget.TextView
import elte.moneyshare.R

class OptimizedDebtViewHolder (itemView: View) : RecyclerView.ViewHolder(itemView) {

    var debtRootLayout: ConstraintLayout = itemView.findViewById(R.id.debtRootLayout)
    var debtNameTextView: TextView = itemView.findViewById(R.id.debtNameTextView)
    var debtOwnerTextView: TextView = itemView.findViewById(R.id.debtOwnerTextView)
    var debtBalanceTextView: TextView = itemView.findViewById(R.id.debtBalanceTextView)
    var debitButton : Button = itemView.findViewById(R.id.solveDebitImageButton)
}
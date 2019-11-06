package elte.moneyshare.view.viewholder

import android.support.constraint.ConstraintLayout
import android.support.v7.widget.RecyclerView
import android.view.View
import android.widget.ImageButton
import android.widget.TextView
import elte.moneyshare.R

class MemberViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView) {

    var memberRootLayout: ConstraintLayout = itemView.findViewById(R.id.memberRootLayout)
    var memberNameTextView: TextView = itemView.findViewById(R.id.memberNameTextView)
    var memberOwnerTextView: TextView = itemView.findViewById(R.id.memberOwnerTextView)
    var memberBalanceTextView: TextView = itemView.findViewById(R.id.memberBalanceTextView)
    var removeButton : ImageButton = itemView.findViewById(R.id.removeMemberImageButton)
    var memberBankAccountTextView: TextView = itemView.findViewById(R.id.memberBankAccountTextView)


}
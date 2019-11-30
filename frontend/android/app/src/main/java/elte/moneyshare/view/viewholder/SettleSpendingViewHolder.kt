package elte.moneyshare.view.viewholder

import android.view.View
import elte.moneyshare.R
import elte.moneyshare.view.Adapter.BaseViewHolder
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.list_item_history_settle_spending.view.*

class SettleSpendingViewHolder(itemView: View) : BaseViewHolder<GroupViewModel.HistoryItem.SettleSpending>(itemView) {
    override fun bind(item: GroupViewModel.HistoryItem.SettleSpending) {
        itemView.dateTextView.text = item.date
        itemView.creatorTextView.text = item.creator
        itemView.typeTextView.text = item.type
        itemView.fromNameTextView.text = item.settleFromName
        itemView.toNameTextView.text = item.settleToName
        itemView.spendingValueTextView.text = itemView.context.getString(R.string.bill_money, item.settleValue)
    }
}
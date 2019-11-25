package elte.moneyshare.view.viewholder

import android.view.View
import elte.moneyshare.view.Adapter.BaseViewHolder
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.list_item_history_add_or_remove_spending.view.*

class AddOrRemoveSpendingViewHolder(itemView: View) : BaseViewHolder<GroupViewModel.HistoryItem.AddOrRemoveSpending>(itemView) {
    override fun bind(item: GroupViewModel.HistoryItem.AddOrRemoveSpending) {
        itemView.dateTextView.text = item.date
        itemView.creatorTextView.text = item.creator
        itemView.typeTextView.text = item.type
        itemView.spendingNameTextView.text = item.spendingName
        itemView.spendingValueTextView.text = item.spendingValue.toString()
    }
}
package elte.moneyshare.view.viewholder

import android.view.View
import elte.moneyshare.view.Adapter.BaseViewHolder
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.list_item_history_modify_spending.view.*

class ModifySpendingViewHolder(itemView: View) : BaseViewHolder<GroupViewModel.HistoryItem.ModifySpending>(itemView) {
    override fun bind(item: GroupViewModel.HistoryItem.ModifySpending) {
        itemView.dateTextView.text = item.date
        itemView.creatorTextView.text = item.creator
        itemView.typeTextView.text = item.type
    }
}
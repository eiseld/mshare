package elte.moneyshare.view.viewholder

import android.view.View
import elte.moneyshare.R
import elte.moneyshare.gone
import elte.moneyshare.view.Adapter.BaseViewHolder
import elte.moneyshare.viewmodel.GroupViewModel
import elte.moneyshare.visible
import kotlinx.android.synthetic.main.list_item_history_modify_spending.view.*

class ModifySpendingViewHolder(itemView: View) : BaseViewHolder<GroupViewModel.HistoryItem.ModifySpending>(itemView) {
    override fun bind(item: GroupViewModel.HistoryItem.ModifySpending) {
        itemView?.apply {
            dateTextView.text = item.date
            creatorTextView.text = item.creator
            typeTextView.text = item.type

            item.nameOld?.let {
                spendingNameTextView.visible()
                spendingNameTextView.text = context.getString(R.string.history_spending_name_modify, item.nameOld, item.nameNew)
            } ?: spendingNameTextView.gone()

            if (item.moneyOld != item.moneyNew && item.moneyOld != null && item.moneyNew != null) {
                spendingValueTextView.visible()
                spendingValueTextView.text = context.getString(R.string.history_spending_money_modify, item.moneyOld, item.moneyNew)
            } else {
                spendingValueTextView.gone()
            }

            item.dateOld?.let {
                spendingDateTextView.visible()
                spendingDateTextView.text = context.getString(R.string.history_spending_date_modify, item.dateOld, item.dateNew)
            } ?: spendingDateTextView.gone()
        }
    }
}
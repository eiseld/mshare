package elte.moneyshare.view.Adapter

import android.content.Context
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.view.viewholder.AddOrRemoveMemberViewHolder
import elte.moneyshare.view.viewholder.AddOrRemoveSpendingViewHolder
import elte.moneyshare.view.viewholder.ModifySpendingViewHolder
import elte.moneyshare.view.viewholder.SettleSpendingViewHolder
import elte.moneyshare.viewmodel.GroupViewModel.HistoryItem

abstract class BaseViewHolder<T>(itemView: View) : RecyclerView.ViewHolder(itemView) {
    abstract fun bind(item: T)
}

class GroupHistoryRecyclerViewAdapter(
    private val context: Context,
    private val historyItems: List<HistoryItem>
) : RecyclerView.Adapter<BaseViewHolder<*>>() {

    companion object {
        private const val TYPE_ADD_OR_REMOVE_MEMBER = 0
        private const val TYPE_ADD_OR_REMOVE_SPENDING = 1
        private const val TYPE_MODIFY_SPENDING = 2
        private const val TYPE_SETTLE_SPENDING = 3
    }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): BaseViewHolder<*> {
        return when (viewType) {
            TYPE_ADD_OR_REMOVE_MEMBER -> {
                AddOrRemoveMemberViewHolder(
                    LayoutInflater.from(context).inflate(R.layout.list_item_history_add_or_remove_member, parent, false)
                )
            }
            TYPE_ADD_OR_REMOVE_SPENDING -> {
                AddOrRemoveSpendingViewHolder(
                    LayoutInflater.from(context).inflate(R.layout.list_item_history_add_or_remove_spending, parent, false)
                )
            }
            TYPE_MODIFY_SPENDING -> {
                ModifySpendingViewHolder(
                    LayoutInflater.from(context).inflate(R.layout.list_item_history_modify_spending, parent, false)
                )
            }
            TYPE_SETTLE_SPENDING -> {
                SettleSpendingViewHolder(
                    LayoutInflater.from(context).inflate(R.layout.list_item_history_settle_spending, parent, false)
                )
            }
            else -> throw IllegalArgumentException("Invalid view type")
        }
    }


    override fun onBindViewHolder(holder: BaseViewHolder<*>, position: Int) {
        val item = historyItems[position]
        when (holder) {
            is AddOrRemoveMemberViewHolder -> holder.bind(item as HistoryItem.AddOrRemoveMember)
            is AddOrRemoveSpendingViewHolder -> holder.bind(item as HistoryItem.AddOrRemoveSpending)
            is ModifySpendingViewHolder -> holder.bind(item as HistoryItem.ModifySpending)
            is SettleSpendingViewHolder -> holder.bind(item as HistoryItem.SettleSpending)
            else -> throw IllegalArgumentException()
        }
    }

    override fun getItemViewType(position: Int): Int =
        when (historyItems[position]) {
            is HistoryItem.AddOrRemoveMember -> TYPE_ADD_OR_REMOVE_MEMBER
            is HistoryItem.AddOrRemoveSpending -> TYPE_ADD_OR_REMOVE_SPENDING
            is HistoryItem.ModifySpending -> TYPE_MODIFY_SPENDING
            is HistoryItem.SettleSpending -> TYPE_SETTLE_SPENDING
        }

    override fun getItemCount(): Int = historyItems.size
}
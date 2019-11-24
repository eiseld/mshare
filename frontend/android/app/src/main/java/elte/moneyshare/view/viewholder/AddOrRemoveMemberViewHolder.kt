package elte.moneyshare.view.viewholder

import android.view.View
import elte.moneyshare.view.Adapter.BaseViewHolder
import elte.moneyshare.viewmodel.GroupViewModel
import kotlinx.android.synthetic.main.list_item_history_add_or_remove_member.view.*

class AddOrRemoveMemberViewHolder(itemView: View) : BaseViewHolder<GroupViewModel.HistoryItem.AddOrRemoveMember>(itemView) {
    override fun bind(item: GroupViewModel.HistoryItem.AddOrRemoveMember) {
        itemView.dateTextView.text = item.date
        itemView.creatorTextView.text = item.creator
        itemView.typeTextView.text = item.type
        itemView.affectedMemberTextView.text = item.memberName
    }
}
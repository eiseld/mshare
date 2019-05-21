package elte.moneyshare.view.Adapter

import android.content.Context
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.entity.DebtorData
import elte.moneyshare.fitWidthToScreen
import elte.moneyshare.view.viewholder.BillMemberViewHolder

class BillMembersRecyclerViewAdapter(private val context: Context, private val billMembers: List<DebtorData>) : RecyclerView.Adapter<BillMemberViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): BillMemberViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_bill_member, parent, false)
        return BillMemberViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return billMembers.size
    }

    override fun onBindViewHolder(holder: BillMemberViewHolder, position: Int) {
        holder.billMemberRootLayout.fitWidthToScreen(context)

        val billMember = billMembers[position]
        holder.billMemberNameTextView.text = billMember.name
        holder.billMemberMoneyTextView.text = String.format(context.getString(R.string.bill_money, billMember.debt))
    }
}
package elte.moneyshare.view.Adapter

import android.content.Context
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.OptimizedDebtData
import elte.moneyshare.view.viewholder.OptimizedDebtViewHolder
import elte.moneyshare.viewmodel.GroupsViewModel

class OptimizedDebtRecyclerViewAdapter(
    private val context: Context,
    private val DebtData: ArrayList<OptimizedDebtData>,
    private val Model: GroupsViewModel
) : RecyclerView.Adapter<OptimizedDebtViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): OptimizedDebtViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_optdebt, parent, false)
        return OptimizedDebtViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return DebtData.count()
    }

    override fun onBindViewHolder(holder: OptimizedDebtViewHolder, position: Int) {
        val debt = DebtData[position]
        holder.debtBalanceTextView.text = debt.optimisedDebtAmount.toString()
        if (debt.creditor.id == SharedPreferences.userId) {
            holder.debtNameTextView.text = debt.debtor.name
            holder.debtBalanceTextView.text = String.format(context.getString(R.string.group_owe), debt.optimisedDebtAmount)
        }
        else if (debt.debtor.id == SharedPreferences.userId) {
            holder.debtNameTextView.text = debt.creditor.name
            holder.debtBalanceTextView.text = String.format(context.getString(R.string.group_owned), debt.optimisedDebtAmount)
        }
        else {
            holder.debtRootLayout.visibility = View.GONE
        }
    }
}
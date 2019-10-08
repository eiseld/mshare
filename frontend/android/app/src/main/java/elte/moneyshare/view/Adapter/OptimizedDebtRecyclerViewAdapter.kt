package elte.moneyshare.view.Adapter

import android.content.Context
import android.support.v7.app.AlertDialog
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import elte.moneyshare.R
import elte.moneyshare.SharedPreferences
import elte.moneyshare.entity.OptimizedDebtData
import elte.moneyshare.gone
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.viewholder.OptimizedDebtViewHolder
import elte.moneyshare.viewmodel.GroupViewModel
import elte.moneyshare.visible

class OptimizedDebtRecyclerViewAdapter(
    private val context: Context,
    private var debtDataList: ArrayList<OptimizedDebtData>,
    private val Model: GroupViewModel,
    private val groupId: Int
) : RecyclerView.Adapter<OptimizedDebtViewHolder>() {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): OptimizedDebtViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_optdebt, parent, false)
        return OptimizedDebtViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return debtDataList.size
    }

    override fun onBindViewHolder(holder: OptimizedDebtViewHolder, position: Int) {
        val debt = debtDataList[position]
        holder.debtBalanceTextView.text = debt.optimisedDebtAmount.toString()
        if (debt.debtor.id == SharedPreferences.userId) {
            holder.debtNameTextView.text = debt.debtor.name
            holder.debtBalanceTextView.text = String.format(context.getString(R.string.group_owe), debt.optimisedDebtAmount)
        } else if (debt.creditor.id == SharedPreferences.userId) {
            holder.debtNameTextView.text = debt.debtor.name
            holder.debtBalanceTextView.text = String.format(context.getString(R.string.group_owned), debt.optimisedDebtAmount)
        } else {
            holder.debtRootLayout.visibility = View.GONE
        }

        if (debt.optimisedDebtAmount != 0L) {
            holder.debitButton.visible()
        } else {
            holder.debitButton.gone()
        }

        holder.debitButton.setOnClickListener()
        {
            val builder = AlertDialog.Builder(context)
            builder.setTitle(context.getString(R.string.popup_title))
            if (debt.creditor.id == SharedPreferences.userId) {
                val msg = String.format(
                    context.getString(R.string.popup_message_to_you),
                    debt.debtor.name,
                    debt.optimisedDebtAmount
                )
                builder.setMessage(msg)
            } else {
                val msg = String.format(
                    context.getString(R.string.popup_message_from_you),
                    debt.optimisedDebtAmount,
                    debt.creditor.name
                )
                builder.setMessage(msg)
            }

            //TODO NOTIFYITEMREMOVED
            builder.setPositiveButton(context.getString(R.string.yes)) { dialog, which ->
                Model.doDebitEqualization(groupId, debt.debtor.id, debt.creditor.id) { response, error ->
                    if (error == null) {
                        Model.getOptimizedDebtData(groupId) { groupData, error ->
                            if (groupData != null) {
                                debtDataList = groupData
                                notifyDataSetChanged()
                            } else {
                                Toast.makeText(context, error.toString(), Toast.LENGTH_SHORT).show()
                            }
                        }
                        Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.GROUPS,context), context)
                    }
                }
            }
            builder.setNeutralButton(context.getString(R.string.no)) { _, _ ->
            }
            val dialog: AlertDialog = builder.create()
            dialog.show()

        }
    }
}
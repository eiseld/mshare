package elte.moneyshare.view.Adapter

import android.animation.AnimatorSet
import android.animation.ValueAnimator
import android.content.Context
import android.os.Bundle
import android.support.v7.widget.LinearLayoutManager
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.animation.AccelerateDecelerateInterpolator
import android.widget.LinearLayout
import elte.moneyshare.*
import elte.moneyshare.entity.SpendingData
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.util.Action
import elte.moneyshare.util.convertErrorCodeToString
import elte.moneyshare.view.AddSpendingFragment
import elte.moneyshare.view.MainActivity
import elte.moneyshare.view.viewholder.BillViewHolder
import elte.moneyshare.viewmodel.GroupViewModel

class BillsRecyclerViewAdapter(
    private val context: Context,
    private val bills: MutableList<SpendingData>,
    private val groupId: Int,
    private val model: GroupViewModel
) : RecyclerView.Adapter<BillViewHolder>() {

    private var animationDuration = 300L

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): BillViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_bill, parent, false)
        return BillViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return bills.size
    }

    override fun onBindViewHolder(holder: BillViewHolder, position: Int) {
        val bill = bills[position]
        holder.billNameTextView.text = bill.name
        holder.billDateTextView.text = bill.date
        holder.billMoneyTextView.text = String.format(context.getString(R.string.bill_money, bill.moneyOwed))


        if (bill.creditor.id == SharedPreferences.userId) {
            holder.removeBillImageButtonView.visibility = View.VISIBLE
        } else {
            holder.removeBillImageButtonView.visibility = View.GONE
        }

        holder.billMembersRecyclerView.layoutManager = LinearLayoutManager(context, LinearLayout.VERTICAL, false)
        holder.billMembersRecyclerView.isNestedScrollingEnabled = false
        holder.billMembersRecyclerView.adapter = BillMembersRecyclerViewAdapter(context, bill.debtors)

        holder.billRootConstraintLayout.setOnClickListener {

            if (holder.billMembersRecyclerView?.alpha == 0f) {
                holder.billMembersRecyclerView?.alpha = 1f
                holder.billMembersRecyclerView?.visible()

                holder.billMembersRecyclerView.measure()

                val openedHeight = holder.billMembersRecyclerView.measuredHeight ?: 0
                val valueAnimatorHeight = ValueAnimator.ofInt(1, openedHeight).setDuration(animationDuration)
                valueAnimatorHeight.addUpdateListener { animation ->
                    holder.billMembersRecyclerView?.layoutParams?.height = animation.animatedValue as Int
                    holder.billMembersRecyclerView?.requestLayout()
                }
                val animatorSet = AnimatorSet()
                animatorSet.play(valueAnimatorHeight)
                animatorSet.interpolator = AccelerateDecelerateInterpolator()
                animatorSet.start()

            } else {

                holder.billMembersRecyclerView.measure()
                val valueAnimatorHeight = ValueAnimator.ofInt(holder.billMembersRecyclerView?.height ?: 0, 1).setDuration(animationDuration)
                valueAnimatorHeight.addUpdateListener { animation ->
                    holder.billMembersRecyclerView?.layoutParams?.height = animation.animatedValue as Int
                    holder.billMembersRecyclerView?.requestLayout()

                    if (animation.animatedValue == 1) {
                        holder.billMembersRecyclerView?.alpha = 0f
                        holder.billMembersRecyclerView?.gone()
                    }
                }

                val animatorSet = AnimatorSet()
                animatorSet.play(valueAnimatorHeight)
                animatorSet.interpolator = AccelerateDecelerateInterpolator()
                animatorSet.start()
            }
        }
        if (bills[position].creditorUserId == SharedPreferences.userId) {
            holder.billRootConstraintLayout.setOnLongClickListener {
                val id = bills[position].id
                showModifyDialog(id)
                return@setOnLongClickListener true
            }
        }

        holder.removeBillImageButtonView.setOnClickListener()
        {
            DialogManager.confirmationDialog(context.getString(R.string.doYouReallyWantToDeleteSpending), context) {
                model.deleteSpending(bill.id, groupId) { response, error ->
                    if (error == null) {
                        bills.removeAt(position)
                        notifyItemRemoved(position)
                        DialogManager.showInfoDialog(context.getString(R.string.spendingSuccessfullyDeleted), context)
                    } else {
                        DialogManager.showInfoDialog(error.convertErrorCodeToString(Action.SPENDING_DELETE, context), context)
                    }
                }
            }
        }
    }

    private fun showModifyDialog(id: Int) {
        DialogManager.confirmationDialog(
            message = context.getString(R.string.confirm_spending_modification),
            title = context.getString(R.string.modify_spending),
            context = context
        ) {
            val fragment = AddSpendingFragment()
            val args = Bundle()
            args.putInt(FragmentDataKeys.MEMBERS_FRAGMENT.value, groupId)
            args.putInt(FragmentDataKeys.ADD_SPENDING_FRAGMENT.value, id)
            fragment.arguments = args
            (context as MainActivity).supportFragmentManager?.beginTransaction()
                ?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()
        }
    }
}
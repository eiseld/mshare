package elte.moneyshare.view.Adapter

import android.animation.AnimatorSet
import android.animation.ValueAnimator
import android.content.Context
import android.support.v7.widget.LinearLayoutManager
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.ViewGroup
import android.view.animation.AccelerateDecelerateInterpolator
import android.widget.LinearLayout
import elte.moneyshare.R
import elte.moneyshare.entity.SpendingData
import elte.moneyshare.gone
import elte.moneyshare.measure
import elte.moneyshare.view.viewholder.BillViewHolder
import elte.moneyshare.visible

class BillsRecyclerViewAdapter(private val context: Context, private val bills: List<SpendingData>) : RecyclerView.Adapter<BillViewHolder>() {

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
        holder.billMoneyTextView.text = String.format(context.getString(R.string.bill_money, bill.moneyOwed))


        holder.billMembersRecyclerView.layoutManager = LinearLayoutManager(context, LinearLayout.VERTICAL, false)
        holder.billMembersRecyclerView.isNestedScrollingEnabled = false
        holder.billMembersRecyclerView.adapter = BillMembersRecyclerViewAdapter(context, bill.debtors)

        holder.billRootConstraintLayout.setOnClickListener {

            if (holder.billMembersRecyclerView?.alpha == 0f){
                holder.billMembersRecyclerView?.alpha = 1f
                holder.billMembersRecyclerView?.visible()

                holder.billMembersRecyclerView.measure()

                val openedHeight = holder.billMembersRecyclerView.measuredHeight ?: 0
                val valueAnimatorHeight = ValueAnimator.ofInt(1, openedHeight).setDuration(animationDuration)
                valueAnimatorHeight.addUpdateListener { animation ->
                    holder.billMembersRecyclerView.layoutParams?.height = animation.animatedValue as Int
                    holder.billMembersRecyclerView.requestLayout()
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
                        holder.billMembersRecyclerView.alpha = 0f
                        holder.billMembersRecyclerView.gone()
                    }
                }

                val animatorSet = AnimatorSet()
                animatorSet.play(valueAnimatorHeight)
                animatorSet.interpolator = AccelerateDecelerateInterpolator()
                animatorSet.start()
            }
        }
    }
}
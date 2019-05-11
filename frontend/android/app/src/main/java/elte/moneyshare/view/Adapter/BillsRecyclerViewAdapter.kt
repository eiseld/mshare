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
import android.widget.Toast
import elte.moneyshare.entity.SpendingData
import elte.moneyshare.view.viewholder.BillViewHolder
import android.support.v7.app.AlertDialog
import android.content.DialogInterface
import elte.moneyshare.viewmodel.GroupViewModel
import android.arch.lifecycle.ViewModelProviders
import android.os.Bundle
import android.support.v4.app.FragmentActivity
import android.support.v4.app.Fragment
import elte.moneyshare.*
import elte.moneyshare.view.AddSpendingFragment
import elte.moneyshare.view.MainActivity
import elte.moneyshare.view.NewGroupFragment

class BillsRecyclerViewAdapter(private val context: Context, private val bills: List<SpendingData>, private val groupId : Int) : RecyclerView.Adapter<BillViewHolder>() {

    private var animationDuration = 300L

    private var listener: (() -> Unit)? = null

    fun setListener(listener: (() -> Unit)?) {
        this.listener = listener
    }

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
        holder.billRootConstraintLayout.setOnLongClickListener {
            val id = bills[position].id
            showModifyDialog(id)
            return@setOnLongClickListener true
        }
    }
    private fun showModifyDialog(id: Int){
        // Late initialize an alert dialog object
        lateinit var dialog:AlertDialog


        // Initialize a new instance of alert dialog builder object
        val builder = AlertDialog.Builder(context)

        // Set a title for alert dialog
        builder.setTitle(context.getString(R.string.modify_spending))

        // Set a message for alert dialog
        builder.setMessage(context.getString(R.string.confirm_spending_modification))


        // On click listener for dialog buttons
        val dialogClickListener = DialogInterface.OnClickListener{_,which ->
            when(which){
                DialogInterface.BUTTON_POSITIVE -> modifySpending(id)
                DialogInterface.BUTTON_NEGATIVE -> Toast.makeText(context, context.getString(R.string.modify_cancelled), Toast.LENGTH_SHORT).show()
            }
        }
        builder.setPositiveButton(context.getString(R.string.yes),dialogClickListener)

        builder.setNegativeButton(context.getString(R.string.no),dialogClickListener)



        dialog = builder.create()

        dialog.show()
    }
    private fun modifySpending(id: Int)
    {
        val fragment = AddSpendingFragment()
        val args = Bundle()
        args.putInt(FragmentDataKeys.MEMBERS_FRAGMENT.value, groupId)
        args.putInt(FragmentDataKeys.BILLS_FRAGMENT.value,id)
        fragment.arguments = args
        (context as MainActivity).supportFragmentManager?.beginTransaction()
            ?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()
    }

}
package elte.moneyshare.view.Adapter

import android.content.Context
import android.os.Handler
import android.support.v4.content.ContextCompat
import android.support.v7.widget.RecyclerView
import android.text.Editable
import android.text.TextUtils
import android.text.TextWatcher
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.entity.Member
import elte.moneyshare.view.viewholder.SelectMemberViewHolder
import elte.moneyshare.visible
import kotlinx.android.synthetic.main.list_item_select_member.view.*

class SelectMembersRecyclerViewAdapter(private val context: Context, private val members: ArrayList<Member>, private val afterSelected: Boolean = false): RecyclerView.Adapter<SelectMemberViewHolder>() {

    var selectedIds: ArrayList<Int> = ArrayList()
    var selectedMembers: ArrayList<Member> = ArrayList(members)
    private var maxSpending = selectedMembers.sumBy { it.balance }

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): SelectMemberViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_select_member, parent, false)
        return SelectMemberViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return members.size
    }

    override fun onBindViewHolder(holder: SelectMemberViewHolder, position: Int) {
        val member = members[position]
        if (!afterSelected) {
            onBindViewHolderToSelect(holder, member)
        } else {
            onBindViewHolderToSelected(holder, position)
        }
    }

    private fun onBindViewHolderToSelect(holder: SelectMemberViewHolder, member: Member) {
        if (selectedIds.contains(member.id)) {
            holder.memberRootLayout.background = ContextCompat.getDrawable(context, R.color.colorSubBackground)
        } else {
            holder.memberRootLayout.background = ContextCompat.getDrawable(context, R.color.colorBackground)
        }

        holder.memberNameTextView.text = member.name

        holder.memberRootLayout.setOnClickListener {
            if (selectedIds.contains(member.id)) {
                selectedIds.remove(member.id)
                holder.memberRootLayout.background = ContextCompat.getDrawable(context, R.color.colorBackground)
                holder.memberRootLayout.untickedImageView.visibility = View.VISIBLE
                holder.memberRootLayout.tickedImageView.visibility = View.GONE
            } else {
                selectedIds.add(member.id)
                holder.memberRootLayout.background = ContextCompat.getDrawable(context, R.color.colorSubBackground)
                holder.memberRootLayout.untickedImageView.visibility = View.GONE
                holder.memberRootLayout.tickedImageView.visibility = View.VISIBLE
            }
        }
    }

    private fun onBindViewHolderToSelected(holder: SelectMemberViewHolder, position: Int) {
        val member = members[position]
        holder.memberNameTextView.text = member.name

        holder.tickedImageView.visibility = View.GONE
        holder.untickedImageView.visibility = View.GONE

        holder.memberSpendingEditText.visible()
        holder.memberSpendingEditText.text = member.balance.toString()

        holder.memberSpendingEditText.addTextChangedListener(object : TextWatcher{
            override fun afterTextChanged(editable: Editable?) {
                if (!TextUtils.isEmpty(editable.toString())) {
                    val spending = Integer.valueOf(editable.toString())
                    var maxMod = maxSpending - selectedMembers.filter { it.id != member.id }.sumBy { it.balance }

                    if ((spending - member.balance) > maxMod) {
                        holder.memberSpendingEditText.error = String.format(context.getString(R.string.max_spending_limit), maxMod)
                        holder.memberSpendingEditText.text = maxMod.toString()
                    } else {
                        maxMod += member.balance
                        member.balance = spending
                        maxMod -= member.balance
                    }
                } else {
                    holder.memberSpendingEditText.text = "0"
                }
            }

            override fun beforeTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {}

            override fun onTextChanged(p0: CharSequence?, p1: Int, p2: Int, p3: Int) {}
        })

        holder.memberSpendingEditText.setOnFocusChangeListener { view, hasFocus ->
            if (!hasFocus) {
                holder.memberSpendingEditText.error = null
            }
        }
    }
}
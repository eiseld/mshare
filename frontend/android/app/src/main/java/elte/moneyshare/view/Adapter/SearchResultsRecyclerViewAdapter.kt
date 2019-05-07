package elte.moneyshare.view.Adapter

import android.content.Context
import android.os.Bundle
import android.support.v7.widget.RecyclerView
import android.view.LayoutInflater
import android.view.ViewGroup
import android.widget.Toast
import elte.moneyshare.FragmentDataKeys
import elte.moneyshare.R
import elte.moneyshare.view.GroupPagerFragment
import elte.moneyshare.view.MainActivity
import elte.moneyshare.view.viewholder.SearchResultViewHolder
import elte.moneyshare.viewmodel.GroupViewModel

class SearchResultsRecyclerViewAdapter(private val context: Context, private val name: String, private val groupId: Int, private val model: GroupViewModel): RecyclerView.Adapter<SearchResultViewHolder>()  {

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): SearchResultViewHolder {
        val itemView = LayoutInflater.from(parent.context).inflate(R.layout.list_item_search_result, parent, false)
        return SearchResultViewHolder(itemView)
    }

    override fun getItemCount(): Int {
        return 1
    }

    override fun onBindViewHolder(holder: SearchResultViewHolder, position: Int) {
        holder.nameTextView.text = name
        holder.nameTextView.setOnClickListener {
            model.postMember(groupId, 5, { response, error ->
                if(error == null) {
                    val fragment = GroupPagerFragment()
                    val args = Bundle()
                    groupId?.let {
                        args.putInt(FragmentDataKeys.GROUP_PAGER_FRAGMENT.value, it)
                    }
                    fragment.arguments = args
                    (context as MainActivity).supportFragmentManager?.beginTransaction()?.replace(R.id.frame_container, fragment)?.addToBackStack(null)?.commit()
                    (context as MainActivity).onBackPressed()
                    Toast.makeText(context, response, Toast.LENGTH_SHORT).show()
                } else {
                    Toast.makeText(context, error, Toast.LENGTH_SHORT).show()
                }
            })
        }
    }
}
package elte.moneyshare.view

import android.app.Fragment
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.R
import elte.moneyshare.entity.Member
import elte.moneyshare.viewmodel.AddMembersViewModel

class AddMembersFragment : Fragment() {

    private lateinit var viewModel: AddMembersViewModel
    private var groupId: Int? = null
    private var members = ArrayList<Member>()

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_add_members, container, false)

    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

    }

}
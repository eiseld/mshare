package elte.moneyshare.view

import android.app.Fragment
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import elte.moneyshare.entity.Member

class AddMembersFragment : Fragment() {

    private lateinit var viewModel: AddMembersViewModel
    private var groupId: Int? = null
    private var members = ArrayList<Member>()

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {

    }

    override fun onActivityCreated(savedInstanceState: Bundle?) {
        super.onActivityCreated(savedInstanceState)

    }

}
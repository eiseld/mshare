package elte.moneyshare.view

import android.content.Context
import android.util.AttributeSet
import android.view.View
import elte.moneyshare.R
import elte.moneyshare.gone
import kotlinx.android.synthetic.main.dialog_confirmation.view.*


class ConfirmationDialogView @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
    defStyle: Int = 0
) : BaseView(context, attrs, defStyle) {

    init {
        View.inflate(context, R.layout.dialog_confirmation, this)
    }

    fun bind(message: String, positiveButtonString: String?, negativeButtonString: String?, confirm: () -> Unit, cancel: () -> Unit) {
        mainTitleTextView?.text = message
        subTitleTextView?.gone()

        if (negativeButtonString != null){
            cancelButton?.text = negativeButtonString
        } else {
            cancelButton?.text = resources.getString(R.string.no)
        }

        if (positiveButtonString != null){
            saveButton?.text = positiveButtonString
        } else {
            saveButton?.text = resources.getString(R.string.yes)
        }

        saveButton?.setOnClickListener {
            confirm()
            cancel()
        }
        cancelButton?.setOnClickListener { cancel() }
    }

}
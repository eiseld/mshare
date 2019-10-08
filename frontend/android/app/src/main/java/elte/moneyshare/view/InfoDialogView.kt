package elte.moneyshare.view

import android.content.Context
import android.text.method.ScrollingMovementMethod
import android.util.AttributeSet
import android.view.View
import elte.moneyshare.R
import kotlinx.android.synthetic.main.dialog_info.view.*

class InfoDialogView @JvmOverloads constructor(
    context: Context,
    attrs: AttributeSet? = null,
    defStyle: Int = 0
) : BaseView(context, attrs, defStyle) {

    init {
        View.inflate(context, R.layout.dialog_info, this)
    }

    fun bind(message: String?, positiveAction: () -> Unit = {}, dismiss: () -> Unit) {
        mainTitleTextView?.movementMethod = ScrollingMovementMethod()
        if (message != null) {
            mainTitleTextView?.text = message
        }

        okTextView?.setOnClickListener {
            //Do not show again the popup
            //positiveAction()
            dismiss()
        }

    }

}
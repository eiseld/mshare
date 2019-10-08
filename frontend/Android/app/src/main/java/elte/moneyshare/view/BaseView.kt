package elte.moneyshare.view

import android.content.Context
import android.util.AttributeSet
import android.widget.LinearLayout

open class BaseView : LinearLayout {

    constructor(context: Context) : super(context)

    constructor(context: Context, attrs: AttributeSet?) : super(context, attrs)

    constructor(context: Context, attrs: AttributeSet?, defStyleAttr: Int) : super(context, attrs, defStyleAttr)

    override fun onAttachedToWindow() {
        super.onAttachedToWindow()
        //EventBus.getDefault().register(this)
    }

    override fun onDetachedFromWindow() {
        super.onDetachedFromWindow()
        //EventBus.getDefault().unregister(this)
    }

    /*@Subscribe
    fun initialSubscribe(event: InitialEvent) {
        //Do nothing here
    }*/

}
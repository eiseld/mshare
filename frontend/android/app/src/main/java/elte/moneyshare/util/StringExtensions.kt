package elte.moneyshare.util

import android.content.Context
import elte.moneyshare.manager.DialogManager

fun String?.showAsDialog(context: Context? = null, positiveAction: () -> Unit = {}) {
    if (!this.isNullOrEmpty()) DialogManager.showInfoDialog(this, context, positiveAction)
}
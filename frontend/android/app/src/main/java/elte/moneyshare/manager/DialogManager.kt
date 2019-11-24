package elte.moneyshare.manager

import android.content.Context
import android.support.v7.app.AlertDialog
import elte.moneyshare.R

object DialogManager {

    private var infoAlertDialog: AlertDialog? = null

    fun showInfoDialog(message: String?, context: Context?, positiveAction: () -> Unit = {}) {
        if (context == null) return

        showDialog(message, context, positiveAction)
    }

    private fun showDialog(message: String?, context: Context, positiveAction: () -> Unit = {}) {
        infoAlertDialog?.dismiss()
        val builder = AlertDialog.Builder(context)
        builder.setTitle(R.string.information_dialog_title)
        builder.setMessage(message)

        builder.setPositiveButton(context.getString(R.string.ok)) { _, _ -> positiveAction()}

        infoAlertDialog = builder.create()
        infoAlertDialog?.show()
    }

    private var confirmationAlertDialog: AlertDialog? = null

    fun confirmationDialog(message: String, title: String?, context: Context?, confirm: () -> Unit) {
        if (context == null) return

        confirmationAlertDialog?.dismiss()
        val builder = AlertDialog.Builder(context)

        if (title.isNullOrEmpty()) builder.setTitle(context.getString(R.string.confirmation_needed))
        else builder.setTitle(title)

        builder.setMessage(message)

        builder.setPositiveButton(context.getString(R.string.yes)) { _, _ -> confirm() }
        builder.setNegativeButton(context.getString(R.string.no)) { _, _ -> }

        confirmationAlertDialog = builder.create()
        confirmationAlertDialog?.show()
    }

    fun confirmationDialog(message: String, context: Context?, confirm: () -> Unit) {
        confirmationDialog(message, null, context, confirm)
    }
}
package elte.moneyshare.manager

import android.app.Activity
import android.content.Context
import android.support.v7.app.AlertDialog
import android.view.WindowManager
import elte.moneyshare.R
import elte.moneyshare.view.ConfirmationDialogView
import elte.moneyshare.view.InfoDialogView


object DialogManager {

    private var infoAlertDialog: AlertDialog? = null

    fun showInfoDialog(message: String?, context: Context?, positiveAction: () -> Unit = {}) {
        if (context == null) return

        showDialog(message, context, positiveAction)
        /*infoAlertDialog?.dismiss()
        infoAlertDialog = AlertDialog.Builder(context).create()

        val infoDialog = InfoDialogView(context)
        infoAlertDialog?.setView(infoDialog)

        infoAlertDialog?.window?.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        val lp = infoAlertDialog?.window?.attributes
        infoAlertDialog?.window?.setLayout(lp?.width ?: 0, lp?.height ?: 0)

        infoAlertDialog?.setOnDismissListener {
            //Do not show again the popup
            //positiveAction()
        }

        infoDialog.bind(message, positiveAction, { infoAlertDialog?.dismiss() })

        // if (!(context as Activity).isFinishing) {
        infoAlertDialog?.show()
        // }*/
    }

    fun showDialog(message: String?, context: Context, positiveAction: () -> Unit = {}) {
        infoAlertDialog?.dismiss()
        val builder = AlertDialog.Builder(context)
        builder.setTitle(R.string.information_dialog_title)
        builder.setMessage(message)

        builder.setPositiveButton(context.getString(R.string.ok)) { _, _ -> positiveAction()}

        infoAlertDialog = builder.create()
        infoAlertDialog?.show()
    }

    private var confirmationAlertDialog: AlertDialog? = null

    fun confirmationDialog(context: Context?, message: String, positiveButtonString: String?, negativeButtonString: String?, confirm: () -> Unit) {
        if (context == null) return

        confirmationAlertDialog?.dismiss()
        confirmationAlertDialog = AlertDialog.Builder(context).create()
        val confirmationDialog = ConfirmationDialogView(context)

        confirmationAlertDialog?.setView(confirmationDialog)

        val lp = confirmationAlertDialog?.window?.attributes
        confirmationAlertDialog?.window?.setLayout(lp?.width ?: 0, lp?.height ?: 0)

        confirmationDialog.bind(message, positiveButtonString, negativeButtonString, { confirm() }, { confirmationAlertDialog?.dismiss() })

        if (!(context as Activity).isFinishing) {
            confirmationAlertDialog?.show()
        }
    }

    fun confirmationDialog(context: Context?, message: String, confirm: () -> Unit) {
        confirmationDialog(context, message, null, null) { confirm()}
    }
}
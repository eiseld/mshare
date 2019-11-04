package elte.moneyshare.util

import android.content.Context
import android.widget.Toast
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.R

enum class Action
{
    AUTH_REGISTER, AUTH_VALIDATE, AUTH_LOGIN,
    GROUPS, GROUPS_ADD_MEMBER, GROUPS_CREATE, GROUPS_SETTLE,
    PROFILE, PROFILE_RESET,PROFILE_UPDATE, PROFILE_LANGUAGE,
    SPENDING, SPENDING_CREATE, SPENDING_UPDATE, SPENDING_DELETE, CHANGE_PASSWORD
}

fun String?.showAsDialog(context: Context? = null, positiveAction: () -> Unit = {}) {
    if (!this.isNullOrEmpty()) DialogManager.showInfoDialog(this, context, positiveAction)
}

fun String?.showToast(context: Context, length: Int = Toast.LENGTH_SHORT){
    if (!this.isNullOrEmpty()) Toast.makeText(context, this, length).show()
}

fun String?.convertErrorCodeToString(
    action: Action,
    context: Context?
): String {
    return when(this)
    {
        "500" ->  context?.getString(R.string.api_common_500).toString()
        "400" -> context?.getString(R.string.api_common_400).toString()
        "200" -> when(action)
        {
            Action.AUTH_REGISTER -> context?.getString(R.string.api_auth_register_200).toString()
            Action.AUTH_VALIDATE -> context?.getString(R.string.api_auth_validate_200).toString()
            Action.AUTH_LOGIN -> context?.getString(R.string.api_auth_login_200).toString()
            Action.GROUPS_ADD_MEMBER -> context?.getString(R.string.api_groups_add_member_200).toString()
            Action.GROUPS_CREATE -> context?.getString(R.string.api_groups_create_group_200).toString()
            Action.GROUPS_SETTLE -> context?.getString(R.string.api_groups_settle_debt_200).toString()
            Action.PROFILE_RESET -> context?.getString(R.string.api_profile_password_reset_200).toString()
            Action.PROFILE_UPDATE -> context?.getString(R.string.api_profile_password_update_200).toString()
            Action.PROFILE_LANGUAGE -> context?.getString(R.string.api_profile_language_update_200).toString()
            Action.SPENDING_CREATE -> context?.getString(R.string.api_profile_language_update_200).toString()
            Action.SPENDING_UPDATE -> context?.getString(R.string.api_profile_language_update_200).toString()
            Action.SPENDING_DELETE -> context?.getString(R.string.api_spending_delete_200).toString()
            Action.CHANGE_PASSWORD -> context?.getString(R.string.passwordSuccessfullyChanged).toString()
            else -> "aa"
        }
        "201" -> when(action)
        {
            Action.AUTH_REGISTER -> context?.getString(R.string.api_auth_register_201).toString()
            else -> "aa"
        }
        "403" -> when(action)
        {
            Action.AUTH_LOGIN -> context?.getString(R.string.api_auth_login_403).toString()
            Action.GROUPS -> context?.getString(R.string.api_groups_403).toString()
            Action.PROFILE -> context?.getString(R.string.api_profile_403).toString()
            Action.SPENDING -> context?.getString(R.string.api_spending_403).toString()
            Action.CHANGE_PASSWORD -> context?.getString(R.string.apiChangePassword403).toString()
            else -> "aa"
        }
        "404" -> when(action)
        {
            Action.GROUPS -> context?.getString(R.string.api_groups_404).toString()
            Action.PROFILE -> context?.getString(R.string.api_profile_404).toString()
            Action.SPENDING -> context?.getString(R.string.api_spending_404).toString()
            else -> "aa"
        }
        "409" -> when(action)
        {
            Action.AUTH_REGISTER -> context?.getString(R.string.api_auth_register_409).toString()
            Action.AUTH_LOGIN -> context?.getString(R.string.api_auth_login_409).toString()
            Action.GROUPS_ADD_MEMBER -> context?.getString(R.string.api_groups_add_member_409).toString()
            Action.GROUPS_CREATE -> context?.getString(R.string.api_groups_create_group_409).toString()
            Action.GROUPS_SETTLE -> context?.getString(R.string.api_groups_settle_debt_409).toString()
            Action.SPENDING_CREATE -> context?.getString(R.string.api_spending_create_409).toString()
            Action.SPENDING_UPDATE -> context?.getString(R.string.api_spending_update_409).toString()
            else -> ""
        }
        "410" -> when(action)
        {
            Action.AUTH_LOGIN -> context?.getString(R.string.api_login_language_validate_410).toString()
            Action.AUTH_REGISTER -> context?.getString(R.string.api_auth_validate_410).toString()
            Action.GROUPS_SETTLE -> context?.getString(R.string.api_groups_settle_debt_410).toString()
            Action.PROFILE_UPDATE -> context?.getString(R.string.api_profile_password_update_410).toString()
            else -> "aa"
        }
        else -> context?.getString(R.string.api_error_common).toString()
    }
}

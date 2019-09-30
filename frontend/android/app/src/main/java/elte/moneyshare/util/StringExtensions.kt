package elte.moneyshare.util

import android.content.Context
import elte.moneyshare.manager.DialogManager
import elte.moneyshare.R

enum class Action
{
    AUTH_REGISTER, AUTH_VALIDATE, AUTH_LOGIN,
    GROUPS,GROUPS_ADD_MEMBER, GROUPS_CREATE,GROUPS_SETTLE,
    PROFILE,PROFILE_RESET,PROFILE_UPDATE,PROFILE_LANGUAGE,
    SPENDING, SPENDING_CREATE,SPENDING_UPDATE
}

fun String?.showAsDialog(context: Context? = null, positiveAction: () -> Unit = {}) {
    if (!this.isNullOrEmpty()) DialogManager.showInfoDialog(this, context, positiveAction)
}

fun String?.convertErrorCodeToString(action: Action): String {
    return when(this)
    {
        "500" ->  R.string.api_common_500.toString()
        "400" -> R.string.api_common_400.toString()
        "200" -> when(action)
        {
            Action.AUTH_REGISTER -> R.string.api_auth_register_200.toString()
            Action.AUTH_VALIDATE -> R.string.api_auth_validate_200.toString()
            Action.AUTH_LOGIN -> R.string.api_auth_login_200.toString()
            Action.GROUPS_ADD_MEMBER -> R.string.api_groups_add_member_200.toString()
            Action.GROUPS_CREATE -> R.string.api_groups_create_group_200.toString()
            Action.GROUPS_SETTLE -> R.string.api_groups_settle_debt_200.toString()
            Action.PROFILE_RESET -> R.string.api_profile_password_reset_200.toString()
            Action.PROFILE_UPDATE -> R.string.api_profile_password_update_200.toString()
            Action.PROFILE_LANGUAGE -> R.string.api_profile_language_update_200.toString()
            Action.SPENDING_CREATE -> R.string.api_profile_language_update_200.toString()
            Action.SPENDING_UPDATE -> R.string.api_profile_language_update_200.toString()
            else -> "aa"
        }
        "201" -> when(action)
        {
            Action.AUTH_REGISTER -> R.string.api_auth_register_201.toString()
            else -> "aa"
        }
        "403" -> when(action)
        {
            Action.AUTH_LOGIN -> R.string.api_auth_login_403.toString()
            Action.GROUPS -> R.string.api_groups_403.toString()
            Action.PROFILE -> R.string.api_profile_403.toString()
            Action.SPENDING -> R.string.api_spending_403.toString()
            else -> "aa"
        }
        "404" -> when(action)
        {
            Action.GROUPS -> R.string.api_groups_404.toString()
            Action.PROFILE -> R.string.api_profile_404.toString()
            Action.SPENDING -> R.string.api_spending_404.toString()
            else -> "aa"
        }
        "409" -> when(action)
        {
            Action.AUTH_REGISTER -> R.string.api_auth_register_409.toString()
            Action.AUTH_LOGIN -> R.string.api_auth_login_409.toString()
            Action.GROUPS_ADD_MEMBER -> R.string.api_groups_add_member_409.toString()
            Action.GROUPS_CREATE -> R.string.api_groups_create_group_409.toString()
            Action.GROUPS_SETTLE -> R.string.api_groups_settle_debt_409.toString()
            Action.SPENDING_CREATE -> R.string.api_spending_create_409.toString()
            Action.SPENDING_UPDATE -> R.string.api_spending_update_409.toString()
            else -> ""
        }
        "410" -> when(action)
        {
            Action.AUTH_REGISTER -> R.string.api_auth_validate_410.toString()
            Action.GROUPS_SETTLE -> R.string.api_groups_settle_debt_410.toString()
            Action.PROFILE_UPDATE -> R.string.api_profile_password_update_410.toString()
            else -> "aa"
        }
        else -> R.string.api_error_common.toString()
    }
}

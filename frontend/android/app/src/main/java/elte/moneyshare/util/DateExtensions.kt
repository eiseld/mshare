package elte.moneyshare.util

import java.text.DateFormat
import java.text.SimpleDateFormat
import java.util.Calendar

val dateFormat = SimpleDateFormat("yyyy-MM-dd")

fun String.convertToCalendar(): Calendar {
    val cal = Calendar.getInstance()
    cal.time = dateFormat.parse(this)
    return cal
}

fun Calendar.formatDate(): String = DateFormat.getDateInstance().format(this.time)

fun Calendar.convertToBackendFormat(): String {
    return dateFormat.format(this.time)
}

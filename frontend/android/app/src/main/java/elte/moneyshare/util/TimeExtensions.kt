package elte.moneyshare.util

import java.text.DateFormat
import java.text.SimpleDateFormat
import java.util.TimeZone
import java.util.Calendar

class TimeExtensions {
    companion object {
        private val utcDateFormat = SimpleDateFormat("yyyy-MM-dd")

        init {
            utcDateFormat.timeZone = TimeZone.getTimeZone("UTC")
        }

        fun convertToCalendar(dateInput: String): Calendar {
            val cal = Calendar.getInstance()
            cal.time = utcDateFormat.parse(dateInput)
            return cal
        }

        fun formatDate(calendar: Calendar): String = DateFormat.getDateInstance().format(calendar.time)

        fun convertToBackendFormat(calendar: Calendar): String = utcDateFormat.format(calendar.time)

    }
}
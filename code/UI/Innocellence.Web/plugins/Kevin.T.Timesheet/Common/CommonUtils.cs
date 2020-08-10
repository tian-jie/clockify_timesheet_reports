using System;

namespace Kevin.T.Timesheet.Common
{
    public static class CommonUtils
    {

        public static int WeekOfYear(this DateTime date)
        {
            int dayOfWeekFirstday = YearFirstWeekDay(date);

            var days = date.DayOfYear - dayOfWeekFirstday;

            return days / 7 + 1;
        }

        private static int YearFirstWeekDay(DateTime date)
        {
            // 计算FirstWeek周期
            var yearFirstDay = new DateTime(date.Year, 1, 1);
            var firstDayofWeek = (int)yearFirstDay.DayOfWeek;
            DateTime weekFirstDay = yearFirstDay.AddDays(-(firstDayofWeek == 0 ? 6 : firstDayofWeek - 1));

            var firstThursday = weekFirstDay.AddDays(3);

            if (yearFirstDay > firstThursday)
            {
                weekFirstDay = weekFirstDay.AddDays(7);
            }
            // weekLastDay = weekFirstDay.AddDays(6);

            // 计算当天跟第一天差几天，算周数
            var dayOfWeekFirstday = weekFirstDay.DayOfYear;
            if (dayOfWeekFirstday > 10)
            {
                dayOfWeekFirstday -= new DateTime(weekFirstDay.Year, 12, 31).DayOfYear;
            }

            return dayOfWeekFirstday;
        }

        public static int YearOfWeekOfYear(this DateTime date)
        {
            int dayOfWeekFirstday = YearFirstWeekDay(date);

            if(date.DayOfYear + dayOfWeekFirstday <= 0)
            {
                return date.Year + 1;
            }
            else if(date.DayOfYear + dayOfWeekFirstday>= new DateTime(date.Year, 12, 31).DayOfYear)
            {
                return date.Year - 1;
            }
            else
            {
                return date.Year;
            }
        }

    }
}
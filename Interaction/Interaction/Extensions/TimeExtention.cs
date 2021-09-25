using System;

namespace Interaction.Services {
    //Enum value representing a holiday.
    public enum Holiday {
        None,
        Halloween,
        Valentines,
        Christmas,
        April_1st,
        Easter,
        NewYear,
    }

    public static class TimeExtention {
        public static long ToMilliseconds(this DateTimeOffset time)
            => TimeUtilities.TicksToMilliseconds(time.Ticks);
        public static long ToSeconds(this DateTimeOffset time)
            => TimeUtilities.TicksToSeconds(time.Ticks);
        public static long ToMinutes(this DateTimeOffset time)
            => TimeUtilities.TicksToMinutes(time.Ticks);
        public static long ToHours(this DateTimeOffset time)
            => TimeUtilities.TicksToHours(time.Ticks);
        public static long ToDays(this DateTimeOffset time)
            => TimeUtilities.TicksToDays(time.Ticks);

        public static long ToMilliseconds(this DateTime time)
            => TimeUtilities.TicksToMilliseconds(time.Ticks);
        public static long ToSeconds(this DateTime time)
            => TimeUtilities.TicksToSeconds(time.Ticks);
        public static long ToMinutes(this DateTime time)
            => TimeUtilities.TicksToMinutes(time.Ticks);
        public static long ToHours(this DateTime time)
            => TimeUtilities.TicksToHours(time.Ticks);
        public static long ToDays(this DateTime time)
            => TimeUtilities.TicksToDays(time.Ticks);

        //Returns last sunday 00:00:00.
        public static DateTimeOffset GetStartOfWeek(this DateTimeOffset date) {
            date = date.Subtract(date.TimeOfDay);
            date = date.AddDays(-(int)date.DayOfWeek);
            return date;
        }


        //Get the time since this date.
        public static TimeSpan GetTimeSince(this DateTimeOffset date)
            => DateTimeOffset.Now - date;


        //Get the date with no time of day.
        public static DateTimeOffset GetDate(int year, int month, int day, TimeSpan offset)
            => new DateTimeOffset(year, month, day, 0, 0, 0, offset);

        //public static DateTimeOffset GetDate(this DateTimeOffset date)
        //	=> GetDate(date.Year, date.Month, date.Date) 


        //Removes the milliseconds from the DateTimeOffset.
        public static DateTimeOffset RemoveMilliseconds(this DateTimeOffset date) {
            return date.AddMilliseconds(-date.Millisecond);
        }

        static DateTimeOffset LastChecked { get; set; }
        static Holiday CurrentHoliday { get; set; }

        //Get the currant holiday.

        //Rechecks if it's a new day.
        public static Holiday GetHoliday() {
            var now = DateTimeOffset.Now.Date;

            // If it's a new day: Get new currant
            if (now != LastChecked) {
                SysLog.Instance.Info(typeof(TimeExtention), $"{now} != {LastChecked}", parameterString: $"{DateTimeOffset.Now}");
                LastChecked = now;
                CurrentHoliday = GetHoliday(now);
            }

            return CurrentHoliday;
        }

        //Gets the holiday of the desired date.
        public static Holiday GetHoliday(this DateTimeOffset date) {
            if (date.Month == 11 && date.Day == 31)
                return Holiday.Halloween;
            if (date.Month == 2 && date.Day == 14)
                return Holiday.Valentines;
            if (date.Month == 12 && date.Day == 24)
                return Holiday.Christmas;
            if (date.Month == 4 && date.Day == 1)
                return Holiday.April_1st;
            if (date.Month == 12 && date.Day == 31)
                return Holiday.NewYear;

            var easter = GaussEaster(date);
            if (date.Month == easter.Month && date.Day == easter.Day)
                return Holiday.Easter;

            return Holiday.None;
        }


        //Calculates easter date for given year Y.

        //Uses the Gauss Easter Algorithm.

        //Source: https://www.geeksforgeeks.org/how-to-calculate-the-easter-date-for-a-given-year-using-gauss-algorithm/
        static (int, int) GaussEaster(int Y) {
            float A, B, C, P, Q, M, N, D, E;

            A = Y % 19;
            B = Y % 4;
            C = Y % 7;
            P = (float)(Y / 100);
            Q = (float)((13 + 8 * P) / 25);
            M = (15 - Q + P - P / 4) % 30;
            N = (4 + P - P / 4) % 7;
            D = (19 * A + M) % 30;
            E = (2 * B + 4 * C + 6 * D + N) % 7;

            int days = (int)(22 + D + E);


            //A corner case, when D is 29.
            if ((D == 29) && (E == 6)) return (4, 19);
            //Another corner case, when D is 28.
            else if ((D == 28) && (E == 6)) return (4, 18);
            else {
                //If days > 31, move to April.
                if (days > 31) return (4, days - 31);
                //Otherwise, stay on March.
                else return (3, days);
            }
        }

        public static DateTimeOffset GaussEaster(this DateTimeOffset date) {
            int Y = date.Year;
            float A, B, C, P, Q, M, N, D, E;

            A = Y % 19;
            B = Y % 4;
            C = Y % 7;
            P = (float)(Y / 100);
            Q = (float)((13 + 8 * P) / 25);
            M = (15 - Q + P - P / 4) % 30;
            N = (4 + P - P / 4) % 7;
            D = (19 * A + M) % 30;
            E = (2 * B + 4 * C + 6 * D + N) % 7;

            int days = (int)(22 + D + E);


            //A corner case, when D is 29.
            if ((D == 29) && (E == 6)) return new DateTimeOffset(Y, 4, 19, 0, 0, 0, date.Offset);
            //Another corner case, when D is 28.
            else if ((D == 28) && (E == 6)) return new DateTimeOffset(Y, 4, 18, 0, 0, 0, date.Offset);
            else {
                //If days > 31, move to April.
                if (days > 31) return new DateTimeOffset(Y, 4, days - 31, 0, 0, 0, date.Offset);
                //Otherwise, stay on March.
                else return new DateTimeOffset(Y, 3, days, 0, 0, 0, date.Offset);
            }
        }
    }
}
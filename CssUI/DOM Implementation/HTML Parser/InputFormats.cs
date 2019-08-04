using CssUI.CSS;
using CssUI.DOM.Enums;
using CssUI.DOM.Exceptions;
using CssUI.Filters;
using System;
using System.Globalization;
using System.Text;
using static CssUI.UnicodeCommon;

namespace CssUI.HTML.Serialization
{
    public static partial class HTMLParser
    {/* Docs: https://www.w3.org/TR/DOM-Parsing/ */
     /* https://html.spec.whatwg.org/multipage/parsing.html#overview-of-the-parsing-model */

        #region Thank the gods for stackoverflow

        /* Source: https://stackoverflow.com/a/11155102 */
        // This presumes that weeks start with Monday.
        // Week 1 is the 1st week of the year with a Thursday in it.
        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }
        #endregion

        #region Dates
        public const string DATE_TIME_FORMAT = @"yyyy-MM-dd\\THH:mm:ss\.fff\\Z0:zzz";
        public const string LOCAL_DATE_TIME_FORMAT = @"yyyy-MM-dd\\THH:mm:ss\.fff";
        public const string MONTH_FORMAT = @"yyyy-MM";
        public const string TIME_FORMAT = @"HH:mm:ss\.fff";

        const long TIME_SCALE_WEEKS = 604800;
        const long TIME_SCALE_DAYS = 86400;
        const long TIME_SCALE_HOURS = 3600;
        const long TIME_SCALE_MINUTES = 60;
        const long TIME_SCALE_SECONDS = 1;

        private static int Get_Days_In_Month(int month, int year)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#number-of-days-in-month-month-of-year-year */
            switch (month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    {
                        return 31;
                    }
                case 4:
                case 6:
                case 9:
                case 11:
                    {
                        return 30;
                    }
                case 2:/* 29 if month is 2 and year is a number divisible by 400, or if year is a number divisible by 4 but not by 100; and 28 otherwise. */
                    {
                        if (0 == year % 400 || 0 == year % 4 && 0 != year % 100)
                            return 29;
                        else
                            return 28;
                    }
                default:
                    throw new ArgumentException($"Unhandled month(# {month})");
            }
        }
        private static int Get_Weeks_In_Year(int year)
        {/* Docs: There are none, they left us on our own */
            /* Get the week number for december 31st of the given year */
            return GetIso8601WeekOfYear(new DateTime(year, 12, 31));
        }


        public static bool Is_Valid_Month_String(ReadOnlyMemory<char> input)
        {
            var Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Month_String(Stream);
        }
        private static bool Is_Valid_Month_String(DataStream<char> Stream)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-month-string */

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlySpan<char> yearDigits) || yearDigits.Length < 4)
                return false;

            bool hasDigitGreaterThanZero = false;
            for (int i = 0; i < yearDigits.Length; i++)
            {
                if (Ascii_Digit_To_Value(yearDigits[i]) > 0)
                {
                    hasDigitGreaterThanZero = true;
                    break;
                }
            }

            if (!hasDigitGreaterThanZero)
                return false;

            if (Stream.Next == CHAR_HYPHEN_MINUS)
                Stream.Consume();
            else
                return false;

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> monthDigits) || monthDigits.Length != 2)
                return false;

            int monthParsed = (int)ParsingCommon.Digits_To_Base10(monthDigits);
            if (monthParsed < 1 || monthParsed > 12)
                return false;

            return true;
        }
        private static bool Consume_Month_Component(DataStream<char> Stream, out int outYear, out int outMonth)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-month-component */

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> yearDigits) || yearDigits.Length < 4)
            {
                outYear = 0;
                outMonth = 0;
                return false;
            }

            int yearParsed = (int)ParsingCommon.Digits_To_Base10(yearDigits);
            if (yearParsed < 1)
            {
                outYear = 0;
                outMonth = 0;
                return false;
            }

            outYear = yearParsed;

            if (!Stream.atEOF && Stream.Next == CHAR_HYPHEN_MINUS)
                Stream.Consume();
            else
            {
                outMonth = 0;
                return false;
            }

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> monthDigits) || monthDigits.Length != 2)
            {
                outMonth = 0;
                return false;
            }

            int monthParsed = (int)ParsingCommon.Digits_To_Base10(monthDigits);
            if (monthParsed < 1 || monthParsed > 12)
            {
                outMonth = 0;
                return false;
            }

            outMonth = monthParsed;

            return true;
        }
        public static bool Parse_Month_String(ReadOnlyMemory<char> input, out int outYear, out int outMonth)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-month-string */

            DataStream<char> Stream = new DataStream<char>(input, EOF);

            if (!Consume_Month_Component(Stream, out int year, out int month))
            {
                outYear = 0;
                outMonth = 0;
                return false;
            }

            if (!Stream.atEOF)
            {
                outYear = 0;
                outMonth = 0;
                return false;
            }

            outYear = year;
            outMonth = month;
            return true;
        }


        public static bool Is_Valid_Date_String(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-date-string */
            var Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Date_String(Stream);
        }
        private static bool Is_Valid_Date_String(DataStream<char> Stream)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-date-string */

            if (!Consume_Month_Component(Stream, out int year, out int month))
            {
                return false;
            }

            if (!Stream.atEOF && Stream.Next == CHAR_HYPHEN_MINUS)
                Stream.Consume();
            else
                return false;

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> dayDigits) || dayDigits.Length != 2)
                return false;

            int dayParsed = (int)ParsingCommon.Digits_To_Base10(dayDigits);
            if (dayParsed < 1 || dayParsed > Get_Days_In_Month(month, year))
                return false;

            return true;
        }
        private static bool Consume_Date_Component(DataStream<char> Stream, out int outYear, out int outMonth, out int outDay)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-date-component */

            if (!Consume_Month_Component(Stream, out int year, out int month))
            {
                outYear = 0;
                outMonth = 0;
                outDay = 0;
                return false;
            }

            if (!Stream.atEOF || Stream.Next == CHAR_HYPHEN_MINUS)
                Stream.Consume();
            else
            {
                outYear = 0;
                outMonth = 0;
                outDay = 0;
                return false;
            }

            outYear = year;
            outMonth = month;
            var maxday = Get_Days_In_Month(month, year);

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> dayDigits) || dayDigits.Length != 2)
            {
                outDay = 0;
                return false;
            }

            int dayParsed = (int)ParsingCommon.Digits_To_Base10(dayDigits);
            if (dayParsed < 1 || dayParsed > maxday)
            {
                outDay = 0;
                return false;
            }

            outDay = dayParsed;
            return true;
        }
        public static bool Parse_Date_String(ReadOnlyMemory<char> input, out DateTime outDate)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-date-string */

            DataStream<char> Stream = new DataStream<char>(input, EOF);

            if (!Consume_Date_Component(Stream, out int year, out int month, out int day))
            {
                outDate = DateTime.MinValue;
                return false;
            }

            if (!Stream.atEOF)
            {
                outDate = DateTime.MinValue;
                return false;
            }

            outDate = new DateTime(year, month, day);
            return true;
        }


        public static bool Is_Valid_Yearless_Date_String(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-yearless-date-string */

            DataStream<char> Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Yearless_Date_String(Stream);
        }
        private static bool Is_Valid_Yearless_Date_String(DataStream<char> Stream)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-yearless-date-string */

            if (Stream.Consume_While((c) => c == CHAR_HYPHEN_MINUS, out ReadOnlyMemory<char> outHypens) && outHypens.Length > 2)
                return false;

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> monthDigits) || monthDigits.Length != 2)
                return false;

            int monthParsed = (int)ParsingCommon.Digits_To_Base10(monthDigits);
            if (monthParsed < 1 || monthParsed > 12)
                return false;

            if (Stream.Next == CHAR_HYPHEN_MINUS)
                Stream.Consume();
            else
                return false;

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> dayDigits) || dayDigits.Length != 2)
                return false;

            int dayParsed = (int)ParsingCommon.Digits_To_Base10(dayDigits);
            if (dayParsed < 1 || dayParsed > Get_Days_In_Month(monthParsed, 4))
                return false;

            return true;
        }
        private static bool Consume_Yearless_Date_Component(DataStream<char> Stream, out int outMonth, out int outDay)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-yearless-date-component */


            if (Stream.Consume_While((c) => c == CHAR_HYPHEN_MINUS, out ReadOnlyMemory<char> outHypens) && outHypens.Length != 0 && outHypens.Length != 2)
            {
                outMonth = 0;
                outDay = 0;
                return false;
            }

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> monthDigits) || monthDigits.Length != 2)
            {
                outMonth = 0;
                outDay = 0;
                return false;
            }

            int monthParsed = (int)ParsingCommon.Digits_To_Base10(monthDigits);
            if (monthParsed < 1 || monthParsed > 12)
            {
                outMonth = 0;
                outDay = 0;
                return false;
            }

            if (Stream.Next == CHAR_HYPHEN_MINUS)
                Stream.Consume();
            else
            {
                outMonth = 0;
                outDay = 0;
                return false;
            }

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> dayDigits) || dayDigits.Length != 2)
            {
                outMonth = 0;
                outDay = 0;
                return false;
            }

            int dayParsed = (int)ParsingCommon.Digits_To_Base10(dayDigits);
            if (dayParsed < 1 || dayParsed > Get_Days_In_Month(monthParsed, 4))
            {
                outMonth = 0;
                outDay = 0;
                return false;
            }

            outMonth = monthParsed;
            outDay = dayParsed;
            return true;
        }
        public static bool Parse_Yearless_Date_String(ReadOnlyMemory<char> input, out int outMonth, out int outDay)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-yearless-date-string */

            var Stream = new DataStream<char>(input, EOF);

            if (!Consume_Yearless_Date_Component(Stream, out int month, out int day))
            {
                outMonth = 0;
                outDay = 0;
                return false;
            }

            if (!Stream.atEOF)
            {
                outMonth = 0;
                outDay = 0;
                return false;
            }

            outMonth = month;
            outDay = day;
            return true;
        }


        public static bool Is_Valid_Time_String(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-time-string */

            var Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Time_String(Stream);
        }
        private static bool Is_Valid_Time_String(DataStream<char> Stream)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-time-string */

            /* HOURS */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> hourDigits) || hourDigits.Length != 2)
                return false;

            int hourParsed = (int)ParsingCommon.Digits_To_Base10(hourDigits);
            if (hourParsed < 0 || hourParsed > 23)
                return false;

            if (Stream.Next == CHAR_COLON)
                Stream.Consume();
            else
                return false;

            /* MINUTES */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> minuteDigits) || minuteDigits.Length != 2)
                return false;

            int minuteParsed = (int)ParsingCommon.Digits_To_Base10(minuteDigits);
            if (minuteParsed < 0 || minuteParsed > 59)
                return false;

            if (Stream.Next == CHAR_COLON)
                Stream.Consume();
            else
                return true;// Time strings arent required to represent seconds

            /* SECONDS */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> secondsDigits) || secondsDigits.Length != 2)
                return false;

            int secondsParsed = (int)ParsingCommon.Digits_To_Base10(secondsDigits);
            if (secondsParsed < 0 || secondsParsed > 59)
                return false;

            if (Stream.Next == CHAR_FULL_STOP)
                Stream.Consume();
            else
                return true;

            /* FRACTIONAL SECONDS PART */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> floatDigits) || floatDigits.Length < 1 || floatDigits.Length > 3)
                return false;

            return true;
        }
        public static bool Is_Valid_Normalized_Time_String(ReadOnlyMemory<char> input)
        {/* Normalized just means we dont have a seconds component present if its 0 */

            var Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Normalized_Time_String(Stream);
        }
        private static bool Is_Valid_Normalized_Time_String(DataStream<char> Stream)
        {/* Normalized just means we dont have a seconds component present if its 0 */

            /* HOURS */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> hourDigits) || hourDigits.Length != 2)
                return false;

            int hourParsed = (int)ParsingCommon.Digits_To_Base10(hourDigits);
            if (hourParsed < 0 || hourParsed > 23)
                return false;

            if (Stream.Next == CHAR_COLON)
                Stream.Consume();
            else
                return false;

            /* MINUTES */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> minuteDigits) || minuteDigits.Length != 2)
                return false;

            int minuteParsed = (int)ParsingCommon.Digits_To_Base10(minuteDigits);
            if (minuteParsed < 0 || minuteParsed > 59)
                return false;

            if (Stream.Next == CHAR_COLON)
                Stream.Consume();
            else
                return true;// Time strings arent required to represent seconds

            /* SECONDS */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> secondsDigits) || secondsDigits.Length != 2)
                return false;

            int secondsParsed = (int)ParsingCommon.Digits_To_Base10(secondsDigits);
            if (secondsParsed < 1 || secondsParsed > 59)
                return false;

            if (Stream.Next == CHAR_FULL_STOP)
                Stream.Consume();
            else
                return true;

            /* FRACTIONAL SECONDS PART */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> floatDigits) || floatDigits.Length < 1 || floatDigits.Length > 3)
                return false;

            return true;
        }
        private static bool Consume_Time_Component(DataStream<char> Stream, out int outHours, out int outMinutes, out double outSeconds)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-time-component */

            /* HOURS */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> hourDigits) || hourDigits.Length != 2)
            {
                outHours = 0;
                outMinutes = 0;
                outSeconds = 0;
                return false;
            }

            var hourParsed = (int)ParsingCommon.Digits_To_Base10_Unsigned(hourDigits);
            if (hourParsed < 0 || hourParsed > 23)
            {
                outHours = 0;
                outMinutes = 0;
                outSeconds = 0;
                return false;
            }

            if (Stream.Next == CHAR_COLON)
                Stream.Consume();
            else
            {
                outHours = 0;
                outMinutes = 0;
                outSeconds = 0;
                return false;
            }

            /* MINUTES */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> minuteDigits) || minuteDigits.Length != 2)
            {
                outHours = 0;
                outMinutes = 0;
                outSeconds = 0;
                return false;
            }

            var minuteParsed = (int)ParsingCommon.Digits_To_Base10_Unsigned(minuteDigits);
            if (minuteParsed < 0 || minuteParsed > 59)
            {
                outHours = 0;
                outMinutes = 0;
                outSeconds = 0;
                return false;
            }

            if (Stream.Next == CHAR_COLON)
                Stream.Consume();
            else
            {
                outHours = 0;
                outMinutes = 0;
                outSeconds = 0;
                return true;// Time strings arent required to represent seconds
            }

            /* SECONDS */
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> secondsDigits) || secondsDigits.Length != 2)
            {
                outHours = 0;
                outMinutes = 0;
                outSeconds = 0;
                return false;
            }

            double secondsParsed = ParsingCommon.Digits_To_Base10_Unsigned(secondsDigits);

            if (Stream.Next == CHAR_FULL_STOP)
            {
                Stream.Consume();
                /* FRACTIONAL SECONDS PART */
                if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> floatDigits) || floatDigits.Length < 1 || floatDigits.Length > 3)
                {
                    outHours = 0;
                    outMinutes = 0;
                    outSeconds = 0;
                    return false;
                }

                secondsParsed = ParsingCommon.ToDecimal(1, secondsDigits.Span, floatDigits.Span, 1, null);
            }
            else
            {
                outHours = 0;
                outMinutes = 0;
                outSeconds = 0;
                return true;
            }

            if (secondsParsed < 0 || secondsParsed > 59)
            {
                outHours = 0;
                outMinutes = 0;
                outSeconds = 0;
                return false;
            }

            outHours = hourParsed;
            outMinutes = minuteParsed;
            outSeconds = secondsParsed;
            return true;
        }
        public static bool Parse_Time_String(ReadOnlyMemory<char> input, out TimeSpan outTime)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-time-string */

            var Stream = new DataStream<char>(input, EOF);

            if (!Consume_Time_Component(Stream, out int hours, out int minutes, out float seconds))
            {
                outTime = TimeSpan.MinValue;
                return false;
            }

            if (!Stream.atEOF)
            {
                outTime = TimeSpan.MinValue;
                return false;
            }

            var time = new TimeSpan(hours, minutes, 0);
            outTime = time.Add(TimeSpan.FromSeconds(seconds));
            return true;
        }


        public static bool Is_Valid_Local_Date_Time_String(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#concept-datetime-local */

            DataStream<char> Stream = new DataStream<char>(input, EOF);

            if (!Is_Valid_Date_String(Stream))
            {
                return false;
            }

            if (Stream.Next == CHAR_T_UPPER || Stream.Next == CHAR_SPACE)
                Stream.Consume();
            else
                return false;

            if (!Is_Valid_Time_String(Stream))
            {
                return false;
            }

            return true;
        }
        public static bool Is_Valid_Normalized_Local_Date_Time_String(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-normalised-local-date-and-time-string */

            DataStream<char> Stream = new DataStream<char>(input, EOF);

            if (!Is_Valid_Date_String(Stream))
            {
                return false;
            }

            if (Stream.Next == CHAR_T_UPPER)
                Stream.Consume();
            else
                return false;

            if (!Is_Valid_Normalized_Time_String(Stream))
            {
                return false;
            }

            return true;
        }
        public static bool Parse_Local_Date_Time_String(ReadOnlyMemory<char> input, out DateTime outDateTime)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-local-date-and-time-string */

            DataStream<char> Stream = new DataStream<char>(input, EOF);

            if (!Consume_Date_Component(Stream, out int year, out int month, out int day))
            {
                outDateTime = DateTime.MinValue;
                return false;
            }

            if (!Stream.atEOF && (Stream.Next == CHAR_T_UPPER || Stream.Next == CHAR_SPACE))
                Stream.Consume();
            else
            {
                outDateTime = DateTime.MinValue;
                return false;
            }

            if (!Consume_Time_Component(Stream, out int hours, out int minutes, out double seconds))
            {
                outDateTime = DateTime.MinValue;
                return false;
            }

            if (!Stream.atEOF)
            {
                outDateTime = DateTime.MinValue;
                return false;
            }

            outDateTime = new DateTime(year, month, day, hours, minutes, 0).AddSeconds(seconds);
            return true;
        }


        public static bool Is_Valid_TimeZone_Offset_String(ReadOnlyMemory<char> input)
        {
            var Stream = new DataStream<char>(input, EOF);
            return Is_Valid_TimeZone_Offset_String(Stream);
        }
        private static bool Is_Valid_TimeZone_Offset_String(DataStream<char> Stream)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-time-zone-offset-string */
            if (Stream.Next == CHAR_Z_UPPER && Stream.NextNext == EOF)
            {
                return true;
            }

            if (Stream.Next != CHAR_PLUS_SIGN && Stream.Next != CHAR_HYPHEN_MINUS)
            {
                return false;
            }

            Stream.Consume();
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> hourDigits) || hourDigits.Length != 2)
            {
                return false;
            }

            int hourParsed = (int)ParsingCommon.Digits_To_Base10(hourDigits);
            if (hourParsed < 0 || hourParsed > 23)
            {
                return false;
            }

            if (Stream.Next != CHAR_COLON)
            {
                return true;
            }

            Stream.Consume();
            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> minuteDigits) || minuteDigits.Length != 2)
            {
                return false;
            }

            int minuteParsed = (int)ParsingCommon.Digits_To_Base10(minuteDigits);
            if (minuteParsed < 0 || minuteParsed > 59)
            {
                return false;
            }

            return true;
        }
        private static bool Consume_TimeZone_Offset_Component(DataStream<char> Stream, out int outTimezoneHours, out int outTimezoneMinutes)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-time-zone-offset-component */

            int timezoneHours = 0;
            int timezoneMinutes = 0;

            if (Stream.Next == CHAR_Z_UPPER)
            {
                timezoneHours = 0;
                timezoneMinutes = 0;
                Stream.Consume();
            }
            else if (Stream.Next == CHAR_PLUS_SIGN || Stream.Next == CHAR_HYPHEN_MINUS)
            {
                int sign = 1;
                if (Stream.Next == CHAR_HYPHEN_MINUS)
                    sign = -1;

                Stream.Consume();
                if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> sDigits))
                {
                    outTimezoneHours = 0;
                    outTimezoneMinutes = 0;
                    return false;
                }

                if (sDigits.Length == 2)
                {
                    timezoneHours = (int)ParsingCommon.Digits_To_Base10(sDigits);

                    if (Stream.atEOF || Stream.Next != CHAR_COLON)
                    {
                        outTimezoneHours = 0;
                        outTimezoneMinutes = 0;
                        return false;
                    }

                    Stream.Consume();
                    if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> minuteDigits) || minuteDigits.Length != 2)
                    {
                        outTimezoneHours = 0;
                        outTimezoneMinutes = 0;
                        return false;
                    }

                    timezoneMinutes = (int)ParsingCommon.Digits_To_Base10(minuteDigits);
                }
                else if (sDigits.Length == 4)
                {
                    timezoneHours = (int)ParsingCommon.Digits_To_Base10(sDigits.Slice(0, 2));
                    timezoneMinutes = (int)ParsingCommon.Digits_To_Base10(sDigits.Slice(2, 2));
                }
                else
                {
                    outTimezoneHours = 0;
                    outTimezoneMinutes = 0;
                    return false;
                }

                if (timezoneHours < 0 || timezoneHours > 23)
                {
                    outTimezoneHours = 0;
                    outTimezoneMinutes = 0;
                    return false;
                }

                if (sign == -1) timezoneHours = -timezoneHours;

                if (timezoneMinutes < 0 || timezoneMinutes > 59)
                {
                    outTimezoneHours = 0;
                    outTimezoneMinutes = 0;
                    return false;
                }

                if (sign == -1) timezoneMinutes = -timezoneMinutes;
            }
            else
            {
                outTimezoneHours = 0;
                outTimezoneMinutes = 0;
                return false;
            }

            outTimezoneHours = timezoneHours;
            outTimezoneMinutes = timezoneMinutes;
            return true;
        }
        public static bool Parse_TimeZone_Offset_String(ReadOnlyMemory<char> input, out int outTimezoneHours, out int outTimezoneMinutes)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-time-zone-offset-string */

            var Stream = new DataStream<char>(input, EOF);

            if (!Consume_TimeZone_Offset_Component(Stream, out int timezoneHours, out int timezoneMinutes))
            {
                outTimezoneHours = 0;
                outTimezoneMinutes = 0;
                return false;
            }

            if (!Stream.atEOF)
            {
                outTimezoneHours = 0;
                outTimezoneMinutes = 0;
                return false;
            }

            outTimezoneHours = timezoneHours;
            outTimezoneMinutes = timezoneMinutes;
            return true;
        }


        public static bool Is_Valid_Global_Date_Time_String(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-global-date-and-time-string */

            var Stream = new DataStream<char>(input, EOF);

            if (Is_Valid_Date_String(input))
            {
                return true;
            }

            if (input.Length == 1 && (input.Span[0] == CHAR_T_UPPER || input.Span[0] == CHAR_SPACE))
            {
                return true;
            }

            if (Is_Valid_Time_String(input))
            {
                return true;
            }

            if (Is_Valid_TimeZone_Offset_String(input))
            {
                return true;
            }

            return false;
        }
        public static bool Parse_Global_Date_Time_String(ReadOnlyMemory<char> input, out DateTimeOffset outTime)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-global-date-and-time-string */

            var Stream = new DataStream<char>(input, EOF);

            if (!Consume_Date_Component(Stream, out int year, out int month, out int day))
            {
                outTime = DateTimeOffset.MinValue;
                return false;
            }

            if (!Stream.atEOF && (Stream.Next == CHAR_T_UPPER || Stream.Next == CHAR_SPACE))
            {
                Stream.Consume();
            }
            else
            {
                outTime = DateTimeOffset.MinValue;
                return false;
            }

            if (!Consume_Time_Component(Stream, out int hours, out int minutes, out double seconds))
            {
                outTime = DateTimeOffset.MinValue;
                return false;
            }

            if (Stream.atEOF)
            {
                outTime = DateTimeOffset.MinValue;
                return false;
            }

            if (!Consume_TimeZone_Offset_Component(Stream, out int timezoneHours, out int timezoneMinutes))
            {
                outTime = DateTimeOffset.MinValue;
                return false;
            }

            if (!Stream.atEOF)
            {
                outTime = DateTimeOffset.MinValue;
                return false;
            }

            var timezoneTimespan = new TimeSpan(timezoneHours, timezoneMinutes, 0);
            var timespan = TimeSpan.FromSeconds(seconds);
            var localTime = new DateTimeOffset(year, month, day, hours, minutes, timespan.Seconds, timespan.Milliseconds, timezoneTimespan);

            outTime = localTime.Subtract(timezoneTimespan);
            return true;
        }


        public static bool Is_Valid_Week_String(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-week-string */

            var Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Week_String(Stream);
        }
        public static bool Is_Valid_Week_String(DataStream<char> Stream)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-week-string */

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> yearDigits) || yearDigits.Length < 4)
            {
                return false;
            }

            var yearParsed = (int)ParsingCommon.Digits_To_Base10(yearDigits);
            if (yearParsed < 1)
            {
                return false;
            }

            if (Stream.Next == CHAR_HYPHEN_MINUS)
                Stream.Consume();
            else
            {
                return false;
            }

            if (Stream.Next == CHAR_W_UPPER)
                Stream.Consume();
            else
            {
                return false;
            }


            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> weekDigits) || weekDigits.Length != 2)
            {
                return false;
            }

            var weekParsed = (int)ParsingCommon.Digits_To_Base10(weekDigits);
            if (weekParsed < 1 || weekParsed > Get_Weeks_In_Year(yearParsed))
            {
                return false;
            }

            return true;
        }
        public static bool Parse_Week_String(ReadOnlyMemory<char> input, out int outWeek, out int outYear)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-week-string */

            var Stream = new DataStream<char>(input, EOF);

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> yearDigits) || yearDigits.Length < 4)
            {
                outWeek = 0;
                outYear = 0;
                return false;
            }

            var yearParsed = (int)ParsingCommon.Digits_To_Base10(yearDigits);
            if (yearParsed < 1)
            {
                outWeek = 0;
                outYear = 0;
                return false;
            }

            if (!Stream.atEOF && Stream.Next == CHAR_HYPHEN_MINUS)
                Stream.Consume();
            else
            {
                outWeek = 0;
                outYear = 0;
                return false;
            }

            if (!Stream.atEOF && Stream.Next == CHAR_W_UPPER)
                Stream.Consume();
            else
            {
                outWeek = 0;
                outYear = 0;
                return false;
            }

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> weekDigits) || weekDigits.Length != 2)
            {
                outWeek = 0;
                outYear = 0;
                return false;
            }

            var weekParsed = (int)ParsingCommon.Digits_To_Base10(weekDigits);
            if (weekParsed < 1 || weekParsed > Get_Weeks_In_Year(yearParsed))
            {
                outWeek = 0;
                outYear = 0;
                return false;
            }

            if (!Stream.atEOF)
            {
                outWeek = 0;
                outYear = 0;
                return false;
            }


            outWeek = weekParsed;
            outYear = yearParsed;
            return true;
        }


        enum EMDisambiguation { Months, Minutes };
        enum EDurationUnit { Years, Months, Weeks, Days, Hours, Minutes, Seconds };
        public static bool Is_Valid_Duration_String(ReadOnlyMemory<char> input, double TimeInSeconds)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-duration-string */

            var Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Duration_String(Stream, TimeInSeconds);
        }
        private static bool Is_Valid_Duration_String(DataStream<char> Stream, double TimeInSeconds)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-duration-string */

            if (Stream.Next == CHAR_P_UPPER)
            {
                Stream.Consume();
                double totalTime = 0;

                if (Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> daysDigits) && daysDigits.Length > 1 && Stream.Next == CHAR_D_UPPER)
                {
                    Stream.Consume();
                    var days = ParsingCommon.Digits_To_Base10_Unsigned(daysDigits);
                    totalTime += days * TIME_SCALE_DAYS;
                }

                if (Stream.Next == CHAR_T_UPPER)
                {
                    Stream.Consume();

                    ulong nextNonDigit = 0;
                    Stream.Scan(ch => !Is_Ascii_Digit(ch), out nextNonDigit);

                    if (Stream.Peek(nextNonDigit) == CHAR_H_UPPER) /* Hours */
                    {
                        if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> hoursDigits) || hoursDigits.Length < 1 || Stream.Next != CHAR_H_UPPER)
                        {
                            return false;
                        }

                        var hours = ParsingCommon.Digits_To_Base10_Unsigned(hoursDigits);
                        totalTime += hours * TIME_SCALE_HOURS;

                        Stream.Consume();
                        Stream.Scan(ch => !Is_Ascii_Digit(ch), out nextNonDigit);
                    }

                    if (Stream.Peek(nextNonDigit) == CHAR_M_UPPER) /* Minutes */
                    {
                        if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> minutesDigits) || minutesDigits.Length < 1 || Stream.Next != CHAR_M_UPPER)
                        {
                            return false;
                        }

                        var minutes = ParsingCommon.Digits_To_Base10_Unsigned(minutesDigits);
                        totalTime += minutes * TIME_SCALE_MINUTES;
                        Stream.Consume();
                    }

                    if (Is_Ascii_Digit(Stream.Next)) /* Seconds */
                    {
                        if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> secondsDigits))
                        {
                            return false;
                        }

                        if (Stream.Next == CHAR_FULL_STOP) /* Fractional part */
                        {
                            Stream.Consume();
                            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> fracDigits) || fracDigits.Length < 1 || fracDigits.Length > 3)
                            {
                                return false;
                            }

                            var seconds = ParsingCommon.ToDecimal(1, secondsDigits.Span, fracDigits.Span, 1, null);
                            totalTime += seconds * TIME_SCALE_SECONDS;
                        }
                        else
                        {
                            var seconds = ParsingCommon.Digits_To_Base10_Unsigned(secondsDigits);
                            totalTime += seconds * TIME_SCALE_SECONDS;
                        }

                        if (Stream.Next != CHAR_S_UPPER)
                        {
                            return false;
                        }
                    }
                }

                if (!MathExt.Feq(TimeInSeconds, totalTime))
                {
                    return false;
                }
            }
            else
            {
                double totalTime = 0;
                while (Is_Valid_Duration_Time_Component(Stream.Clone()))
                {
                    Consume_Duration_Time_Component(Stream, out long outSeconds);
                    totalTime += outSeconds;
                }

                if (!MathExt.Feq(TimeInSeconds, totalTime))
                {
                    return false;
                }
            }

            return true;
        }
        public static bool Parse_Duration_String(ReadOnlyMemory<char> input, out double outDurationSecs)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-duration-string */

            var Stream = new DataStream<char>(input, EOF);
            bool result = Parse_Duration_String(Stream, out double durationSecs);
            outDurationSecs = durationSecs;
            return result;
        }
        private static bool Parse_Duration_String(DataStream<char> Stream, out double outDurationSecs)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-duration-string */
            double months = 0, seconds = 0;
            int componentCount = 0;
            EMDisambiguation m_disambiguator = EMDisambiguation.Minutes;

            Stream.Consume_While(Is_Ascii_Whitespace);

            if (Stream.atEOF)
            {
                outDurationSecs = double.NaN;
                return false;
            }

            if (Stream.Next == CHAR_P_UPPER)
            {
                Stream.Consume();
                m_disambiguator = EMDisambiguation.Months;
                Stream.Consume_While(Is_Ascii_Whitespace);
            }

            double N = 0;
            while (true)
            {
                if (Stream.atEOF) break;
                EDurationUnit units;

                if (Stream.Next == CHAR_T_UPPER)
                {
                    Stream.Consume();
                    m_disambiguator = EMDisambiguation.Minutes;
                    Stream.Consume_While(Is_Ascii_Whitespace);
                }

                if (Stream.Next == CHAR_FULL_STOP)
                {
                    N = 0;
                }
                else if (Is_Ascii_Digit(Stream.Next))
                {
                    if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> outDigits))
                    {
                        outDurationSecs = double.NaN;
                        return false;
                    }

                    N = ParsingCommon.Digits_To_Base10_Unsigned(outDigits);
                }
                else
                {
                    outDurationSecs = double.NaN;
                    return false;
                }

                if (Stream.atEOF)
                {
                    outDurationSecs = double.NaN;
                    return false;
                }

                if (Stream.Next == CHAR_FULL_STOP)
                {
                    Stream.Consume();
                    if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> outDigits))
                    {
                        outDurationSecs = double.NaN;
                        return false;
                    }

                    if (outDigits.IsEmpty)
                    {
                        outDurationSecs = double.NaN;
                        return false;
                    }

                    long s = ParsingCommon.Digits_To_Base10(outDigits);
                    double fraction = s / Math.Pow(10, outDigits.Length);
                    N += fraction;

                    Stream.Consume_While(Is_Ascii_Whitespace);
                    if (Stream.atEOF)
                    {
                        outDurationSecs = double.NaN;
                        return false;
                    }

                    if (Stream.Next != CHAR_S_LOWER && Stream.Next != CHAR_S_UPPER)
                    {
                        outDurationSecs = double.NaN;
                        return false;
                    }

                    units = EDurationUnit.Seconds;
                }
                else
                {
                    Stream.Consume_While(Is_Ascii_Whitespace);

                    switch (Stream.Next)
                    {
                        case CHAR_Y_LOWER:
                        case CHAR_Y_UPPER:
                            {
                                units = EDurationUnit.Years;
                                m_disambiguator = EMDisambiguation.Months;
                            }
                            break;
                        case CHAR_W_LOWER:
                        case CHAR_W_UPPER:
                            {
                                units = EDurationUnit.Weeks;
                                m_disambiguator = EMDisambiguation.Minutes;
                            }
                            break;
                        case CHAR_D_LOWER:
                        case CHAR_D_UPPER:
                            {
                                units = EDurationUnit.Days;
                                m_disambiguator = EMDisambiguation.Minutes;
                            }
                            break;
                        case CHAR_H_LOWER:
                        case CHAR_H_UPPER:
                            {
                                units = EDurationUnit.Hours;
                                m_disambiguator = EMDisambiguation.Minutes;
                            }
                            break;
                        case CHAR_S_LOWER:
                        case CHAR_S_UPPER:
                            {
                                units = EDurationUnit.Seconds;
                                m_disambiguator = EMDisambiguation.Minutes;
                            }
                            break;
                        case CHAR_M_LOWER:
                        case CHAR_M_UPPER:
                            {
                                if (m_disambiguator == EMDisambiguation.Months) units = EDurationUnit.Months;
                                else units = EDurationUnit.Minutes;
                            }
                            break;
                        default:
                            {
                                outDurationSecs = double.NaN;
                                return false;
                            }
                    }

                    componentCount++;
                    double multiplier = 1;

                    if (units == EDurationUnit.Years)
                    {
                        multiplier *= 12;
                        units = EDurationUnit.Months;
                    }

                    if (units == EDurationUnit.Months)
                    {
                        months += N * multiplier;
                    }
                    else
                    {
                        if (units == EDurationUnit.Weeks)
                        {
                            multiplier *= 7;
                            units = EDurationUnit.Days;
                        }

                        if (units == EDurationUnit.Days)
                        {
                            multiplier *= 24;
                            units = EDurationUnit.Hours;
                        }

                        if (units == EDurationUnit.Hours)
                        {
                            multiplier *= 60;
                            units = EDurationUnit.Minutes;
                        }

                        if (units == EDurationUnit.Minutes)
                        {
                            multiplier *= 60;
                            units = EDurationUnit.Seconds;
                        }

                        seconds += N * multiplier;
                    }

                    Stream.Consume_While(Is_Ascii_Whitespace);
                }
            }

            if (componentCount == 0)
            {
                outDurationSecs = double.NaN;
                return false;
            }

            if (months != 0)
            {
                outDurationSecs = double.NaN;
                return false;
            }

            outDurationSecs = seconds;
            return true;
        }


        public static bool Is_Valid_Duration_Time_Component(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#duration-time-component */

            var Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Duration_Time_Component(Stream);
        }
        private static bool Is_Valid_Duration_Time_Component(DataStream<char> Stream)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#duration-time-component */

            Stream.Consume_While(Is_Ascii_Whitespace);

            if (!Is_Ascii_Digit(Stream.Next))
            {
                return false;
            }

            ulong nextNonDigit = 0;
            if (!Stream.Scan(ch => !Is_Ascii_Digit(ch), out nextNonDigit))
            {
                return false;
            }

            var nonDigit = Stream.Peek(nextNonDigit);
            switch (nonDigit)
            {
                case CHAR_W_UPPER:
                case CHAR_W_LOWER:
                case CHAR_D_UPPER:
                case CHAR_D_LOWER:
                case CHAR_H_UPPER:
                case CHAR_H_LOWER:
                case CHAR_M_UPPER:
                case CHAR_M_LOWER:
                case CHAR_S_UPPER:
                case CHAR_S_LOWER:
                    break;
                default:
                    return false;
            }

            if (!Stream.Consume_While(Is_Ascii_Digit))
            {
                return false;
            }

            Stream.Consume();
            Stream.Consume_While(Is_Ascii_Whitespace);

            return true;
        }
        public static bool Consume_Duration_Time_Component(ReadOnlyMemory<char> input, out double outTimeInSeconds)
        {
            var Stream = new DataStream<char>(input, EOF);
            bool result = Consume_Duration_Time_Component(Stream, out double TimeInSeconds);
            outTimeInSeconds = TimeInSeconds;
            return result;
        }
        private static bool Consume_Duration_Time_Component(DataStream<char> Stream, out double outTimeInSeconds)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#duration-time-component */

            Stream.Consume_While(Is_Ascii_Whitespace);

            if (!Is_Ascii_Digit(Stream.Next))
            {
                outTimeInSeconds = double.NaN;
                return false;
            }

            if (!Stream.Scan(ch => !Is_Ascii_Digit(ch), out ulong nextNonDigit))
            {
                outTimeInSeconds = double.NaN;
                return false;
            }

            long scale = 1;
            var nonDigit = Stream.Peek(nextNonDigit);

            switch (nonDigit)
            {
                case CHAR_W_UPPER:
                case CHAR_W_LOWER:
                    {
                        scale = TIME_SCALE_WEEKS;
                    }
                    break;
                case CHAR_D_UPPER:
                case CHAR_D_LOWER:
                    {
                        scale = TIME_SCALE_DAYS;
                    }
                    break;
                case CHAR_H_UPPER:
                case CHAR_H_LOWER:
                    {
                        scale = TIME_SCALE_HOURS;
                    }
                    break;
                case CHAR_M_UPPER:
                case CHAR_M_LOWER:
                    {
                        scale = TIME_SCALE_MINUTES;
                    }
                    break;
                case CHAR_S_UPPER:
                case CHAR_S_LOWER:
                    {
                        scale = TIME_SCALE_SECONDS;
                    }
                    break;
                default:
                    {
                        outTimeInSeconds = double.NaN;
                        return false;
                    }
            }

            if (!Stream.Consume_While(Is_Ascii_Digit, out ReadOnlyMemory<char> unitDigits) || unitDigits.Length < 1)
            {
                outTimeInSeconds = double.NaN;
                return false;
            }
            Stream.Consume();
            Stream.Consume_While(Is_Ascii_Whitespace);

            var unitsParsed = ParsingCommon.Digits_To_Base10(unitDigits);

            outTimeInSeconds = unitsParsed * scale;
            return true;
        }


        public static bool Is_Valid_Date_With_Optional_Time_String(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-date-string-with-optional-time */

            if (Is_Valid_Date_String(input))
            {
                return true;
            }

            if (Is_Valid_Global_Date_Time_String(input))
            {
                return true;
            }

            return false;
        }


        public static bool Parse_Date_Or_Time_String(ReadOnlyMemory<char> input, out DateTimeOffset outDateTime)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-date-or-time-string */

            var Stream = new DataStream<char>(input, EOF);
            bool result = Parse_Date_Or_Time_String(Stream, out DateTimeOffset dateTime);
            outDateTime = dateTime;
            return result;
        }
        private static bool Parse_Date_Or_Time_String(DataStream<char> Stream, out DateTimeOffset outDateTime)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#parse-a-date-or-time-string */

            bool bDatePresent = true, bTimePresent = true;
            int year = 0, month = 0, day = 0;
            int hours = 0, minutes = 0;
            double seconds = 0;
            int timezoneHours = 0, timezoneMinutes = 0;

            var StartPosition = Stream.Position;
            if (!Consume_Date_Component(Stream, out year, out month, out day))
            {
                bDatePresent = false;
            }

            if (bDatePresent && !Stream.atEOF && (Stream.Next == CHAR_T_UPPER || Stream.Next == CHAR_SPACE))
            {
                Stream.Consume();
            }
            else if (bDatePresent && (Stream.atEOF || !(Stream.Next == CHAR_T_UPPER && Stream.Next == CHAR_SPACE)))
            {
                bTimePresent = false;
            }
            else
            {
                if (!bDatePresent)
                {
                    Stream.Seek(StartPosition);
                }
            }

            if (bTimePresent)
            {
                if (!Consume_Time_Component(Stream, out hours, out minutes, out seconds))
                {
                    outDateTime = DateTimeOffset.MinValue;
                    return false;
                }
            }

            if (bDatePresent && bTimePresent)
            {
                if (Stream.atEOF)
                {
                    outDateTime = DateTimeOffset.MinValue;
                    return false;
                }

                if (!Consume_TimeZone_Offset_Component(Stream, out timezoneHours, out timezoneMinutes))
                {
                    outDateTime = DateTimeOffset.MinValue;
                    return false;
                }
            }

            if (!Stream.atEOF)
            {
                outDateTime = DateTimeOffset.MinValue;
                return false;
            }

            if (bDatePresent && !bTimePresent)
            {
                outDateTime = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
                return true;
            }
            else if (bTimePresent && !bDatePresent)
            {
                outDateTime = new DateTime(0, 0, 0, hours, minutes, 0, DateTimeKind.Utc).AddSeconds(seconds);
                return true;
            }


            var timezone = new TimeSpan(timezoneHours, timezoneMinutes, 0);
            var tmp = new DateTimeOffset(year, month, day, hours, minutes, 0, timezone).AddSeconds(seconds);
            outDateTime = tmp.Subtract(timezone);
            return true;
        }


        public static bool Is_Valid_Simple_Color(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-simple-colour */

            var Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Simple_Color(Stream);
        }
        private static bool Is_Valid_Simple_Color(DataStream<char> Stream)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-simple-colour */

            if (Stream.Length != 7)
            {
                return false;
            }

            if (Stream.Next != CHAR_NUMBER_SIGN)
            {
                return false;
            }

            Stream.Consume();

            if (!Stream.Consume_While(Is_Ascii_Hex_Digit, out ReadOnlyMemory<char> outHex) || outHex.Length != 6)
            {
                return false;
            }

            return true;
        }


        public static bool Is_Valid_Lowercase_Simple_Color(ReadOnlyMemory<char> input)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-lowercase-simple-colour */

            var Stream = new DataStream<char>(input, EOF);
            return Is_Valid_Lowercase_Simple_Color(Stream);
        }
        private static bool Is_Valid_Lowercase_Simple_Color(DataStream<char> Stream)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#valid-lowercase-simple-colour */

            if (Stream.Length != 7)
            {
                return false;
            }

            if (Stream.Next != CHAR_NUMBER_SIGN)
            {
                return false;
            }

            Stream.Consume();

            if (!Stream.Consume_While(Is_Ascii_Hex_Digit_Lower, out ReadOnlyMemory<char> outHex) || outHex.Length != 6)
            {
                return false;
            }

            return true;
        }

        public static bool Parse_Simple_Color_String(ReadOnlyMemory<char> input, out SimpleColor outColor)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#rules-for-parsing-simple-colour-values */

            var Stream = new DataStream<char>(input, EOF);
            var result = Parse_Simple_Color_String(Stream, out SimpleColor color);
            outColor = color;
            return result;
        }
        private static bool Parse_Simple_Color_String(DataStream<char> Stream, out SimpleColor outColor)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#rules-for-parsing-simple-colour-values */

            if (Stream.Length != 7)
            {
                outColor = SimpleColor.MinValue;
                return false;
            }

            if (Stream.Next != CHAR_NUMBER_SIGN)
            {
                outColor = SimpleColor.MinValue;
                return false;
            }

            Stream.Consume();

            if (!Stream.Consume_While(Is_Ascii_Hex_Digit, out ReadOnlyMemory<char> outHex) || outHex.Length != 6)
            {
                outColor = SimpleColor.MinValue;
                return false;
            }

            var R = (byte)ParsingCommon.Parse_Hex(outHex.Slice(0, 2));
            var G = (byte)ParsingCommon.Parse_Hex(outHex.Slice(2, 2));
            var B = (byte)ParsingCommon.Parse_Hex(outHex.Slice(4, 2));

            outColor = new SimpleColor(R, G, B);
            return true;
        }


        public static bool Parse_Legacy_Color_String(ReadOnlyMemory<char> input, out SimpleColor outColor)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#rules-for-parsing-a-legacy-colour-value */

            var Stream = new DataStream<char>(input, EOF);
            var result = Parse_Legacy_Color_String(Stream, out SimpleColor color);
            outColor = color;
            return result;
        }
        private static bool Parse_Legacy_Color_String(DataStream<char> Stream, out SimpleColor outColor)
        {/* Docs: https://html.spec.whatwg.org/multipage/common-microsyntaxes.html#rules-for-parsing-a-legacy-colour-value */

            Stream = new DataStream<char>(StringCommon.Trim(Stream.AsMemory()), EOF);

            var mem = Stream.AsMemory();
            if (StringCommon.StriEq(mem.Span, "transparent".AsSpan()))
            {
                outColor = SimpleColor.MinValue;
                return false;
            }

            /* Check and see if this is a named color */
            if (Lookup.TryEnum(StringCommon.Transform(mem, To_ASCII_Lower_Alpha), out EColor outEnumValue))
            {
                if (Lookup.TryData(outEnumValue, out EnumData outColorData))
                {
                    outColor = new SimpleColor(outColorData.Data[1], outColorData.Data[2], outColorData.Data[3]);
                    return true;
                }
            }

            if (Stream.Length == 4 && Stream.Next == CHAR_NUMBER_SIGN && !Stream.Scan(c => !Is_Ascii_Hex_Digit(c), out _, 1))
            {
                Stream.Consume();
                int _r = 17 * (int)Ascii_Hex_To_Value(Stream.Consume());
                int _g = 17 * (int)Ascii_Hex_To_Value(Stream.Consume());
                int _b = 17 * (int)Ascii_Hex_To_Value(Stream.Consume());

                outColor = new SimpleColor(_r, _g, _b);
                return true;
            }

            string input = StringCommon.Replace(mem, FilterUnicodeOOB.Instance, "00".AsSpan(), true);

            int startOffset = input[0] == CHAR_NUMBER_SIGN ? 1 : 0;
            input = input.Substring(startOffset, Math.Min(input.Length, 128));

            /* 10) Replace any character in input that is not an ASCII hex digit with the character U+0030 DIGIT ZERO (0). */
            input = StringCommon.Transform(input.AsMemory(), (c) => Is_Ascii_Hex_Digit(c) ? c : CHAR_DIGIT_0);
            /* 11) While input's length is zero or not a multiple of three, append a U+0030 DIGIT ZERO (0) character to input. */
            var remainder = input.Length == 0 ? 3 : input.Length % 3;
            if (remainder > 0) input = string.Concat(input, new string(CHAR_DIGIT_0, remainder));

            var length = input.Length / 3;
            string[] split = new string[3] {
                input.Substring(0, length),
                input.Substring(length, length),
                input.Substring(length*2, length),
            };

            /* 13) If length is greater than 8, then remove the leading length-8 characters in each component, and let length be 8. */
            if (length > 8)
            {
                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = split[i].Substring(split[i].Length - 8);
                }

                length = 8;
            }

            /* 14) While length is greater than two and the first character in each component is a U+0030 DIGIT ZERO (0) character, remove that character and reduce length by one. */
            while (length > 2 && split[0][0] == CHAR_DIGIT_0 && split[1][0] == CHAR_DIGIT_0 && split[2][0] == CHAR_DIGIT_0)
            {
                length--;

                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = split[i].Substring(1);
                }
            }

            /* 15) If length is still greater than two, truncate each component, leaving only the first two characters in each. */
            if (length > 2)
            {
                for (int i = 0; i < split.Length; i++)
                {
                    split[i] = split[i].Substring(0, 2);
                }
            }

            int r = (int)ParsingCommon.Parse_Hex(split[0].AsMemory());
            int g = (int)ParsingCommon.Parse_Hex(split[1].AsMemory());
            int b = (int)ParsingCommon.Parse_Hex(split[2].AsMemory());

            outColor = new SimpleColor(r, g, b);
            return true;
        }
        #endregion
    }

}

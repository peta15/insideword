using System;

namespace InsideWordMVCWeb.Models.Utility
{
    /// <summary>
    /// Summary description for Utility
    /// </summary>
    public static class InsideWordUtility
    {
        static readonly private string charArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        static public string AlphaNumeric(int index)
        {
            return charArray[index % charArray.Length].ToString();
        }

        public static int Digits(int number)
        {
            return (int)(Math.Log10(number) + 1);
        }

        public static string FormatVotes(long? votes)
        {
            return (votes ?? 0).ToString("+#;-#;0");
        }

        // format pretty dates such as "1 minute ago"
        public static string GetPrettyDate(DateTime d)
        {
            string prettyDate;
            // Get time span elapsed since the date.
            TimeSpan s = DateTime.UtcNow.Subtract(d);

            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;

            // Handle same-day times.
            if (dayDiff == 0)
            {
                // Get total number of seconds elapsed.
                int secDiff = (int)s.TotalSeconds;

                // Less than one minute ago.
                if (secDiff < 60)
                {
                    prettyDate = "just now";
                }
                // Less than 2 minutes ago.
                else if (secDiff < 120)
                {
                    prettyDate = "1 minute ago";
                }
                // Less than one hour ago.
                else if (secDiff < 3600)
                {
                    prettyDate = string.Format("{0:0} minutes ago",
                                                s.TotalMinutes);
                }
                // Less than 2 hours ago.
                else if (secDiff < 7200)
                {
                    prettyDate = "1 hour ago";
                }
                // Less than one day ago (secDiff < 86400)
                else
                {
                    prettyDate = string.Format("{0:0} hours ago",
                                               s.TotalHours);
                }
            }
            else
            {
                // Handle previous days.
                if (dayDiff == 1)
                {
                    prettyDate = "yesterday";
                }
                else if (dayDiff < 7)
                {
                    prettyDate = string.Format("{0:0} days ago",
                                                 s.TotalDays);
                }
                // previous weeks
                else if (dayDiff < 14)
                {
                    prettyDate = "last week";
                }
                else if (dayDiff < 31)
                {

                    prettyDate = string.Format("{0:0} weeks ago",
                                                s.TotalDays / 7.0);
                }
                // previous months
                else if (dayDiff < 61)
                {
                    prettyDate = "1 month ago";
                }
                else if (dayDiff < 365)
                {
                    prettyDate = string.Format("{0:0} months ago",
                                               s.TotalDays / 30.5);
                }
                // previous years
                else if (dayDiff < 730)
                {
                    prettyDate = "a year ago";
                }
                else
                {
                    prettyDate = string.Format("{0:0} years ago",
                                               s.TotalDays / 365.0);
                }
            }

            return prettyDate;
        }
    }
}

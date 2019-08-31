using System;
using ChortkivBot.Contracts.Helpers;

namespace ChortkivBot.Travel.Helpers
{
    public class DateFormatter : IDateFormatter
    {
        public string BlaBlaCarDate(DateTime date)
        {
            var timeString = date.ToString("HH':'mm':'ss");
            var dateString = date.ToString("yyyy-MM-dd");
            return $"{dateString} {timeString}";
        }

        public string BusDate(DateTime date)
        {
            return date.ToString("dd.MM.yy");
        }
    }
}

using System;

namespace Domain
{
    public static class CurrentTime
    {
        private static readonly TimeZoneInfo SeAsiaTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

        public static DateTime GetCurrentTime()
        {
            var timeUtc = DateTime.UtcNow;
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, SeAsiaTimeZone);
        }
        
        public static DateTime ToUtcTime(DateTime vnTimeToConverted)
        {
            return TimeZoneInfo.ConvertTimeToUtc(vnTimeToConverted, SeAsiaTimeZone);
        }
        
        public static DateTime ToLocalTime(DateTime utcTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
        }
    }
}
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
        
        public static DateTime ToLocalTime(DateTime timeToConverted)
        {
            var timeUtc = TimeZoneInfo.ConvertTimeToUtc(timeToConverted, TimeZoneInfo.Local);
            return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.Local);
        }
    }
}
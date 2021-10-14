using System;

namespace Domain
{
	public static class CurrentTime
	{
		private static readonly TimeZoneInfo SeAsiaTimeZone =
			TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

		public static DateTime GetCurrentTime()
		{
			DateTime timeUtc = DateTime.UtcNow;
			return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, SeAsiaTimeZone);
		}

		public static DateTime ToLocalTime(DateTime timeToConverted)
		{
			DateTime timeUtc = timeToConverted.AddHours(-7);
			return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, TimeZoneInfo.Local);
		}
	}
}
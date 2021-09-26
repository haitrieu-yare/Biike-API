using System;

namespace Domain
{
	public static class CurrentTime
	{
		public static DateTime GetCurrentTime()
		{
			var timeUtc = DateTime.UtcNow;
			var SEAsiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, SEAsiaTimeZone);
		}
	}
}
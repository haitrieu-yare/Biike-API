using System;

namespace Domain
{
	public static class CurrentTime
	{
		public static DateTime GetCurrentTime()
		{
			DateTime timeUtc = DateTime.UtcNow;
			TimeZoneInfo seAsiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
			return TimeZoneInfo.ConvertTimeFromUtc(timeUtc, seAsiaTimeZone);
		}
	}
}
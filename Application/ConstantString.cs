namespace Application
{
	public static class ConstantString
	{
		public const string OneTimeJob = "OneTimeJob";
		public const string ReoccurredJob = "ReoccurredJob";
		public static string GetJobNameAutoCancellation(int tripId)
		{
			return $"Trip {tripId} Auto Cancellation";
		}
		public static string GetTriggerNameAutoCancellation(int tripId)
		{
			return $"Trip {tripId} Auto Cancellation Trigger";
		}
	}
}
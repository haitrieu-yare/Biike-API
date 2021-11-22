using System.Collections.Generic;

namespace Application
{
    public static class Constant
    {
        public const int MaxTripCount = 30;
        public const string CouldNotGetUserRole = "Could not get user's role.";

        public const string CouldNotGetIdOfUserSentRequest = "Could not get the ID of the user that sent the request.";

        // User fo GET
        public const string DidNotHavePermissionToAccess = "User did not have permission to access this resource.";

        // Use for DELETE
        public const string DidNotHavePermissionToMakeRequest = "User did not have permission to make this request.";

        // Use for POST, PUT
        public const string NotSameUserId =
            "ID of the user sent the request is not the same as ID of the user in the request's body.";

        public const string OneTimeJob = "OneTimeJob";
        public const string ReoccurredJob = "ReoccurredJob";
        public const string TripCompletionPoint = "Điểm thưởng chuyến đi";
        public const string TripFeedbackPoint = "Điểm thưởng đánh giá";
        public const string TripTipPoint = "Điểm tặng cho người chở";
        public const string RedemptionUsage = "Sử dụng điểm đổi voucher";

        public static string OnlyRole(string role)
        {
            return $"Only {role} can send request to this endpoint.";
        }

        public static string GetJobNameAutoCancellation(int tripId)
        {
            return $"Trip {tripId} Auto Cancellation";
        }

        public static string GetTriggerNameAutoCancellation(int tripId, string triggerPurpose)
        {
            return $"Trip {tripId} Auto Cancellation Trigger {triggerPurpose}";
        }
    }
    public static class Color
    {
        public const string Green = "33691E";
        public const string Yellow = "FFC400";
        public const string Purple = "7B1FA2";
        public const string Red = "EF5350";
        public const string Pink = "FF80AB";
        public const string Aqua = "00838F";
        public const string LightBlue = "80DEEA";
        public const string Blue = "0288D1";
        public const string StrongGreen = "388E3C";
        public const string LightGreen = "558B2F";
        public const string White = "FFFFFF";

        public static readonly IReadOnlyList<string> ColorList = new List<string>
        {
            Green,
            StrongGreen,
            LightGreen,
            Yellow,
            Pink,
            Purple,
            Red,
            Aqua,
            LightBlue,
            Blue
        };
    }
}
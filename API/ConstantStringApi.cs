namespace API
{
	public static class ConstantStringApi
	{
		public const string CouldNotGetUserRole = "Could not get user's role.";
		public const string CouldNotGetIdOfUserSentRequest = "Could not get the ID of the user that sent the request.";
		// User fo GET
		public const string DidNotHavePermissionToAccess = "User did not have permission to access this resource.";
		// Use for DELETE
		public const string DidNotHavePermissionToMakeRequest = "User did not have permission to make this request.";
		// Use for POST, PUT
		public const string NotSameUserId =
			"ID of the user sent the request is not the same as ID of the user in the request's body.";

		public static string OnlyRole(string role)
		{
			return $"Only {role} can send request to this endpoint.";
		}
	}
}
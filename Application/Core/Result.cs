namespace Application.Core
{
	public class Result<T>
	{
		public bool IsSuccess { get; set; }
		public string NewResourceId { get; set; } = string.Empty;
		public PaginationDto? PaginationDto { get; set; }
		public bool IsUnauthorized { get; set; } = false;
		public T? Value { get; set; }
		public string? ErrorMessage { get; set; }
		public string? NotFoundMessage { get; set; }
		public string? SuccessMessage { get; set; }

		public static Result<T> Success(T value, string successMessage)
			=> new Result<T> { IsSuccess = true, Value = value, SuccessMessage = successMessage };
		public static Result<T> Success(T value, string successMessage, PaginationDto paginationDto)
			=> new Result<T>
			{
				IsSuccess = true,
				Value = value,
				SuccessMessage = successMessage,
				PaginationDto = paginationDto,
			};
		public static Result<T> Success(T value, string successMessage, string newResourceId)
			=> new Result<T>
			{
				IsSuccess = true,
				Value = value,
				SuccessMessage = successMessage,
				NewResourceId = newResourceId,
			};
		public static Result<T> Failure(string errorMessage)
			=> new Result<T> { IsSuccess = false, ErrorMessage = errorMessage };
		public static Result<T> Unauthorized()
			=> new Result<T> { IsSuccess = false, IsUnauthorized = true };
		public static Result<T> NotFound(string notFoundMessage)
			=> new Result<T> { IsSuccess = false, NotFoundMessage = notFoundMessage };
	}
}
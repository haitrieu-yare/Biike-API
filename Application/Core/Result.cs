namespace Application.Core
{
	public class Result<T>
	{
		public bool IsSuccess { get; set; }
		public string NewResourceId { get; set; } = string.Empty;
		public PaginationDto? PaginationDto { get; set; }
		public bool IsUnauthorized { get; set; }
		public T? Value { get; set; }
		public string? ErrorMessage { get; set; }
		public string? NotFoundMessage { get; set; }
		public string? SuccessMessage { get; set; }

		public static Result<T> Success(T value, string successMessage)
		{
			return new Result<T> { IsSuccess = true, Value = value, SuccessMessage = successMessage };
		}

		public static Result<T> Success(T value, string successMessage, PaginationDto paginationDto)
		{
			return new Result<T>
			{
				IsSuccess = true,
				Value = value,
				SuccessMessage = successMessage,
				PaginationDto = paginationDto
			};
		}

		public static Result<T> Success(T value, string successMessage, string newResourceId)
		{
			return new Result<T>
			{
				IsSuccess = true,
				Value = value,
				SuccessMessage = successMessage,
				NewResourceId = newResourceId
			};
		}

		public static Result<T> Failure(string errorMessage)
		{
			return new Result<T> { IsSuccess = false, ErrorMessage = errorMessage };
		}

		public static Result<T> Unauthorized()
		{
			return new Result<T> { IsSuccess = false, IsUnauthorized = true };
		}

		public static Result<T> NotFound(string notFoundMessage)
		{
			return new Result<T> { IsSuccess = false, NotFoundMessage = notFoundMessage };
		}
	}
}
namespace Application.Core
{
    public class Result<T>
    {
        public bool IsSuccess { get; private init; }
        public PaginationDto? PaginationDto { get; private init; }
        public string? NewResourceId { get; private init; }
        public bool IsUnauthorized { get; private init; }
        public T? Value { get; private init; }
        public string? ErrorMessage { get; private init; }
        public string? UnauthorizedMessage { get; private init; }
        public string? NotFoundMessage { get; private init; }
        public string? SuccessMessage { get; private init; }

        public static Result<T> Success(T value, string successMessage)
        {
            return new Result<T> {IsSuccess = true, Value = value, SuccessMessage = successMessage};
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
            return new Result<T> {IsSuccess = false, ErrorMessage = errorMessage};
        }

        public static Result<T> Unauthorized(string unauthorizedMessage)
        {
            return new Result<T> {IsSuccess = false, IsUnauthorized = true, UnauthorizedMessage = unauthorizedMessage};
        }

        public static Result<T> NotFound(string notFoundMessage)
        {
            return new Result<T> {IsSuccess = false, NotFoundMessage = notFoundMessage};
        }
    }
}
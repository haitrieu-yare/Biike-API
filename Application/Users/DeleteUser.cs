using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using FirebaseAdmin.Auth;
using MediatR;
using Persistence;

namespace Application.Users
{
	public class DeleteUser
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int UserId { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<DeleteUser> _logger;
			public Handler(DataContext context, ILogger<DeleteUser> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var user = await _context.User
						.FindAsync(new object[] { request.UserId }, cancellationToken);

					if (user == null) return null!;

					UserRecordArgs userRecordArgs = new UserRecordArgs()
					{
						Uid = request.UserId.ToString(),
						Disabled = !user.IsDeleted
					};

					// Delete on Firebase
					try
					{
						await FirebaseAuth.DefaultInstance.UpdateUserAsync(userRecordArgs, cancellationToken);
					}
					catch (FirebaseAuthException e)
					{
						_logger.LogError($"Error delete user on Firebase. {e.InnerException?.Message ?? e.Message}");
						return Result<Unit>.Failure(
							$"Error delete user on Firebase. {e.InnerException?.Message ?? e.Message}");
					}

					user.IsDeleted = !user.IsDeleted;

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation($"Failed to delete user with userId {request.UserId}.");
						return Result<Unit>.Failure($"Failed to delete user with userId {request.UserId}.");
					}
					else
					{
						_logger.LogInformation($"Successfully deleted user with userId {request.UserId}.");
						return Result<Unit>.Success(
							Unit.Value, $"Successfully deleted user with userId {request.UserId}.");
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}
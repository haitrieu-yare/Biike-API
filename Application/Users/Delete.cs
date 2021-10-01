using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Enums;
using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
	public class Delete
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int Id { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Delete> _logger;
			public Handler(DataContext context, ILogger<Delete> logger)
			{
				_logger = logger;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var user = await _context.User
						.FindAsync(new object[] { request.Id }, cancellationToken);
					if (user == null) return null!;

					#region Delete on Firebase
					try
					{
						await FirebaseAuth.DefaultInstance.DeleteUserAsync(user.Id.ToString());
					}
					catch (FirebaseAuthException e)
					{
						_logger.LogError($"Error delete user: {e.Message}");
						return Result<Unit>.Failure($"Error delete user: {e.Message}");
					}
					#endregion

					user.Status = (int)UserStatus.Deleted;

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to delete user");
						return Result<Unit>.Failure("Failed to delete user");
					}
					else
					{
						_logger.LogInformation("Successfully deleted user");
						return Result<Unit>.Success(Unit.Value, "Successfully deleted user");
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.Message);
					return Result<Unit>.Failure(ex.Message);
				}
			}
		}
	}
}
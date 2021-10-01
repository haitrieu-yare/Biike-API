using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using FirebaseAdmin.Auth;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
	public class DeleteFireBaseUser
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int Id { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<DeleteFireBaseUser> _logger;
			public Handler(DataContext context, ILogger<DeleteFireBaseUser> logger)
			{
				_logger = logger;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var users = await _context.User.ToListAsync();
					if (users == null) return null!;

					var listUserId = new List<string>();
					foreach (var user in users)
					{
						listUserId.Add(user.Id.ToString());
					}

					try
					{
						DeleteUsersResult result = await FirebaseAuth.DefaultInstance
							.DeleteUsersAsync(listUserId);
					}
					catch (FirebaseAuthException e)
					{
						_logger.LogError($"Error delete user: {e.Message}");
						return Result<Unit>.Failure($"Error delete user: {e.Message}");
					}

					_logger.LogInformation("Successfully deleted firebase's users");
					return Result<Unit>.Success(Unit.Value, "Successfully deleted firebase's users");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled");
				}
			}
		}
	}
}
using System.Threading;
using System.Threading.Tasks;
using Application.Users.DTOs;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
	public class EditLoginDevice
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int Id { get; set; }
			public UserLoginDeviceDTO UserLoginDeviceDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<EditLoginDevice> _logger;
			public Handler(DataContext context, ILogger<EditLoginDevice> logger)
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

					user.LastLoginDevice = request.UserLoginDeviceDTO.LastLoginDevice;
					user.LastTimeLogin = request.UserLoginDeviceDTO.LastTimeLogin;

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update user's login device");
						return Result<Unit>.Failure("Failed to update user's login device");
					}
					else
					{
						_logger.LogInformation("Successfully updated user's login device");
						return Result<Unit>.Success(Unit.Value);
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
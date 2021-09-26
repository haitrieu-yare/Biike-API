using System.Threading;
using System.Threading.Tasks;
using Application.Users.DTOs;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
	public class EditProfile
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int Id { get; set; }
			public UserProfileEditDTO UserProfileEditDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<EditProfile> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<EditProfile> logger)
			{
				_logger = logger;
				_mapper = mapper;
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

					_mapper.Map(request.UserProfileEditDTO, user);

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update user's profile");
						return Result<Unit>.Failure("Failed to update user's profile");
					}
					else
					{
						_logger.LogInformation("Successfully updated user's profile");
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
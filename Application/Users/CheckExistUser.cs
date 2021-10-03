using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using AutoMapper;
using Persistence;
using Application.Core;
using Application.Users.DTOs;

namespace Application.Users
{
	public class CheckExistUser
	{
		public class Command : IRequest<Result<Unit>>
		{
			public UserExistDTO UserExistDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<CheckExistUser> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<CheckExistUser> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var user = await _context.User
						.Where(u => u.Email == request.UserExistDTO.Email ||
							u.PhoneNumber == request.UserExistDTO.PhoneNumber)
						.SingleOrDefaultAsync(cancellationToken);

					if (user != null)
					{
						_logger.LogInformation("User with the same email or phone number has already existed.");
						return Result<Unit>.Failure("User with the same email or phone number has already existed.");
					}

					_logger.LogInformation("User doesn't exist.");
					return Result<Unit>.Success(Unit.Value, "User doesn't exist.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<Unit>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
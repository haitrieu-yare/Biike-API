using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

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
			private readonly Hashing _hashing;
			public Handler(DataContext context, IMapper mapper, ILogger<CheckExistUser> logger,
				Hashing hashing)
			{
				_hashing = hashing;
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var oldUser = await _context.User
						.Where(u => u.Email == request.UserExistDTO.Email ||
							u.PhoneNumber == request.UserExistDTO.PhoneNumber)
						.SingleOrDefaultAsync(cancellationToken);
					if (oldUser != null)
					{
						_logger.LogInformation("User with the same email or phone number has already existed");
						return Result<Unit>.Failure("User with the same email or phone number has already existed");
					}

					_logger.LogInformation("User doesn't exist");
					return Result<Unit>.Success(Unit.Value, "User doesn't exist");
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
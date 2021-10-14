using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
	public class DetailSelfUser
	{
		public class Query : IRequest<Result<UserDto>>
		{
			public int UserId { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<UserDto>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Handler> _logger;
			private readonly IMapper _mapper;

			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<UserDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					UserDto userProfile = await _context.User
						.Where(u => u.UserId == request.UserId)
						.Where(u => u.IsDeleted != true)
						.ProjectTo<UserDto>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					if (userProfile == null)
					{
						_logger.LogInformation("User doesn't exist");
						return Result<UserDto>.NotFound("User doesn't exist.");
					}

					// Set to null to make unnecessary fields excluded from the response body.
					userProfile.Role = null;
					userProfile.UserStatus = null;
					userProfile.LastLoginDevice = null;
					userProfile.LastTimeLogin = null;
					userProfile.IsBikeVerified = null;
					userProfile.CreatedDate = null;
					userProfile.IsDeleted = null;

					_logger.LogInformation("Successfully retrieved user self profile");
					return Result<UserDto>.Success(
						userProfile, "Successfully retrieved user self profile.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<UserDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using MediatR;
using Persistence;

namespace Application.Users
{
	public class DetailSelfUser
	{
		public class Query : IRequest<Result<UserDTO>>
		{
			public int UserId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<UserDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailSelfUser> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailSelfUser> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<UserDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var userProfile = await _context.User
						.Where(u => u.UserId == request.UserId)
						// .Where(u => u.Status == (int)UserStatus.Active)
						.Where(u => u.IsDeleted != true)
						.ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);
					// Set to null to make unnecessary fields excluded from response body.
					userProfile.Role = null;
					userProfile.UserStatus = null;
					userProfile.LastLoginDevice = null;
					userProfile.LastTimeLogin = null;
					userProfile.IsBikeVerified = null;
					userProfile.CreatedDate = null;
					userProfile.IsDeleted = null;

					_logger.LogInformation("Successfully retrieved user self profile.");
					return Result<UserDTO>.Success(
						userProfile, "Successfully retrieved user self profile.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<UserDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
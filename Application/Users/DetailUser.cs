using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Persistence;

namespace Application.Users
{
	public class DetailUser
	{
		public class Query : IRequest<Result<UserDto>>
		{
			public int UserId { get; set; }
			public bool IsAdmin { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<UserDto>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailUser> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailUser> logger)
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

					UserDto userProfile = new UserDto();

					if (request.IsAdmin)
					{
						User userProfileDb = await _context.User
							.FindAsync(new object[] { request.UserId }, cancellationToken);

						_mapper.Map(userProfileDb, userProfile);
					}
					else
					{
						userProfile = await _context.User
							.Where(u => u.UserId == request.UserId)
							// .Where(u => u.Status == (int)UserStatus.Active)
							.Where(u => u.IsDeleted != true)
							.ProjectTo<UserDto>(_mapper.ConfigurationProvider)
							.SingleOrDefaultAsync(cancellationToken);
						// Set to null to make unnecessary fields excluded from response body.
						userProfile.Role = null;
						userProfile.TotalPoint = null;
						userProfile.UserStatus = null;
						userProfile.LastLoginDevice = null;
						userProfile.LastTimeLogin = null;
						userProfile.IsBikeVerified = null;
						userProfile.CreatedDate = null;
						userProfile.IsDeleted = null;
					}

					_logger.LogInformation($"Successfully retrieved user profile by UserId {request.UserId}.");
					return Result<UserDto>.Success(
						userProfile, $"Successfully retrieved user profile by UserId {request.UserId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<UserDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
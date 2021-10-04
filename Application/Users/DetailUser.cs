using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Persistence;
using Application.Core;
using Application.Users.DTOs;
using Domain.Enums;
using Domain.Entities;

namespace Application.Users
{
	public class DetailUser
	{
		public class Query : IRequest<Result<UserDTO>>
		{
			public int UserId { get; set; }
			public bool IsAdmin { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<UserDTO>>
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

			public async Task<Result<UserDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					UserDTO userProfile = new UserDTO();

					if (request.IsAdmin)
					{
						User userProfileDB = await _context.User
							.FindAsync(new object[] { request.UserId }, cancellationToken);

						_mapper.Map(userProfileDB, userProfile);
					}
					else
					{
						userProfile = await _context.User
							.Where(u => u.UserId == request.UserId)
							.Where(u => u.Status == (int)UserStatus.Active)
							.Where(u => u.IsDeleted != true)
							.ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
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
					return Result<UserDTO>.Success(
						userProfile, $"Successfully retrieved user profile by UserId {request.UserId}.");
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
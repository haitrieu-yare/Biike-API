using Application.Core;
using Application.Users.DTOs;
using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Persistence;
using AutoMapper;
using Microsoft.Extensions.Logging;
using System.Linq;
using Domain.Enums;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Application.Users
{
	public class DetailSelf
	{
		public class Query : IRequest<Result<UserSelfProfileDTO>>
		{
			public int Id { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<UserSelfProfileDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailSelf> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailSelf> logger)
			{
				_mapper = mapper;
				_context = context;
				_logger = logger;
			}

			public async Task<Result<UserSelfProfileDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var AppUserProfile = await _context.User
						.Where(u => u.Id == request.Id)
						.Where(u => u.Status == (int)UserStatus.Active)
						.ProjectTo<UserSelfProfileDTO>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved user self profile");
					return Result<UserSelfProfileDTO>.Success(AppUserProfile);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<UserSelfProfileDTO>.Failure("Request was cancelled");
				}
			}
		}
	}
}
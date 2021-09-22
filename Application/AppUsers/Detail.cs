using Application.Core;
using Application.AppUsers.DTOs;
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

namespace Application.AppUsers
{
	public class Detail
	{
		public class Query : IRequest<Result<AppUserProfileDTO>>
		{
			public int Id { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<AppUserProfileDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Detail> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Detail> logger)
			{
				_mapper = mapper;
				_context = context;
				_logger = logger;
			}

			public async Task<Result<AppUserProfileDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var AppUserProfile = await _context.AppUser
						.Where(u => u.Id == request.Id)
						.Where(u => u.Status == (int)AppUserStatus.Active)
						.ProjectTo<AppUserProfileDTO>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved user profile");
					return Result<AppUserProfileDTO>.Success(AppUserProfile);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<AppUserProfileDTO>.Failure("Request was cancelled");
				}
			}
		}
	}
}
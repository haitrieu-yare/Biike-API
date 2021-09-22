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
	public class DetailSelf
	{
		public class Query : IRequest<Result<AppUserSelfProfileDTO>>
		{
			public int Id { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<AppUserSelfProfileDTO>>
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

			public async Task<Result<AppUserSelfProfileDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var AppUserProfile = await _context.AppUser
						.Where(u => u.Id == request.Id)
						.Where(u => u.Status == (int)AppUserStatus.Active)
						.ProjectTo<AppUserSelfProfileDTO>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved user self profile");
					return Result<AppUserSelfProfileDTO>.Success(AppUserProfile);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<AppUserSelfProfileDTO>.Failure("Request was cancelled");
				}
			}
		}
	}
}
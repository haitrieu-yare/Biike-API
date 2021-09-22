using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.AppUsers.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.AppUsers
{
	public class List
	{
		public class Query : IRequest<Result<List<AppUserProfileDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<AppUserProfileDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<List> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<List> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<AppUserProfileDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var users = await _context.AppUser
						.ProjectTo<AppUserProfileDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all user");
					return Result<List<AppUserProfileDTO>>.Success(users);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<AppUserProfileDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}
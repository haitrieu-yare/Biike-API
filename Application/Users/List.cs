using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Users.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Users
{
	public class List
	{
		public class Query : IRequest<Result<List<UserInfoDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<UserInfoDTO>>>
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

			public async Task<Result<List<UserInfoDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var users = await _context.User
						.ProjectTo<UserInfoDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all user");
					return Result<List<UserInfoDTO>>
						.Success(users, "Successfully retrieved list of all user");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<UserInfoDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}
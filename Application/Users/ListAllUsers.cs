using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Users.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Users
{
	public class ListAllUsers
	{
		public class Query : IRequest<Result<List<UserDTO>>> { }
		public class Handler : IRequestHandler<Query, Result<List<UserDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListAllUsers> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListAllUsers> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<UserDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var users = await _context.User
						.ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all users.");
					return Result<List<UserDTO>>.Success(
						users, "Successfully retrieved list of all users.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<List<UserDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
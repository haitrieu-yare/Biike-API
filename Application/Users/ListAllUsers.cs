using System.Collections.Generic;
using System.Linq;
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
		public class Query : IRequest<Result<List<UserDTO>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}
		public class Handler : IRequestHandler<Query, Result<List<UserDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Handler> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
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

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<UserDTO>>.Failure("Page must larger than 0.");
					}
					else if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must larger than 0");
						return Result<List<UserDTO>>.Failure("Limit must larger than 0.");
					}

					int totalRecord = await _context.User.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<UserDTO> users = new List<UserDTO>();

					if (request.Page <= lastPage)
					{
						users = await _context.User
							.OrderBy(u => u.UserId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<UserDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, users.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all users");
					return Result<List<UserDTO>>.Success(
						users, "Successfully retrieved list of all users.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<UserDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
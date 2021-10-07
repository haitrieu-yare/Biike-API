using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Intimacies.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Intimacies
{
	public class ListIntimaciesByUserId
	{
		public class Query : IRequest<Result<List<IntimacyDTO>>>
		{
			public int UserOneId { get; set; }
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<IntimacyDTO>>>
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

			public async Task<Result<List<IntimacyDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<IntimacyDTO>>.Failure("Page must larger than 0.");
					}

					int totalRecord = await _context.Intimacy
						.Where(i => i.UserOneId == request.UserOneId)
						.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<IntimacyDTO> intimacies = new List<IntimacyDTO>();

					if (request.Page <= lastPage)
					{
						intimacies = await _context.Intimacy
							.Where(i => i.UserOneId == request.UserOneId)
							.ProjectTo<IntimacyDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, intimacies.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of user intimacies " +
						$"by UserId {request.UserOneId}");
					return Result<List<IntimacyDTO>>.Success(intimacies, "Successfully retrieved list of " +
						$"user intimacies by UserId {request.UserOneId}.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<IntimacyDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
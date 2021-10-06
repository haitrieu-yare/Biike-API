using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Redemptions.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;
using System.Linq;

namespace Application.Redemptions
{
	public class ListRedemption
	{
		public class Query : IRequest<Result<List<RedemptionDTO>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<RedemptionDTO>>>
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

			public async Task<Result<List<RedemptionDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<RedemptionDTO>>.Failure("Page must larger than 0.");
					}

					int totalRecord = await _context.Redemption.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<RedemptionDTO> redemptions = new List<RedemptionDTO>();

					if (request.Page <= lastPage)
					{
						redemptions = await _context.Redemption
							.OrderBy(r => r.RedemptionId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<RedemptionDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, redemptions.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all redemptions");
					return Result<List<RedemptionDTO>>.Success(
						redemptions, "Successfully retrieved list of all redemptions.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<RedemptionDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
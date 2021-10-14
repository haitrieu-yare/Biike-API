using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Redemptions.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Redemptions
{
	public class ListRedemption
	{
		public class Query : IRequest<Result<List<RedemptionDto>>>
		{
			public int Page { get; init; }
			public int Limit { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<List<RedemptionDto>>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Handler> _logger;
			private readonly IMapper _mapper;

			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<RedemptionDto>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must be larger than 0");
						return Result<List<RedemptionDto>>.Failure("Page must be larger than 0.");
					}

					if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must be larger than 0");
						return Result<List<RedemptionDto>>.Failure("Limit must be larger than 0.");
					}

					int totalRecord = await _context.Redemption.CountAsync(cancellationToken);

					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);

					List<RedemptionDto> redemptions = new();

					if (request.Page <= lastPage)
						redemptions = await _context.Redemption.OrderBy(r => r.RedemptionId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<RedemptionDto>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

					PaginationDto paginationDto = new(
						request.Page, request.Limit, redemptions.Count, lastPage, totalRecord);

					_logger.LogInformation("Successfully retrieved list of all redemptions");
					return Result<List<RedemptionDto>>.Success(redemptions,
						"Successfully retrieved list of all redemptions.", paginationDto);
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<RedemptionDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
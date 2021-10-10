using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Bikes
{
	public class ListBikes
	{
		public class Query : IRequest<Result<List<BikeDto>>>
		{
			public int Page { get; init; }
			public int Limit { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<List<BikeDto>>>
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

			public async Task<Result<List<BikeDto>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must be larger than 0");
						return Result<List<BikeDto>>.Failure("Page must be larger than 0.");
					}

					int totalRecord = await _context.Bike.CountAsync(cancellationToken);

					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);

					List<BikeDto> bikes = new();

					if (request.Page <= lastPage)
						bikes = await _context.Bike
							.OrderBy(b => b.BikeId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<BikeDto>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

					PaginationDto paginationDto = new(
						request.Page, request.Limit, bikes.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all bikes");
					return Result<List<BikeDto>>.Success(
						bikes, "Successfully retrieved list of all bikes.", paginationDto);
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<BikeDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
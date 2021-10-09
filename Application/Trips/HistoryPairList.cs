using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using MediatR;
using Persistence;

namespace Application.Trips
{
	public class HistoryPairList
	{
		public class Query : IRequest<Result<List<TripPairDto>>>
		{
			public int UserOneId { get; set; }
			public int UserTwoId { get; set; }
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripPairDto>>>
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

			public async Task<Result<List<TripPairDto>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<TripPairDto>>.Failure("Page must larger than 0.");
					}

					int totalRecord = await _context.Trip
						.Where(t => (t.KeerId == request.UserOneId && t.BikerId == request.UserTwoId)
							|| (t.KeerId == request.UserTwoId && t.BikerId == request.UserOneId))
						.Where(t => t.Status == (int)TripStatus.Finished
							|| t.Status == (int)TripStatus.Cancelled)
						.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<TripPairDto> trips = new List<TripPairDto>();

					if (request.Page <= lastPage)
					{
						trips = await _context.Trip
							.Where(t => (t.KeerId == request.UserOneId && t.BikerId == request.UserTwoId)
							|| (t.KeerId == request.UserTwoId && t.BikerId == request.UserOneId))
							.Where(t => t.Status == (int)TripStatus.Finished
								|| t.Status == (int)TripStatus.Cancelled)
							.OrderByDescending(t => t.BookTime)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<TripPairDto>(_mapper.ConfigurationProvider,
								new { userTwoId = request.UserTwoId })
							.ToListAsync(cancellationToken);
					}

					PaginationDto paginationDto = new PaginationDto(
						request.Page, request.Limit, trips.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all history pair trip");
					return Result<List<TripPairDto>>.Success(
						trips, "Successfully retrieved list of all history pair trip.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripPairDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
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
using MediatR;
using Persistence;

namespace Application.Trips
{
	public class ListTrips
	{
		public class Query : IRequest<Result<List<TripDetailDTO>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripDetailDTO>>>
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

			public async Task<Result<List<TripDetailDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<TripDetailDTO>>.Failure("Page must larger than 0.");
					}

					int totalRecord = await _context.Trip.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<TripDetailDTO> trips = new List<TripDetailDTO>();

					if (request.Page <= lastPage)
					{
						trips = await _context.Trip
							.OrderBy(t => t.TripId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<TripDetailDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, trips.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all trips");
					return Result<List<TripDetailDTO>>.Success(
						trips, "Successfully retrieved list of all trips.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripDetailDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
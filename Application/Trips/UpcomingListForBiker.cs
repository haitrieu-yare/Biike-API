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
	public class UpcomingListForBiker
	{
		public class Query : IRequest<Result<List<TripDTO>>>
		{
			public int UserId { get; set; }
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripDTO>>>
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

			public async Task<Result<List<TripDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<TripDTO>>.Failure("Page must larger than 0.");
					}

					int totalRecord = await _context.Trip
						.Where(t => t.KeerId != request.UserId)
						.Where(t => t.Status == (int)TripStatus.Finding)
						.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<TripDTO> trips = new List<TripDTO>();

					if (request.Page <= lastPage)
					{
						trips = await _context.Trip
							.Where(t => t.KeerId != request.UserId)
							.Where(t => t.Status == (int)TripStatus.Finding)
							.OrderBy(t => t.BookTime)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<TripDTO>(_mapper.ConfigurationProvider,
								new { isKeer = false })
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, trips.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all upcoming trips for biker");
					return Result<List<TripDTO>>.Success(
						trips, "Successfully retrieved list of all upcoming trips for biker.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
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
	public class UpcomingList
	{
		public class Query : IRequest<Result<List<TripDTO>>>
		{
			public int UserId { get; set; }
			public int Role { get; set; }
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

					if (request.Role != (int)RoleStatus.Keer && request.Role != (int)RoleStatus.Biker)
					{
						_logger.LogInformation("Role must be " + (int)RoleStatus.Keer
							+ " (Keer) or " + (int)RoleStatus.Biker + " (Biker)");
						return Result<List<TripDTO>>.Failure("Role must be " + (int)RoleStatus.Keer
							+ " (Keer) or " + (int)RoleStatus.Biker + " (Biker).");
					}

					int totalRecord = await _context.Trip
						.Where(t => (request.Role == (int)RoleStatus.Keer) ?
							t.KeerId == request.UserId :
							t.BikerId == request.UserId)
						.Where(t => t.Status == (int)TripStatus.Finding
							|| t.Status == (int)TripStatus.Waiting)
						.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<TripDTO> trips = new List<TripDTO>();

					if (request.Page <= lastPage)
					{
						trips = await _context.Trip
							.Where(t => (request.Role == (int)RoleStatus.Keer) ?
								t.KeerId == request.UserId :
								t.BikerId == request.UserId)
							.Where(t => t.Status == (int)TripStatus.Finding
								|| t.Status == (int)TripStatus.Waiting)
							.OrderBy(t => t.BookTime)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<TripDTO>(_mapper.ConfigurationProvider,
								new { role = request.Role })
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, trips.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all upcoming trip");
					return Result<List<TripDTO>>.Success(
						trips, "Successfully retrieved list of all upcoming trip.", paginationDto);
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
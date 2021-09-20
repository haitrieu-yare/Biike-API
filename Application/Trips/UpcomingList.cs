using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
	public class UpcomingList
	{
		public class Query : IRequest<Result<List<TripUpcomingDTO>>>
		{
			public int UserId { get; set; }
			public int Role { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripUpcomingDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<UpcomingList> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<UpcomingList> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<TripUpcomingDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var tripUpcomingDTO = new List<TripUpcomingDTO>();

					if (request.Role == (int)RoleStatus.Keer)
					{
						var tripUpcomingKeerDTO = await _context.Trip
							.Where(t => t.KeerId == request.UserId)
							.Where(t => t.Status == (int)TripStatus.Finding
								|| t.Status == (int)TripStatus.Waiting)
							.ProjectTo<KeerUpcomingTripDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

						_mapper.Map(tripUpcomingKeerDTO, tripUpcomingDTO);
					}
					else if (request.Role == (int)RoleStatus.Biker)
					{
						var tripUpcomingBikerDTO = await _context.Trip
							.Where(t => t.BikerId == request.UserId)
							.Where(t => t.Status == (int)TripStatus.Waiting)
							.ProjectTo<BikerUpcomingTripDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

						_mapper.Map(tripUpcomingBikerDTO, tripUpcomingDTO);
					}

					_logger.LogInformation("Successfully retrieved list of all upcoming trip");
					return Result<List<TripUpcomingDTO>>.Success(tripUpcomingDTO);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripUpcomingDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}
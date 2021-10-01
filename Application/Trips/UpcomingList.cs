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
		public class Query : IRequest<Result<List<TripDTO>>>
		{
			public int UserId { get; set; }
			public int Role { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripDTO>>>
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

			public async Task<Result<List<TripDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Role != (int)RoleStatus.Keer && request.Role != (int)RoleStatus.Biker)
					{
						_logger.LogInformation("Role must be " + (int)RoleStatus.Keer
							+ " (Keer) or " + (int)RoleStatus.Biker + " (Biker)");
						return Result<List<TripDTO>>.Failure("Role must be " + (int)RoleStatus.Keer
							+ " (Keer) or " + (int)RoleStatus.Biker + " (Biker)");
					}

					var tripDTO = await _context.Trip
						.Where(t => (request.Role == (int)RoleStatus.Keer) ?
							t.KeerId == request.UserId :
							t.BikerId == request.UserId)
						.Where(t => t.Status == (int)TripStatus.Finding
							|| t.Status == (int)TripStatus.Waiting)
						.ProjectTo<TripDTO>(_mapper.ConfigurationProvider,
							new { role = request.Role })
						.OrderBy(t => t.TimeBook)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all upcoming trip");
					return Result<List<TripDTO>>
						.Success(tripDTO, "Successfully retrieved list of all upcoming trip");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}
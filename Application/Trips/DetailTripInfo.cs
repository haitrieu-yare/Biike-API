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
using Domain.Entities;

namespace Application.Trips
{
	public class DetailTripInfo
	{
		public class Query : IRequest<Result<TripDetailInfoDto>>
		{
			public int TripId { get; set; }
			public int Role { get; set; }
			public int UserRequestId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<TripDetailInfoDto>>
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

			public async Task<Result<TripDetailInfoDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var tripDb = await _context.Trip
						.FindAsync(new object[] { request.TripId }, cancellationToken);

					if (tripDb == null) return Result<TripDetailInfoDto>.NotFound("Trip doens't exist.");

					bool isRequestUserInTrip = true;

					if (tripDb.BikerId == null && request.UserRequestId != tripDb.KeerId)
					{
						isRequestUserInTrip = false;
					}
					else if (request.UserRequestId != tripDb.KeerId && request.UserRequestId != tripDb.BikerId)
					{
						isRequestUserInTrip = false;
					}

					if (!isRequestUserInTrip)
					{
						_logger.LogInformation($"User with UserId {request.UserRequestId} " +
							$"request an unauthorized content of trip with TripId {request.TripId}");
						return Result<TripDetailInfoDto>.Unauthorized();
					}

					var trip = await _context.Trip
						.Where(t => t.TripId == request.TripId)
						.ProjectTo<TripDetailInfoDto>(_mapper.ConfigurationProvider,
							new { role = request.Role })
						.SingleOrDefaultAsync(cancellationToken);

					// Set to null to make unnecessary fields excluded from response body.
					trip.Feedbacks.ForEach(feedback =>
					{
						feedback.TripId = null;
						feedback.CreatedDate = null;
					});

					_logger.LogInformation($"Successfully retrieved trip by TripId {request.TripId}");
					return Result<TripDetailInfoDto>.Success(
						trip, $"Successfully retrieved trip by TripId {request.TripId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<TripDetailInfoDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
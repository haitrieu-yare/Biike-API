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
		public class Query : IRequest<Result<TripDetailInfoDTO>>
		{
			public int TripId { get; set; }
			public int Role { get; set; }
			public int UserRequestId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<TripDetailInfoDTO>>
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

			public async Task<Result<TripDetailInfoDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var tripDB = await _context.Trip
						.FindAsync(new object[] { request.TripId }, cancellationToken);

					if (request.UserRequestId != tripDB.KeerId && request.UserRequestId != tripDB.BikerId)
					{
						_logger.LogInformation($"User with UserId {request.UserRequestId} " +
							$"request an unauthorized content of trip with TripId {request.TripId}");
						return Result<TripDetailInfoDTO>.Unauthorized();
					}

					var trip = await _context.Trip
						.Where(t => t.TripId == request.TripId)
						.ProjectTo<TripDetailInfoDTO>(_mapper.ConfigurationProvider,
							new { role = request.Role })
						.SingleOrDefaultAsync(cancellationToken);

					// Set to null to make unnecessary fields excluded from response body.
					trip.Feedbacks.ForEach(feedback =>
					{
						feedback.TripId = null;
						feedback.CreatedDate = null;
					});

					_logger.LogInformation($"Successfully retrieved trip by TripId {request.TripId}");
					return Result<TripDetailInfoDTO>.Success(
						trip, $"Successfully retrieved trip by TripId {request.TripId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<TripDetailInfoDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
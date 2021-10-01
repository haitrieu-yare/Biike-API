using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
	public class EditTripCancellation
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int TripId { get; set; }
			public int UserId { get; set; }
			public TripCancellationDTO TripCancellationDTO { get; set; } = null!;
		}
		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<EditTripCancellation> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<EditTripCancellation> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var oldTrip = await _context.Trip
						.FindAsync(new object[] { request.TripId }, cancellationToken);
					if (oldTrip == null) return null!;

					switch (oldTrip.Status)
					{
						case (int)TripStatus.Finished:
							_logger.LogInformation("Trip has already finished");
							return Result<Unit>.Failure("Trip has already finished");
						case (int)TripStatus.Cancelled:
							_logger.LogInformation("Trip has already cancelled");
							return Result<Unit>.Failure("Trip has already cancelled");
					}

					if (request.UserId != oldTrip.KeerId && request.UserId != oldTrip.BikerId)
					{
						_logger.LogInformation("Cancellation request must come from Keer or Biker of the trip");
						return Result<Unit>.Failure("Cancellation request must come from Keer or Biker of the trip");
					}

					_mapper.Map(request.TripCancellationDTO, oldTrip);
					oldTrip.CancelPersonId = request.UserId;
					oldTrip.Status = (int)TripStatus.Cancelled;

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update trip");
						return Result<Unit>.Failure("Failed to update trip");
					}
					else
					{
						_logger.LogInformation("Successfully updated trip");
						return Result<Unit>.Success(Unit.Value, "Successfully updated trip");
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.Message);
					return Result<Unit>.Failure(ex.Message);
				}
			}
		}
	}
}
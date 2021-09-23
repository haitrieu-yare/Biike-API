using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips.DTOs
{
	public class EditTripProcess
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int Id { get; set; }
			public DateTime? Time { get; set; }
		}
		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<EditTripProcess> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<EditTripProcess> logger)
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

					if (request.Time == null)
					{
						_logger.LogInformation("No parameters are received");
						return Result<Unit>.Failure("No parameters are received");
					}

					var oldTrip = await _context.Trip
						.FindAsync(new object[] { request.Id }, cancellationToken);
					if (oldTrip == null) return null;

					if (oldTrip.BikerId == null)
					{
						_logger.LogInformation("Trip must has Biker before starting");
						return Result<Unit>.Failure("Trip must has Biker before starting");
					}
					switch (oldTrip.Status)
					{
						case 1:
							oldTrip.PickupTime = request.Time;
							oldTrip.Status = 2;
							break;
						case 2:
							oldTrip.FinishedTime = request.Time;
							oldTrip.Status = 3;
							break;
						case 3:
							_logger.LogInformation("Trip has already finished");
							return Result<Unit>.Failure("Trip has already finished");
						case 4:
							_logger.LogInformation("Trip has already cancelled");
							return Result<Unit>.Failure("Trip has already cancelled");
					}

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update trip");
						return Result<Unit>.Failure("Failed to update trip");
					}
					else
					{
						_logger.LogInformation("Successfully updated trip");
						return Result<Unit>.Success(Unit.Value);
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
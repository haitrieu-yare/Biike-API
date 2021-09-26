using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
	public class EditTripBiker
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int Id { get; set; }
			public int BikerId { get; set; }
		}
		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<EditTripBiker> _logger;
			public Handler(DataContext context, ILogger<EditTripBiker> logger)
			{
				_logger = logger;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var oldTrip = await _context.Trip
						.FindAsync(new object[] { request.Id }, cancellationToken);
					if (oldTrip == null) return null!;

					if (request.BikerId == oldTrip.KeerId)
					{
						_logger.LogInformation("Biker and Keer can't be the same person");
						return Result<Unit>.Failure("Biker and Keer can't be the same person");
					}

					var biker = await _context.User
						.FindAsync(new object[] { request.BikerId }, cancellationToken);

					if (!biker.IsBikeVerified)
					{
						_logger.LogInformation("Biker doesn't have verified bike yet");
						return Result<Unit>.Failure("Biker doesn't have verified bike yet");
					}

					var bike = await _context.Bike
						.Where(b => b.UserId == biker.Id)
						.FirstOrDefaultAsync(cancellationToken);

					oldTrip.BikerId = biker.Id;
					oldTrip.PlateNumber = bike.PlateNumber;
					oldTrip.Status = 1;

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
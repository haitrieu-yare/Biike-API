using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using MediatR;
using Persistence;
using Domain.Entities;

namespace Application.Bikes
{
	public class ReplaceBike
	{
		public class Command : IRequest<Result<Unit>>
		{
			public BikeCreateDto BikeCreateDto { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
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

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var oldBike = await _context.Bike
						.Where(b => b.UserId == request.BikeCreateDto.UserId)
						.SingleOrDefaultAsync(cancellationToken);

					if (oldBike == null)
					{
						_logger.LogInformation("User doesn't have a bike");
						return Result<Unit>.Failure("User doesn't have a bike.");
					}

					_context.Bike.Remove(oldBike);

					Bike newBike = new Bike();

					_mapper.Map(request.BikeCreateDto, newBike);

					await _context.Bike.AddAsync(newBike, cancellationToken);

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to replace old bike with new bike");
						return Result<Unit>.Failure("Failed to replace old bike with new bike.");
					}
					else
					{
						_logger.LogInformation("Successfully replace old bike with new bike");
						return Result<Unit>.Success(
							Unit.Value, "Successfully replace old bike with new bike.", newBike.BikeId.ToString());
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}
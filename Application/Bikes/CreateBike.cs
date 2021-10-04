using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.Bikes
{
	public class CreateBike
	{
		public class Command : IRequest<Result<Unit>>
		{
			public BikeCreateDTO BikeCreateDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<CreateBike> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<CreateBike> logger)
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
						.Where(b => b.UserId == request.BikeCreateDTO.UserId)
						.SingleOrDefaultAsync(cancellationToken);

					if (oldBike != null)
					{
						_logger.LogInformation("Biker already has bike.");
						return Result<Unit>.Failure("Biker already has bike.");
					}

					Bike newBike = new Bike();

					_mapper.Map(request.BikeCreateDTO, newBike);

					await _context.Bike.AddAsync(newBike, cancellationToken);

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new bike.");
						return Result<Unit>.Failure("Failed to create new bike.");
					}
					else
					{
						_logger.LogInformation("Successfully created new bike.");
						return Result<Unit>.Success(Unit.Value, "Successfully created new bike.", newBike.BikeId.ToString());
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
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
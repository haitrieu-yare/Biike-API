using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.Trips
{
	public class CreateTrip
	{
		public class Command : IRequest<Result<Unit>>
		{
			public TripCreateDTO TripCreateDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<CreateTrip> _logger;
			private readonly IMapper _mapper;
			public Handler(DataContext context, IMapper mapper, ILogger<CreateTrip> logger)
			{
				_mapper = mapper;
				_logger = logger;
				_context = context;
			}
			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var newTrip = new Trip();
					_mapper.Map(request.TripCreateDTO, newTrip);

					await _context.Trip.AddAsync(newTrip, cancellationToken);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new trip");
						return Result<Unit>.Failure("Failed to create new trip");
					}
					else
					{
						_logger.LogInformation("Successfully created trip");
						return Result<Unit>.Success(Unit.Value, "Successfully created trip");
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
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Bikes
{
	public class Create
	{
		public class Command : IRequest<Result<Unit>>
		{
			public BikeDTO BikeDTO { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Create> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Create> logger)
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

					var oldBike = await _context.Bike
						.Where(b => b.AppUserId == request.BikeDTO.UserId)
						.SingleOrDefaultAsync(cancellationToken);
					if (oldBike != null)
					{
						_logger.LogInformation("Biker already has bike");
						return Result<Unit>.Failure("Biker already has bike");
					}

					var newBike = new Bike();
					_mapper.Map(request.BikeDTO, newBike);

					await _context.Bike.AddAsync(newBike, cancellationToken);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new bike");
						return Result<Unit>.Failure("Failed to create new bike");
					}
					else
					{
						_logger.LogInformation("Successfully created Bike");
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
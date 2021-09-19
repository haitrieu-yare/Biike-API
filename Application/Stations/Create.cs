using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Stations
{
	public class Create
	{
		public class Command : IRequest<Result<Unit>>
		{
			public StationDTO StationDto { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Create> _logger;
			private readonly IMapper _mapper;
			public Handler(DataContext context, IMapper mapper, ILogger<Create> logger)
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

					#region Check related data
					var area = await _context.Area.FindAsync(request.StationDto.AreaId);

					if (area == null)
					{
						_logger.LogInformation("Failed to create new station");
						return Result<Unit>.Failure("Failed to create new station");
					}
					#endregion

					var newStation = new Station();
					_mapper.Map(request.StationDto, newStation);

					await _context.Station.AddAsync(newStation, cancellationToken);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new station");
						return Result<Unit>.Failure("Failed to create new station");
					}
					else
					{
						_logger.LogInformation("Successfully created station");
						return Result<Unit>.Success(Unit.Value);
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled");
				}
			}
		}
	}
}
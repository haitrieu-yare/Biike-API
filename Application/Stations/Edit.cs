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
	public class Edit
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int Id { get; set; }
			public StationDTO newStationDto { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Edit> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Edit> logger)
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

					#region Check related data
					var area = await _context.Area.FindAsync(request.newStationDto.AreaId);

					if (area == null)
					{
						_logger.LogInformation("Failed to update station");
						return Result<Unit>.Failure("Failed to update station");
					}
					#endregion

					var oldStation = await _context.Station.FindAsync(request.Id);
					if (oldStation == null) return null;

					_mapper.Map(request.newStationDto, oldStation);
					var result = await _context.SaveChangesAsync() > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update station");
						return Result<Unit>.Failure("Failed to update station");
					}
					else
					{
						_logger.LogInformation("Successfully updated station");
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
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
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
			public Station Station { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Create> _logger;
			public Handler(DataContext context, ILogger<Create> logger)
			{
				_logger = logger;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					#region Check related data
					var area = await _context.Area.FindAsync(request.Station.AreaId);

					if (area == null)
					{
						_logger.LogInformation("Failed to create new station");
						return Result<Unit>.Failure("Failed to create new station");
					}
					#endregion

					await _context.Station.AddAsync(request.Station, cancellationToken);
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
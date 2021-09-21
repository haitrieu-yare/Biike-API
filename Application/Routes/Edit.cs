using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Routes
{
	public class Edit
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int Id { get; set; }
			public RouteDTO NewRouteDTO { get; set; }
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

					var oldRoute = await _context.Route
						.FindAsync(new object[] { request.Id }, cancellationToken);
					if (oldRoute == null) return null;

					_mapper.Map(request.NewRouteDTO, oldRoute);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update route");
						return Result<Unit>.Failure("Failed to update route");
					}
					else
					{
						_logger.LogInformation("Successfully updated route");
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
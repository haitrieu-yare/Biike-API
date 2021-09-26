using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Bikes
{
	public class Detail
	{
		public class Query : IRequest<Result<BikeDTO>>
		{
			public int UserId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<BikeDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Detail> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Detail> logger)
			{
				_mapper = mapper;
				_context = context;
				_logger = logger;
			}

			public async Task<Result<BikeDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var bike = await _context.Bike
						.Where(b => b.UserId == request.UserId)
						.ProjectTo<BikeDTO>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved bike");
					return Result<BikeDTO>.Success(bike);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<BikeDTO>.Failure("Request was cancelled");
				}
			}
		}
	}
}
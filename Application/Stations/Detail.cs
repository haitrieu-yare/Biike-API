using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Stations
{
	public class Detail
	{
		public class Query : IRequest<Result<StationDTO>>
		{
			public int Id { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<StationDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Detail> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Detail> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<StationDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var station = await _context.Station
					.Where(s => s.IsDeleted != true)
					.ProjectTo<StationDTO>(_mapper.ConfigurationProvider)
					.FirstOrDefaultAsync(x => x.Id == request.Id);

					_logger.LogInformation("Successfully retrieved station");
					return Result<StationDTO>.Success(station);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<StationDTO>.Failure("Request was cancelled");
				}
			}
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Bikes.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Bikes
{
	public class ListBikes
	{
		public class Query : IRequest<Result<List<BikeDTO>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<BikeDTO>>>
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

			public async Task<Result<List<BikeDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<BikeDTO>>.Failure("Page must larger than 0.");
					}

					var totalRecord = await _context.Bike.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<BikeDTO> bikes = new List<BikeDTO>();

					if (request.Page <= lastPage)
					{
						bikes = await _context.Bike
							.OrderBy(b => b.BikeId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<BikeDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDTO = new PaginationDTO(
						request.Page, request.Limit, bikes.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all bikes");
					return Result<List<BikeDTO>>.Success(
						bikes, "Successfully retrieved list of all bikes.", paginationDTO);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<BikeDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
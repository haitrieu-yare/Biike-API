using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.TripTransactions.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.TripTransactions
{
	public class ListTripTransactions
	{
		public class Query : IRequest<Result<List<TripTransactionDTO>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripTransactionDTO>>>
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

			public async Task<Result<List<TripTransactionDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<TripTransactionDTO>>.Failure("Page must larger than 0.");
					}
					else if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must larger than 0");
						return Result<List<TripTransactionDTO>>.Failure("Limit must larger than 0.");
					}

					int totalRecord = await _context.TripTransaction.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<TripTransactionDTO> tripTransactions = new List<TripTransactionDTO>();

					if (request.Page <= lastPage)
					{
						tripTransactions = await _context.TripTransaction
						.OrderBy(t => t.TripTransactionId)
						.Skip((request.Page - 1) * request.Limit)
						.Take(request.Limit)
						.ProjectTo<TripTransactionDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, tripTransactions.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all trip transaction");
					return Result<List<TripTransactionDTO>>.Success(
						tripTransactions, "Successfully retrieved list of all trip transaction.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripTransactionDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
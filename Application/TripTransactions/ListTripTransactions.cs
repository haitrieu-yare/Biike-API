using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.TripTransactions.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.TripTransactions
{
	public class ListTripTransactions
	{
		public class Query : IRequest<Result<List<TripTransactionDto>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripTransactionDto>>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Handler> _logger;
			private readonly IMapper _mapper;

			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<TripTransactionDto>>> Handle(Query request,
				CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<TripTransactionDto>>.Failure("Page must larger than 0.");
					}

					if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must larger than 0");
						return Result<List<TripTransactionDto>>.Failure("Limit must larger than 0.");
					}

					int totalRecord = await _context.TripTransaction.CountAsync(cancellationToken);

					#region Calculate last page

					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);

					#endregion

					List<TripTransactionDto> tripTransactions = new();

					if (request.Page <= lastPage)
						tripTransactions = await _context.TripTransaction
							.OrderBy(t => t.TripTransactionId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<TripTransactionDto>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

					PaginationDto paginationDto = new(
						request.Page, request.Limit, tripTransactions.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all trip transaction");
					return Result<List<TripTransactionDto>>.Success(
						tripTransactions, "Successfully retrieved list of all trip transaction.", paginationDto);
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripTransactionDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
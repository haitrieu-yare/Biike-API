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
	public class DetailTripTransactionByTrip
	{
		public class Query : IRequest<Result<List<TripTransactionDto>>>
		{
			public int TripId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripTransactionDto>>>
		{
			private readonly DataContext _context;
			private readonly ILogger<DetailTripTransactionByTrip> _logger;
			private readonly IMapper _mapper;

			public Handler(DataContext context, IMapper mapper, ILogger<DetailTripTransactionByTrip> logger)
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

					var tripTransaction = await _context.TripTransaction
						.Where(t => t.TripId == request.TripId)
						.ProjectTo<TripTransactionDto>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved trip transaction " +
					                       $"based on tripId {request.TripId}.");
					return Result<List<TripTransactionDto>>.Success(
						tripTransaction, "Successfully retrieved trip transaction " +
						                 $"based on tripId {request.TripId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<List<TripTransactionDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
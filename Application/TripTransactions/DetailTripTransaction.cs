using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.TripTransactions.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.TripTransactions
{
	public class DetailTripTransaction
	{
		public class Query : IRequest<Result<TripTransactionDto>>
		{
			public int TripTransactionId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<TripTransactionDto>>
		{
			private readonly DataContext _context;
			private readonly ILogger<DetailTripTransaction> _logger;
			private readonly IMapper _mapper;

			public Handler(DataContext context, IMapper mapper, ILogger<DetailTripTransaction> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<TripTransactionDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var tripTransactionDb = await _context.TripTransaction
						.FindAsync(new object[] { request.TripTransactionId }, cancellationToken);

					TripTransactionDto tripTransaction = new();

					_mapper.Map(tripTransactionDb, tripTransaction);

					_logger.LogInformation("Successfully retrieved trip transaction " +
					                       $"based on transactionId {request.TripTransactionId}.");
					return Result<TripTransactionDto>.Success(
						tripTransaction, "Successfully retrieved trip transaction " +
						                 $"based on transactionId {request.TripTransactionId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<TripTransactionDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
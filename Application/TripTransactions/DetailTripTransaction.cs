using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.TripTransactions.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.TripTransactions
{
	public class DetailTripTransaction
	{
		public class Query : IRequest<Result<TripTransactionDto>>
		{
			public int TripTransactionId { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<TripTransactionDto>>
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

			public async Task<Result<TripTransactionDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					TripTransaction tripTransactionDb =
						await _context.TripTransaction.FindAsync(new object[] { request.TripTransactionId },
							cancellationToken);

					if (tripTransactionDb == null)
					{
						_logger.LogInformation(
							"Trip transaction with TripTransactionId {request.TripTransactionId} doesn't exist",
							request.TripTransactionId);
						return Result<TripTransactionDto>.NotFound(
							$"Trip transaction with TripTransactionId {request.TripTransactionId} doesn't exist.");
					}

					TripTransactionDto tripTransaction = new();

					_mapper.Map(tripTransactionDb, tripTransaction);

					_logger.LogInformation(
						"Successfully retrieved trip transaction " +
						"based on transactionId {request.TripTransactionId}", request.TripTransactionId);
					return Result<TripTransactionDto>.Success(tripTransaction,
						"Successfully retrieved trip transaction " +
						$"based on transactionId {request.TripTransactionId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<TripTransactionDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
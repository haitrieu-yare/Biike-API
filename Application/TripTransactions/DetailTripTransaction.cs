using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.TripTransactions.DTOs;
using AutoMapper;
using MediatR;
using Persistence;

namespace Application.TripTransactions
{
	public class DetailTripTransaction
	{
		public class Query : IRequest<Result<TripTransactionDTO>>
		{
			public int TripTransactionId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<TripTransactionDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailTripTransaction> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailTripTransaction> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<TripTransactionDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var tripTransactionDB = await _context.TripTransaction
						.FindAsync(new object[] { request.TripTransactionId }, cancellationToken);

					TripTransactionDTO tripTransaction = new TripTransactionDTO();

					_mapper.Map(tripTransactionDB, tripTransaction);

					_logger.LogInformation("Successfully retrieved trip transaction " +
						$"based on transactionId {request.TripTransactionId}.");
					return Result<TripTransactionDTO>.Success(
						tripTransaction, "Successfully retrieved trip transaction " +
						$"based on transactionId {request.TripTransactionId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<TripTransactionDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}

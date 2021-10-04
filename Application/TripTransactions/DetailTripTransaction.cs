using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
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
				_mapper = mapper;
				_context = context;
				_logger = logger;
			}

			public async Task<Result<TripTransactionDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var TripTransaction = await _context.TripTransaction
						.Where(t => t.TripTransactionId == request.TripTransactionId)
						.ProjectTo<TripTransactionDTO>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved trip transaction based on transactionId");
					return Result<TripTransactionDTO>
						.Success(TripTransaction, "Successfully retrieved trip transaction based on transactionId");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<TripTransactionDTO>.Failure("Request was cancelled");
				}
			}
		}
	}
}
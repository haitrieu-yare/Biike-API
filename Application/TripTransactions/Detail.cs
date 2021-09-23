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

namespace Application.TripTransactions
{
	public class Detail
	{
		public class Query : IRequest<Result<TripTransactionDTO>>
		{
			public int Id { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<TripTransactionDTO>>
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

			public async Task<Result<TripTransactionDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var TripTransaction = await _context.TripTransaction
						.Where(t => t.Id == request.Id)
						.ProjectTo<TripTransactionDTO>(_mapper.ConfigurationProvider)
						.SingleOrDefaultAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved trip transaction based on transactionId");
					return Result<TripTransactionDTO>.Success(TripTransaction);
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
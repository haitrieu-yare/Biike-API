using System.Collections.Generic;
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
	public class ListTripTransactions
	{
		public class Query : IRequest<Result<List<TripTransactionDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<TripTransactionDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListTripTransactions> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListTripTransactions> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<TripTransactionDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var tripTransactions = await _context.TripTransaction
						.ProjectTo<TripTransactionDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all trip transaction");
					return Result<List<TripTransactionDTO>>
						.Success(tripTransactions, "Successfully retrieved list of all trip transaction");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<TripTransactionDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}
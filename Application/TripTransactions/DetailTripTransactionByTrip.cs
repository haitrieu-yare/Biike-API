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
	public class DetailTripTransactionByTrip
	{
		public class Query : IRequest<Result<List<TripTransactionDTO>>>
		{
			public int TripId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<TripTransactionDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailTripTransactionByTrip> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailTripTransactionByTrip> logger)
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

					var tripTransaction = await _context.TripTransaction
						.Where(t => t.TripId == request.TripId)
						.ProjectTo<TripTransactionDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved trip transaction " +
						$"based on tripId {request.TripId}.");
					return Result<List<TripTransactionDTO>>.Success(
						tripTransaction, "Successfully retrieved trip transaction " +
						$"based on tripId {request.TripId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<List<TripTransactionDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
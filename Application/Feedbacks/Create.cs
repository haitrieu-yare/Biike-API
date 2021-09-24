using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.TripTransactions;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Feedbacks.DTOs
{
	public class Create
	{
		public class Command : IRequest<Result<Unit>>
		{
			public FeedbackDTO FeedbackDTO { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Create> _logger;
			private readonly IMapper _mapper;
			private readonly AutoCreate _autoCreate;
			public Handler(DataContext context, IMapper mapper, ILogger<Create> logger, AutoCreate autoCreate)
			{
				_autoCreate = autoCreate;
				_mapper = mapper;
				_logger = logger;
				_context = context;
			}
			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var newFeedback = new Feedback();
					_mapper.Map(request.FeedbackDTO, newFeedback);

					await _context.Feedback.AddAsync(newFeedback, cancellationToken);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new feedback");
						return Result<Unit>.Failure("Failed to create new feedback");
					}
					else
					{
						_logger.LogInformation("Successfully created feedback");

						var trip = await _context.Trip
							.Where(t => t.Id == request.FeedbackDTO.TripId)
							.SingleOrDefaultAsync(cancellationToken);

						if (request.FeedbackDTO.UserId == trip.KeerId)
						{
							switch (newFeedback.Star)
							{
								case 4:
									return await _autoCreate.Run(trip, 5, cancellationToken);
								case 5:
									return await _autoCreate.Run(trip, 10, cancellationToken);
							}
						}

						return Result<Unit>.Success(Unit.Value);
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.Message);
					return Result<Unit>.Failure(ex.Message);
				}
			}
		}
	}
}
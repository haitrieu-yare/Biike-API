using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.TripTransactions;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Persistence;

namespace Application.Feedbacks.DTOs
{
	public class CreateFeedback
	{
		public class Command : IRequest<Result<Unit>>
		{
			public FeedbackCreateDTO FeedbackCreateDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<CreateFeedback> _logger;
			private readonly IMapper _mapper;
			private readonly AutoCreateTripTransaction _autoCreate;
			public Handler(DataContext context, IMapper mapper, ILogger<CreateFeedback> logger, AutoCreateTripTransaction autoCreate)
			{
				_context = context;
				_mapper = mapper;
				_autoCreate = autoCreate;
				_logger = logger;
			}
			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var trip = await _context.Trip
						.FindAsync(new object[] { request.FeedbackCreateDTO.TripId! }, cancellationToken);

					if (trip == null) return null!;

					if (trip.Status == (int)TripStatus.Cancelled)
					{
						_logger.LogInformation("Trip has already been cancelled.");
						return Result<Unit>.Failure("Trip has already been cancelled.");
					}
					else if (trip.Status != (int)TripStatus.Finished)
					{
						_logger.LogInformation("Can't create feedback because trip hasn't finished yet.");
						return Result<Unit>.Failure("Can't create feedback because trip hasn't finished yet.");
					}

					var feedbacks = await _context.Feedback
						.Where(f => f.TripId == request.FeedbackCreateDTO.TripId)
						.ToListAsync(cancellationToken);

					var existedFeedback = feedbacks.Find(f => f.UserId == request.FeedbackCreateDTO.UserId);

					if (existedFeedback != null)
					{
						_logger.LogInformation("Trip feedback is already existed.");
						return Result<Unit>.Failure("Trip feedback is already existed.");
					}

					Feedback newFeedback = new Feedback();

					_mapper.Map(request.FeedbackCreateDTO, newFeedback);

					await _context.Feedback.AddAsync(newFeedback, cancellationToken);

					try
					{
						// Create new transaction to add more point to Biker
						if (request.FeedbackCreateDTO.UserId == trip.KeerId)
						{
							switch (newFeedback.Star)
							{
								case 4:
									await _autoCreate.Run(trip, 5, cancellationToken);
									break;
								case 5:
									await _autoCreate.Run(trip, 10, cancellationToken);
									break;
							}
						}

						_logger.LogInformation("Successfully created feedback.");
						return Result<Unit>.Success(Unit.Value,
							"Successfully created feedback.", newFeedback.FeedbackId.ToString());
					}
					catch (System.Exception ex)
					{
						_logger.LogInformation("Failed to create new feedback.");
						_logger.LogInformation(ex.InnerException?.Message ?? ex.Message);

						return Result<Unit>.Failure(
							$"Failed to create new feedback. {ex.InnerException?.Message ?? ex.Message}");
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}
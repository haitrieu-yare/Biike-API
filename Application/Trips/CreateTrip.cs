using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Trips.DTOs;
using AutoMapper;
using Domain;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Application.Trips
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class CreateTrip
	{
		public class Command : IRequest<Result<Unit>>
		{
			public TripCreateDto TripCreateDto { get; init; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Handler> _logger;
			private readonly IMapper _mapper;
			private readonly ISchedulerFactory _schedulerFactory;

			public Handler(DataContext context, IMapper mapper, ISchedulerFactory schedulerFactory,
				ILogger<Handler> logger)
			{
				_context = context;
				_mapper = mapper;
				_schedulerFactory = schedulerFactory;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					Trip newTrip = new();

					_mapper.Map(request.TripCreateDto, newTrip);

					await _context.Trip.AddAsync(newTrip, cancellationToken);

					bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new trip");
						return Result<Unit>.Failure("Failed to create new trip.");
					}

					string jobName = ConstantString.GetJobNameAutoCancellation(newTrip.TripId);

					IJobDetail job = JobBuilder.Create<AutoTripCancellation>()
						.WithIdentity(jobName, ConstantString.OneTimeJob)
						.UsingJobData("TripId", $"{newTrip.TripId}")
						.Build();

					string triggerName = ConstantString.GetTriggerNameAutoCancellation(newTrip.TripId);

					// Trigger the job to run now, and then repeat every 3 seconds
					ITrigger trigger = TriggerBuilder.Create()
						.WithIdentity(triggerName, ConstantString.OneTimeJob)
						.StartAt(CurrentTime.ToLocalTime(newTrip.BookTime))
						.Build();

					// Tell quartz to schedule the job using the trigger
					IScheduler scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
					await scheduler.ScheduleJob(job, trigger, cancellationToken);
					_logger.LogInformation("Successfully created cancellation job");

					_logger.LogInformation("Successfully created trip");
					return Result<Unit>.Success(Unit.Value, "Successfully created trip.", newTrip.TripId.ToString());
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}
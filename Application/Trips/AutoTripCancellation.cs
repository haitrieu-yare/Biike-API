using System;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Application.Trips
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class AutoTripCancellation : IJob
	{
		private readonly DataContext _context;
		private readonly ILogger<AutoTripCancellation> _logger;

		public AutoTripCancellation(DataContext context, ILogger<AutoTripCancellation> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			JobDataMap dataMap = context.JobDetail.JobDataMap;
			var tripId = Convert.ToInt32(dataMap.GetString("TripId"));

			if (tripId == 0)
			{
				_logger.LogError("Could not find tripId for trip cancellation");
				return;
			}

			Trip trip = await _context.Trip.FindAsync(tripId);

			if (trip == null)
			{
				_logger.LogError("Trip with {TripId} doesn't exist", tripId);
				return;
			}

			switch (trip.Status)
			{
				case (int) TripStatus.Finished:
					_logger.LogInformation("Trip has already finished");
					return;
				case (int) TripStatus.Cancelled:
					_logger.LogInformation("Trip has already cancelled");
					return;
				case (int) TripStatus.Finding:
					trip.CancelReason = "Tự động hủy vì đã quá giờ khởi hành.";
					trip.Status = (int) TripStatus.Cancelled;
					trip.FinishedTime = CurrentTime.GetCurrentTime();
					break;
			}

			bool result = await _context.SaveChangesAsync() > 0;

			if (!result)
			{
				_logger.LogError("Failed to automatically cancel trip with TripId {TripId}", tripId);
				return;
			}

			_logger.LogInformation("Successfully automatically cancelled trip with TripId {TripId}", tripId);
		}
	}
}
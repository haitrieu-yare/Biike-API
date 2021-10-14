using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Quartz;

namespace Application.Trips
{
	// ReSharper disable once ClassNeverInstantiated.Global
	public class StartupTripAutoCancellation : IJob
	{
		private readonly DataContext _context;
		private readonly ISchedulerFactory _schedulerFactory;

		public StartupTripAutoCancellation (DataContext context, ISchedulerFactory schedulerFactory)
		{
			_context = context;
			_schedulerFactory = schedulerFactory;
		}

		public async Task Execute(IJobExecutionContext context)
		{
			List<Trip> trips = await _context.Trip.Where(t => t.Status == (int) TripStatus.Finding).ToListAsync();

			foreach (var trip in trips)
			{
				await CreateAutoTripCancellation.Run(_schedulerFactory, trip);
			}
		}
	}
}
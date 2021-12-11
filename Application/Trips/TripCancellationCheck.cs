using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Trips
{
    public class TripCancellationCheck
    {
        private readonly DataContext _context;
        private readonly ILogger<TripCancellationCheck> _logger;

        public TripCancellationCheck(DataContext context, ILogger<TripCancellationCheck> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsLimitExceeded(int userId)
        {
            var isCancellationLimitOn = await _context.Configuration
                .Where(configuration => configuration.ConfigurationName.Equals("IsCancellationLimitOn"))
                .Select(configuration => configuration.ConfigurationValue)
                .SingleOrDefaultAsync();

            if (string.IsNullOrEmpty(isCancellationLimitOn))
            {
                _logger.LogError("Can not find IsCancellationLimitOn configuration");
                return false;
            }

            if (!isCancellationLimitOn.Equals("true"))
            {
                return false;
            }
            
            var cancellationLimitConfig = await _context.Configuration
                .Where(configuration => configuration.ConfigurationName.Equals("CancellationLimit"))
                .Select(configuration => configuration.ConfigurationValue)
                .SingleOrDefaultAsync();
            
            if (string.IsNullOrEmpty(cancellationLimitConfig))
            {
                _logger.LogError("Can not find CancellationLimit configuration");
                return false;
            }
            
            if (!int.TryParse(cancellationLimitConfig, out var cancellationLimit))
            {
                _logger.LogError("CancellationLimit configuration's value is error");
                return false;
            }

            if (cancellationLimit < 0)
            {
                _logger.LogError("CancellationLimit configuration's value is error");
                return false;
            }
            
            var currentDate = CurrentTime.GetCurrentTime();

            var cancelledTripInDay = await _context.Trip
                .Where(trip => trip.KeerId == userId || trip.BikerId == userId)
                .Where(trip => trip.CancelTime.HasValue && trip.CancelTime.Value.Day == currentDate.Day)
                .Where(trip => trip.Status == (int) TripStatus.Cancelled)
                .CountAsync();

            return cancelledTripInDay > cancellationLimit;
        }
    }
}
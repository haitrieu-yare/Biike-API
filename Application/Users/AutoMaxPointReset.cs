using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Application.Users
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AutoMaxPointReset : IJob
    {
        private readonly DataContext _context;
        private readonly ILogger<AutoMaxPointReset> _logger;

        public AutoMaxPointReset(DataContext context, ILogger<AutoMaxPointReset> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            List<User> users = await _context.User.Where(u => u.IsBikeVerified == true).ToListAsync();

            foreach (var user in users)
            {
                user.MaxTotalPoint = 0;
            }

            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                _logger.LogError("Failed to automatically resetting user max total points");
                return;
            }

            _logger.LogInformation("Successfully automatically resetting user max total points");
        }
    }
}
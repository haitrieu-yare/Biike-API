using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.PointHistory
{
    public class AutoPointHistoryCreation
    {
        private readonly DataContext _context;
        private readonly ILogger<AutoPointHistoryCreation> _logger;

        public AutoPointHistoryCreation(DataContext context, ILogger<AutoPointHistoryCreation> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Run(int userId, int historyType, int relatedId, int point, int totalPoint,
            string description, DateTime timeStamp)
        {
            var pointHistory = new Domain.Entities.PointHistory
            {
                UserId = userId,
                HistoryType = historyType,
                RelatedId = relatedId,
                Point = point,
                TotalPoint = totalPoint,
                Description = description,
                TimeStamp = timeStamp
            };

            await _context.PointHistory.AddAsync(pointHistory);
            
            var result = await _context.SaveChangesAsync() > 0;

            if (!result)
            {
                _logger.LogError("Failed to create new point history");
            }

            _logger.LogInformation("Successfully created point history");
        }
    }
}
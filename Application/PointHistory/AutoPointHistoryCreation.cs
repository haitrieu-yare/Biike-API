using System;
using System.Threading.Tasks;
using Application.Notifications;
using Application.Notifications.DTOs;
using Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.PointHistory
{
    public class AutoPointHistoryCreation
    {
        private readonly DataContext _context;
        private readonly NotificationSending _notiSender;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutoPointHistoryCreation> _logger;

        public AutoPointHistoryCreation(DataContext context, NotificationSending notiSender,
            IConfiguration configuration ,ILogger<AutoPointHistoryCreation> logger)
        {
            _context = context;
            _notiSender = notiSender;
            _configuration = configuration;
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

            var message = point < 0 ? 
                    $"Bạn đã sử dụng {point * -1} điểm, số điểm mới của bạn là {totalPoint}" :
                    $"Bạn đã được cộng thêm {point} điểm, số điểm mới của bạn là {totalPoint}";
            
            // ReSharper disable StringLiteralTypo
            var notification = new NotificationDto
            {
                NotificationId = Guid.NewGuid(),
                Title = "Điểm của bạn đã được thay đổi",
                Content = message,
                ReceiverId = userId,
                Url = $"{_configuration["ApiPath"]}/points",
                IsRead = false,
                CreatedDate = CurrentTime.GetCurrentTime()
            };
            // ReSharper restore StringLiteralTypo

            await _notiSender.Run(notification);

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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;

namespace Persistence.Data
{
    public static class FeedbackSeed
    {
        public static async Task SeedData(DataContext context)
        {
            if (context.Feedback.Any()) return;

            var currentTime = CurrentTime.GetCurrentTime();

            var feedbackList = new List<Feedback>
            {
                new()
                {
                    UserId = 1,
                    TripId = 1,
                    FeedbackContent = "Chuyến đi rất là thoải mái luôn.",
                    TripStar = 5,
                    Criteria = "Dịch Vụ Tốt",
                    CreatedDate = currentTime.AddDays(-9).AddSeconds(900)
                },
                new()
                {
                    UserId = 3,
                    TripId = 1,
                    FeedbackContent = "Mọi chuyện suôn sẻ, thuận lợi.",
                    TripStar = 5,
                    Criteria = "Đúng Giờ",
                    CreatedDate = currentTime.AddDays(-9).AddSeconds(800)
                },
                new()
                {
                    UserId = 2,
                    TripId = 6,
                    FeedbackContent = "Tài xế đến hơi trễ xíu.",
                    TripStar = 4,
                    Criteria = "Cần Đến Đúng Giờ",
                    CreatedDate = currentTime.AddDays(-2).AddSeconds(420 + 600 + 300)
                },
                new()
                {
                    UserId = 3,
                    TripId = 6,
                    FeedbackContent = "Bạn dễ thương.",
                    TripStar = 5,
                    Criteria = "Thân Thiện",
                    CreatedDate = currentTime.AddDays(-2).AddSeconds(420 + 600 + 400)
                }
            };

            // Save change for each item because EF doesn't insert like the order
            // we define in our list.
            foreach (var feedback in feedbackList)
            {
                await context.Feedback.AddAsync(feedback);
                await context.SaveChangesAsync();
            }
        }
    }
}
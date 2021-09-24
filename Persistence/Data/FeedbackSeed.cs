using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;

namespace Persistence.Data
{
	public static class FeedbackSeed
	{
		public static async Task SeedData(DataContext context)
		{
			if (context.Feedback.Any()) return;

			var feedbackList = new List<Feedback>
			{
				new Feedback
				{
					AppUserId = 1,
					TripId = 1,
					FeedbackContent = "Chuyến đi rất là thoải mái luôn.",
					Star = 5,
					Criteria = "Dịch Vụ Tốt"
				},
				new Feedback
				{
					AppUserId = 4,
					TripId = 1,
					FeedbackContent = "Mọi chuyện suôn sẻ, thuận lợi.",
					Star = 5,
					Criteria = "Đúng Giờ"
				},
				new Feedback
				{
					AppUserId = 2,
					TripId = 2,
					FeedbackContent = "Tài xế đến hơi trễ xíu.",
					Star = 4,
					Criteria = "Cần Đến Đúng Giờ"
				},
				new Feedback
				{
					AppUserId = 5,
					TripId = 2,
					FeedbackContent = "Bạn nữ dễ thương.",
					Star = 5,
					Criteria = "Thân Thiện"
				},
				new Feedback
				{
					AppUserId = 6,
					TripId = 3,
					FeedbackContent = "Bạn này chạy hơi ẩu.",
					Star = 3,
					Criteria = "Chạy Ẩu"
				},
				new Feedback
				{
					AppUserId = 7,
					TripId = 3,
					FeedbackContent = "Bạn này nói hơi nhiều.",
					Star = 4,
					Criteria = "Không Thân Thiện"
				},
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
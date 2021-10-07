using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Feedbacks.DTOs;
using AutoMapper;
using MediatR;
using Persistence;

namespace Application.Feedbacks
{
	public class DetailFeedback
	{
		public class Query : IRequest<Result<FeedbackDTO>>
		{
			public int FeedbackId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<FeedbackDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListFeedbacksByTrip> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListFeedbacksByTrip> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<FeedbackDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var feedbackDB = await _context.Feedback
						.FindAsync(new object[] { request.FeedbackId }, cancellationToken);

					FeedbackDTO feedback = new FeedbackDTO();

					_mapper.Map(feedbackDB, feedback);

					_logger.LogInformation($"Successfully retrieved feedback by FeedbackId {request.FeedbackId}.");
					return Result<FeedbackDTO>.Success(
						feedback, $"Successfully retrieved feedback by FeedbackId {request.FeedbackId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<FeedbackDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
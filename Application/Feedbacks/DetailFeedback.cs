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
		public class Query : IRequest<Result<FeedbackDto>>
		{
			public int FeedbackId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<FeedbackDto>>
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

			public async Task<Result<FeedbackDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var feedbackDb = await _context.Feedback
						.FindAsync(new object[] { request.FeedbackId }, cancellationToken);

					FeedbackDto feedback = new FeedbackDto();

					_mapper.Map(feedbackDb, feedback);

					_logger.LogInformation($"Successfully retrieved feedback by FeedbackId {request.FeedbackId}.");
					return Result<FeedbackDto>.Success(
						feedback, $"Successfully retrieved feedback by FeedbackId {request.FeedbackId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<FeedbackDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
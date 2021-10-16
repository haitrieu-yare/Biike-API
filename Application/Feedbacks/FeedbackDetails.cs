using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Feedbacks.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Feedbacks
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FeedbackDetails
    {
        public class Query : IRequest<Result<FeedbackDto>>
        {
            public int FeedbackId { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<FeedbackDto>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
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

                    Domain.Entities.Feedback feedbackDb =
                        await _context.Feedback.FindAsync(new object[] {request.FeedbackId}, cancellationToken);

                    FeedbackDto feedback = new();

                    _mapper.Map(feedbackDb, feedback);

                    _logger.LogInformation("Successfully retrieved feedback by FeedbackId {request.FeedbackId}",
                        request.FeedbackId);
                    return Result<FeedbackDto>.Success(feedback,
                        $"Successfully retrieved feedback by FeedbackId {request.FeedbackId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<FeedbackDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}
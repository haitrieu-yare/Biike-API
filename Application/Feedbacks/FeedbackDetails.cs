using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Feedbacks.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

        // ReSharper disable once UnusedType.Global
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

                    FeedbackDto feedback = await _context.Feedback.Where(f => f.FeedbackId == request.FeedbackId)
                        .ProjectTo<FeedbackDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (feedback == null)
                    {
                        _logger.LogInformation("Feedback doesn't exist");
                        return Result<FeedbackDto>.NotFound("Feedback doesn't exist.");
                    }

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
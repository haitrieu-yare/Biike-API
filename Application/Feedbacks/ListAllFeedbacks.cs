using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Feedbacks.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Feedbacks
{
	public class ListAllFeedbacks
	{
		public class Query : IRequest<Result<List<FeedbackDTO>>>
		{
			public bool IsAdmin { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<FeedbackDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListAllFeedbacks> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListAllFeedbacks> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<FeedbackDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var feedbacks = await _context.Feedback
						.ProjectTo<FeedbackDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					if (!request.IsAdmin)
					{
						// Set to null to make unnecessary fields excluded from response body.
						feedbacks.ForEach(f => f.CreatedDate = null);
					}

					_logger.LogInformation("Successfully retrieved all trip's feedbacks.");
					return Result<List<FeedbackDTO>>.Success(feedbacks, "Successfully retrieved all trip's feedbacks.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<List<FeedbackDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
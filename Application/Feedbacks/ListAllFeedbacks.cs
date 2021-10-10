using System;
using System.Collections.Generic;
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
	public class ListAllFeedbacks
	{
		public class Query : IRequest<Result<List<FeedbackDto>>>
		{
			public int Page { get; init; }
			public int Limit { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<List<FeedbackDto>>>
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

			public async Task<Result<List<FeedbackDto>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must be larger than 0");
						return Result<List<FeedbackDto>>.Failure("Page must be larger than 0.");
					}

					int totalRecord = await _context.Feedback.CountAsync(cancellationToken);

					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);

					List<FeedbackDto> feedbacks = new();

					if (request.Page <= lastPage)
						feedbacks = await _context.Feedback.OrderBy(f => f.FeedbackId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<FeedbackDto>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

					PaginationDto paginationDto = new(
						request.Page, request.Limit, feedbacks.Count, lastPage, totalRecord);

					_logger.LogInformation("Successfully retrieved all trip's feedbacks");
					return Result<List<FeedbackDto>>.Success(feedbacks, "Successfully retrieved all trip's feedbacks.",
						paginationDto);
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<FeedbackDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
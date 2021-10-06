using System.Collections.Generic;
using System.Linq;
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
			public bool IsAdmin { get; init; }
			public int Page { get; init; }
			public int Limit { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<List<FeedbackDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Handler> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
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

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<FeedbackDTO>>.Failure("Page must larger than 0.");
					}

					int totalRecord = await _context.Feedback.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<FeedbackDTO> feedbacks = new List<FeedbackDTO>();

					if (request.Page <= lastPage)
					{
						feedbacks = await _context.Feedback
							.OrderBy(f => f.FeedbackId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<FeedbackDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

						if (!request.IsAdmin)
						{
							// Set to null to make unnecessary fields excluded from response body.
							feedbacks.ForEach(f => f.CreatedDate = null);
						}
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, feedbacks.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved all trip's feedbacks");
					return Result<List<FeedbackDTO>>.Success(
						feedbacks, "Successfully retrieved all trip's feedbacks.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<FeedbackDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
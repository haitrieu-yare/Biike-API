using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Intimacies.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Intimacies
{
	public class ListIntimacies
	{
		public class Query : IRequest<Result<List<IntimacyDto>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<IntimacyDto>>>
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

			public async Task<Result<List<IntimacyDto>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<IntimacyDto>>.Failure("Page must larger than 0.");
					}

					int totalRecord = await _context.Intimacy.CountAsync(cancellationToken);

					#region Calculate last page

					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);

					#endregion

					List<IntimacyDto> intimacies = new();

					if (request.Page <= lastPage)
						intimacies = await _context.Intimacy
							.OrderBy(i => i.UserOneId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<IntimacyDto>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

					PaginationDto paginationDto = new(
						request.Page, request.Limit, intimacies.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all intimacies");
					return Result<List<IntimacyDto>>.Success(
						intimacies, "Successfully retrieved list of all intimacies.", paginationDto);
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<IntimacyDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
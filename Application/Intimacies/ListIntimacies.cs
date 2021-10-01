using System.Collections.Generic;
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
		public class Query : IRequest<Result<List<IntimacyDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<IntimacyDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListIntimacies> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListIntimacies> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<IntimacyDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var intimacies = await _context.Intimacy
						.ProjectTo<IntimacyDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all intimacies");
					return Result<List<IntimacyDTO>>.Success(intimacies);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<IntimacyDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Intimacies.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Intimacies
{
	public class DetailIntimacy
	{
		public class Query : IRequest<Result<List<IntimacyDTO>>>
		{
			public int UserOneId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<IntimacyDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailIntimacy> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailIntimacy> logger)
			{
				_mapper = mapper;
				_context = context;
				_logger = logger;
			}

			public async Task<Result<List<IntimacyDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var intimacies = await _context.Intimacy
						.Where(i => i.UserOneId == request.UserOneId)
						.ProjectTo<IntimacyDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of user intimacies");
					return Result<List<IntimacyDTO>>.Success(intimacies, "Successfully retrieved list of user intimacies");
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
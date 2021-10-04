using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Redemptions.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Redemptions
{
	public class ListRedemption
	{
		public class Query : IRequest<Result<List<RedemptionDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<RedemptionDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListRedemption> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListRedemption> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<RedemptionDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var redemptions = await _context.Redemption
						.ProjectTo<RedemptionDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all redemptions.");
					return Result<List<RedemptionDTO>>.Success(
						redemptions, "Successfully retrieved list of all redemptions.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<List<RedemptionDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
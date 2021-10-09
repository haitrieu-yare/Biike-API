using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Redemptions.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Redemptions
{
	public class DetailRedemption
	{
		public class Query : IRequest<Result<RedemptionDto>>
		{
			public int RedemptionId { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<RedemptionDto>>
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

			public async Task<Result<RedemptionDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					Redemption redemptionDb =
						await _context.Redemption.FindAsync(new object[] { request.RedemptionId }, cancellationToken);

					if (redemptionDb == null)
					{
						_logger.LogInformation("Redemption doesn't exist");
						return Result<RedemptionDto>.NotFound("Redemption doesn't exist.");
					}

					RedemptionDto redemption = new();

					_mapper.Map(redemptionDb, redemption);

					_logger.LogInformation("Successfully retrieved redemption by {request.RedemptionId}",
						request.RedemptionId);
					return Result<RedemptionDto>.Success(redemption,
						$"Successfully retrieved redemption by {request.RedemptionId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<RedemptionDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
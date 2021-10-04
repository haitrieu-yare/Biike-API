using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Redemptions.DTOs;
using AutoMapper;
using MediatR;
using Persistence;

namespace Application.Redemptions
{
	public class DetailRedemption
	{
		public class Query : IRequest<Result<RedemptionDTO>>
		{
			public int RedemptionId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<RedemptionDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListUserRedemption> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListUserRedemption> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<RedemptionDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var redemptionDB = await _context.Redemption
						.FindAsync(new object[] { request.RedemptionId }, cancellationToken);

					RedemptionDTO redemption = new RedemptionDTO();

					_mapper.Map(redemptionDB, redemption);

					_logger.LogInformation($"Successfully retrieved redemption by {request.RedemptionId}.");
					return Result<RedemptionDTO>.Success(
						redemption, $"Successfully retrieved redemption by {request.RedemptionId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<RedemptionDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
	public class DetailVoucher
	{
		public class Query : IRequest<Result<VoucherDto>>
		{
			public int VoucherId { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<VoucherDto>>
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

			public async Task<Result<VoucherDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					Voucher voucherDb =
						await _context.Voucher.FindAsync(new object[] { request.VoucherId }, cancellationToken);

					if (voucherDb == null)
					{
						_logger.LogInformation("Voucher doesn't exist");
						return Result<VoucherDto>.NotFound("Voucher doesn't exist.");
					}

					VoucherDto voucher = new();

					_mapper.Map(voucherDb, voucher);

					_logger.LogInformation("Successfully retrieved voucher " + "by voucherId {request.VoucherId}",
						request.VoucherId);
					return Result<VoucherDto>.Success(voucher,
						$"Successfully retrieved voucher by voucherId {request.VoucherId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<VoucherDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
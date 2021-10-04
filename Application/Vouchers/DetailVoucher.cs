using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.Vouchers
{
	public class DetailVoucher
	{
		public class Query : IRequest<Result<VoucherDTO>>
		{
			public int VoucherId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<VoucherDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailVoucher> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailVoucher> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<VoucherDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					VoucherDTO voucher = new VoucherDTO();

					Voucher voucherDB = await _context.Voucher
						.FindAsync(new object[] { request.VoucherId }, cancellationToken);

					_mapper.Map(voucherDB, voucher);

					_logger.LogInformation("Successfully retrieved voucher " +
						$"by voucherId {request.VoucherId}.");
					return Result<VoucherDTO>.Success(voucher,
						$"Successfully retrieved voucher by voucherId {request.VoucherId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<VoucherDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
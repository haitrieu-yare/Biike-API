using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using MediatR;
using Persistence;

namespace Application.Vouchers
{
	public class EditVoucher
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int VoucherId { get; set; }
			public VoucherEditDTO NewVoucher { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<EditVoucher> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<EditVoucher> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var oldVoucher = await _context.Voucher
						.FindAsync(new object[] { request.VoucherId }, cancellationToken);

					if (oldVoucher == null) return null!;

					_mapper.Map(request.NewVoucher, oldVoucher);

					if (oldVoucher.EndDate.CompareTo(oldVoucher.StartDate) < 0)
					{
						_logger.LogInformation("EndDate must be set later than StartDate.");
						return Result<Unit>.Failure("EndDate must be set later than StartDate.");
					}

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation($"Failed to update voucher by voucherId {request.VoucherId}.");
						return Result<Unit>.Failure($"Failed to update voucher by voucherId {request.VoucherId}.");
					}
					else
					{
						_logger.LogInformation($"Successfully updated voucher by voucherId {request.VoucherId}.");
						return Result<Unit>.Success(
							Unit.Value, $"Successfully updated voucher by voucherId {request.VoucherId}.");
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}
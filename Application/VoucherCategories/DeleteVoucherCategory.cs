using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using MediatR;
using Persistence;

namespace Application.VoucherCategories
{
	public class DeleteVoucherCategory
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int VoucherCategoryId { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<DeleteVoucherCategory> _logger;
			public Handler(DataContext context, ILogger<DeleteVoucherCategory> logger)
			{
				_context = context;
				_logger = logger;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var voucherCategory = await _context.VoucherCategory
						.FindAsync(new object[] { request.VoucherCategoryId }, cancellationToken);

					if (voucherCategory == null) return null!;

					_context.VoucherCategory.Remove(voucherCategory);

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation($"Failed to delete voucher's category " +
							$"by voucherCategoryId {request.VoucherCategoryId}.");
						return Result<Unit>.Failure($"Failed to delete voucher's category " +
							$"by voucherCategoryId {request.VoucherCategoryId}.");
					}
					else
					{
						_logger.LogInformation($"Successfully deleted voucher's category " +
							$"by voucherCategoryId {request.VoucherCategoryId}.");
						return Result<Unit>.Success(Unit.Value, $"Successfully deleted voucher's category " +
							$"by voucherCategoryId {request.VoucherCategoryId}.");
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
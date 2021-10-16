using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.VoucherCategories
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherCategoryDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int VoucherCategoryId { get; init; }
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Domain.Entities.VoucherCategory voucherCategory =
                        await _context.VoucherCategory.FindAsync(new object[] {request.VoucherCategoryId},
                            cancellationToken);

                    if (voucherCategory == null)
                    {
                        _logger.LogInformation("Voucher category doesn't exist");
                        return Result<Unit>.NotFound("Voucher category doesn't exist.");
                    }

                    _context.VoucherCategory.Remove(voucherCategory);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation(
                            "Failed to delete voucher's category " + "by voucherCategoryId {request.VoucherCategoryId}",
                            request.VoucherCategoryId);
                        return Result<Unit>.Failure("Failed to delete voucher's category " +
                                                    $"by voucherCategoryId {request.VoucherCategoryId}.");
                    }

                    _logger.LogInformation(
                        "Successfully deleted voucher's category " + "by voucherCategoryId {request.VoucherCategoryId}",
                        request.VoucherCategoryId);
                    return Result<Unit>.Success(Unit.Value,
                        "Successfully deleted voucher's category " +
                        $"by voucherCategoryId {request.VoucherCategoryId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}
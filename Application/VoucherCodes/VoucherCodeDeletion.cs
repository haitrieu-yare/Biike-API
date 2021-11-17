using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.VoucherCodes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherCodeDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int voucherCodeId)
            {
                VoucherCodeId = voucherCodeId;
            }

            public int VoucherCodeId { get; }
        }

        // ReSharper disable once UnusedType.Global
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

                    VoucherCode voucherCode =
                        await _context.VoucherCode.FindAsync(new object[] {request.VoucherCodeId}, cancellationToken);

                    if (voucherCode == null)
                    {
                        _logger.LogInformation("Voucher code doesn't exist");
                        return Result<Unit>.NotFound("Voucher code doesn't exist.");
                    }

                    _context.VoucherCode.Remove(voucherCode);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete voucher code " + "by voucherCodeId {VoucherCodeId}",
                            request.VoucherCodeId);
                        return Result<Unit>.Failure("Failed to delete voucher code " +
                                                    $"by voucherCodeId {request.VoucherCodeId}.");
                    }

                    _logger.LogInformation("Successfully deleted voucher code by voucherCodeId {VoucherCodeId}",
                        request.VoucherCodeId);
                    return Result<Unit>.Success(Unit.Value,
                        "Successfully deleted voucher code " + $"by voucherCodeId {request.VoucherCodeId}.");
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
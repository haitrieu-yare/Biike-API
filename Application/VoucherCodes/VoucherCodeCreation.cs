using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.VoucherCodes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherCodeCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int voucherId, List<string> voucherCodes)
            {
                VoucherCodes = voucherCodes;
                VoucherId = voucherId;
            }

            public List<string> VoucherCodes { get; }
            public int VoucherId { get; }
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

                    List<VoucherCode> newVoucherCodes = request.VoucherCodes.Select(voucherCode =>
                            new VoucherCode {VoucherId = request.VoucherId, VoucherCodeName = voucherCode,})
                        .ToList();

                    await _context.VoucherCode.AddRangeAsync(newVoucherCodes, cancellationToken);
                    var voucherCodeResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!voucherCodeResult)
                    {
                        _logger.LogInformation("Failed to create new voucher codes");
                        return Result<Unit>.Failure("Failed to create new voucher codes.");
                    }

                    _logger.LogInformation("Successfully created new voucher codes");
                    return Result<Unit>.Success(Unit.Value, "Successfully created new voucher codes.",
                        newVoucherCodes.First().VoucherCodeId.ToString());
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
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
    public class VoucherCodeDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(List<int> voucherCodeIds)
            {
                VoucherCodeIds = voucherCodeIds;
            }

            public List<int> VoucherCodeIds { get; }
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

                    List<VoucherCode> voucherCodes = new();

                    foreach (var voucherCodeId in request.VoucherCodeIds)
                    {
                        var voucherCode =
                            await _context.VoucherCode.FindAsync(new object[] {voucherCodeId}, cancellationToken);

                        if (voucherCode == null)
                        {
                            _logger.LogInformation("Voucher code with {VoucherCodeId} doesn't exist", voucherCodeId);
                            return Result<Unit>.NotFound($"Voucher code with {voucherCodeId} doesn't exist.");
                        }

                        voucherCodes.Add(voucherCode);

                        var voucher =
                            await _context.Voucher.FindAsync(new object[] {voucherCode.VoucherId}, cancellationToken);

                        if (voucher == null)
                        {
                            _logger.LogInformation("Voucher with {VoucherId} doesn't exist", voucherCode.VoucherId);
                            return Result<Unit>.NotFound($"Voucher with {voucherCode.VoucherId} doesn't exist.");
                        }

                        voucher.Quantity--;
                        voucher.Remaining--;
                    }

                    _context.VoucherCode.RemoveRange(voucherCodes);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete list voucher code");
                        return Result<Unit>.Failure("Failed to delete list voucher code.");
                    }

                    _logger.LogInformation("Successfully deleted list voucher code");
                    return Result<Unit>.Success(Unit.Value, "Successfully deleted list voucher code.");
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
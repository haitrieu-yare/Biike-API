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

namespace Application.Vouchers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherImageCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int voucherId, List<string> voucherImages)
            {
                VoucherId = voucherId;
                VoucherImages = voucherImages;
            }

            public List<string> VoucherImages { get; }
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
                var voucher = await _context.Voucher.FindAsync(new object[] {request.VoucherId}, cancellationToken);

                if (voucher == null)
                {
                    _logger.LogInformation("Voucher doesn't exist");
                    return Result<Unit>.NotFound("Voucher doesn't exist.");
                }

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.VoucherImages.Count == 0)
                    {
                        _logger.LogInformation("Voucher image list must be provided");
                        return Result<Unit>.Failure("Voucher image list must be provided.");
                    }

                    List<VoucherImage> newVoucherImages = request.VoucherImages.Select(voucherImage =>
                            new VoucherImage {VoucherId = voucher.VoucherId, VoucherImageUrl = voucherImage})
                        .ToList();

                    await _context.VoucherImage.AddRangeAsync(newVoucherImages, cancellationToken);
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new voucher image");
                        return Result<Unit>.Failure("Failed to create new voucher image.");
                    }

                    _logger.LogInformation("Successfully created new voucher image");
                    return Result<Unit>.Success(Unit.Value, "Successfully created new voucher image.",
                        newVoucherImages.First().VoucherId.ToString());
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
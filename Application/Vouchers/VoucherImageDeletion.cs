using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Vouchers.DTOs;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherImageDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(VoucherImageDeletionDto voucherImageDeletionDto)
            {
                VoucherImageDeletionDto = voucherImageDeletionDto;
            }

            public VoucherImageDeletionDto VoucherImageDeletionDto { get; init; }
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

                    Voucher? voucher =
                        await _context.Voucher.FindAsync(new object[] {request.VoucherImageDeletionDto.VoucherId!},
                            cancellationToken);

                    if (voucher == null)
                    {
                        _logger.LogInformation("Voucher doesn't exist");
                        return Result<Unit>.NotFound("Voucher doesn't exist.");
                    }

                    if (request.VoucherImageDeletionDto.VoucherImageIds!.Count == 0)
                    {
                        _logger.LogInformation("There are no imageId in request");
                        return Result<Unit>.NotFound("There are no imageId in request.");
                    }

                    foreach (var voucherImageId in request.VoucherImageDeletionDto.VoucherImageIds)
                    {
                        var voucherImage = await _context.VoucherImage
                            .Where(a => a.VoucherImageId == int.Parse(voucherImageId))
                            .SingleOrDefaultAsync(cancellationToken);

                        if (voucherImage == null) continue;

                        if (voucherImage.VoucherId != request.VoucherImageDeletionDto.VoucherId)
                        {
                            _logger.LogInformation(
                                "VoucherImage with VoucherImageId {VoucherImageId} does not belong to voucher " +
                                "with VoucherId {VoucherId}", voucherImageId,
                                request.VoucherImageDeletionDto.VoucherId);
                            return Result<Unit>.NotFound(
                                $"VoucherImage with VoucherImageId {voucherImageId} does not belong to voucher " +
                                $"with VoucherId {request.VoucherImageDeletionDto.VoucherId}.");
                        }

                        _context.VoucherImage.Remove(voucherImage);
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete voucher image by voucherId {request.VoucherId}",
                            request.VoucherImageDeletionDto.VoucherId);
                        return Result<Unit>.Failure(
                            $"Failed to delete voucher image by voucherId {request.VoucherImageDeletionDto.VoucherId}.");
                    }

                    _logger.LogInformation("Successfully deleted voucher image by voucherId {request.VoucherId}",
                        request.VoucherImageDeletionDto.VoucherId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully deleted voucher image by voucherId {request.VoucherImageDeletionDto.VoucherId}.");
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
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
    public class VoucherAddressDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(VoucherAddressDeletionDto voucherAddressDeletionDto)
            {
                VoucherAddressDeletionDto = voucherAddressDeletionDto;
            }

            public VoucherAddressDeletionDto VoucherAddressDeletionDto { get; init; }
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

                    Voucher voucher =
                        await _context.Voucher.FindAsync(new object[] {request.VoucherAddressDeletionDto.VoucherId!},
                            cancellationToken);

                    if (voucher == null)
                    {
                        _logger.LogInformation("Voucher doesn't exist");
                        return Result<Unit>.NotFound("Voucher doesn't exist.");
                    }

                    if (request.VoucherAddressDeletionDto.AddressIds!.Count == 0)
                    {
                        _logger.LogInformation("There are no addressId in request");
                        return Result<Unit>.NotFound("There are no addressId in request.");
                    }

                    foreach (var addressId in request.VoucherAddressDeletionDto.AddressIds)
                    {
                        var address = await _context.Address
                            .Where(a => a.AddressId == int.Parse(addressId))
                            .Include(a => a.VoucherAddress)
                            .SingleOrDefaultAsync(cancellationToken);
                        
                        if (address == null) continue;

                        if (address.VoucherAddress == null || address.VoucherAddress.VoucherId !=
                            request.VoucherAddressDeletionDto.VoucherId)
                        {
                            _logger.LogInformation(
                                "Address with AddressId {AddressId} does not belong to voucher" +
                                "with VoucherId {VoucherId}", addressId, request.VoucherAddressDeletionDto.VoucherId);
                            return Result<Unit>.NotFound(
                                $"Address with AddressId {addressId} does not belong to voucher" +
                                $"with VoucherId {request.VoucherAddressDeletionDto.VoucherId}.");
                        }

                        _context.VoucherAddress.Remove(address.VoucherAddress);
                        _context.Address.Remove(address);
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete voucher address by voucherId {request.VoucherId}",
                            request.VoucherAddressDeletionDto.VoucherId);
                        return Result<Unit>.Failure(
                            $"Failed to delete voucher address by voucherId {request.VoucherAddressDeletionDto.VoucherId}.");
                    }

                    _logger.LogInformation("Successfully deleted voucher address by voucherId {request.VoucherId}",
                        request.VoucherAddressDeletionDto.VoucherId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully deleted voucher address by voucherId {request.VoucherAddressDeletionDto.VoucherId}.");
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
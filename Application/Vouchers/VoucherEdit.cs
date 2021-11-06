using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int VoucherId { get; init; }
            public VoucherEditDto NewVoucher { get; init; } = null!;
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
            {
                _context = context;
                _mapper = mapper;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Voucher oldVoucher =
                        await _context.Voucher.FindAsync(new object[] {request.VoucherId}, cancellationToken);

                    if (oldVoucher == null)
                    {
                        _logger.LogInformation("Voucher doesn't exist");
                        return Result<Unit>.NotFound("Voucher doesn't exist.");
                    }

                    _mapper.Map(request.NewVoucher, oldVoucher);

                    if (oldVoucher.EndDate.CompareTo(oldVoucher.StartDate) < 0)
                    {
                        _logger.LogInformation("EndDate must be set later than StartDate");
                        return Result<Unit>.Failure("EndDate must be set later than StartDate.");
                    }
                    
                    if (request.NewVoucher.VoucherAddresses != null && request.NewVoucher.VoucherAddresses.Count > 0)
                    {
                        foreach (var voucherAddressDto in request.NewVoucher.VoucherAddresses)
                        {
                            Address oldAddress =
                                await _context.Address.FindAsync(new object[] {voucherAddressDto.AddressId!},
                                    cancellationToken);

                            if (oldAddress == null)
                            {
                                _logger.LogInformation("Address with AddressId {AddressId} doesn't exist",
                                    voucherAddressDto.AddressId);
                                return Result<Unit>.NotFound(
                                    $"Address with AddressId {voucherAddressDto.AddressId} doesn't exist.");
                            }
                            
                            _mapper.Map(voucherAddressDto, oldAddress);
                        }
                    }
                    
                    if (request.NewVoucher.VoucherImages != null && request.NewVoucher.VoucherImages.Count > 0)
                    {
                        foreach (var voucherImageDto in request.NewVoucher.VoucherImages)
                        {
                            Image oldImage =
                                await _context.Image.FindAsync(new object[] {voucherImageDto.ImageId!},
                                    cancellationToken);
                            
                            if (oldImage == null)
                            {
                                _logger.LogInformation("Image with ImageId {ImageId} doesn't exist",
                                    voucherImageDto.ImageId);
                                return Result<Unit>.NotFound(
                                    $"Image with ImageId {voucherImageDto.ImageId} doesn't exist.");
                            }
                            
                            oldImage.ImageUrl = voucherImageDto.ImageUrl!;
                        }
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update voucher by voucherId {request.VoucherId}",
                            request.VoucherId);
                        return Result<Unit>.Failure($"Failed to update voucher by voucherId {request.VoucherId}.");
                    }

                    _logger.LogInformation("Successfully updated voucher by voucherId {request.VoucherId}",
                        request.VoucherId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully updated voucher by voucherId {request.VoucherId}.");
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
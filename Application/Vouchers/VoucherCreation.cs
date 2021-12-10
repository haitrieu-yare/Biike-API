using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(VoucherCreationDto voucherCreationDto)
            {
                VoucherCreationDto = voucherCreationDto;
            }

            public VoucherCreationDto VoucherCreationDto { get; }
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
                await using IDbContextTransaction transaction =
                    await _context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Voucher newVoucher = new();

                    _mapper.Map(request.VoucherCreationDto, newVoucher);

                    if (newVoucher.EndDate.CompareTo(newVoucher.StartDate) < 0)
                    {
                        _logger.LogInformation("EndDate must be set later than StartDate");
                        return Result<Unit>.Failure("EndDate must be set later than StartDate.");
                    }

                    await _context.Voucher.AddAsync(newVoucher, cancellationToken);
                    var voucherResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!voucherResult)
                    {
                        _logger.LogInformation("Failed to create new voucher");
                        return Result<Unit>.Failure("Failed to create new voucher.");
                    }

                    if (request.VoucherCreationDto.AddressIds.Count > 0)
                    {
                        List<VoucherAddress> voucherAddresses = request.VoucherCreationDto.AddressIds
                            .Select(addressId => new VoucherAddress
                            {
                                AddressId = addressId, 
                                VoucherId = newVoucher.VoucherId
                            }).ToList();
                        
                        await _context.VoucherAddress.AddRangeAsync(voucherAddresses, cancellationToken);
                        var voucherAddressResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                        if (!voucherAddressResult)
                        {
                            _logger.LogInformation("Failed to create new voucher");
                            return Result<Unit>.Failure("Failed to create new voucher.");
                        }
                    }

                    if (request.VoucherCreationDto.VoucherImages.Count > 0)
                    {
                        List<VoucherImage> voucherImages = request.VoucherCreationDto.VoucherImages
                            .Select(voucherImage => new VoucherImage
                            {
                                VoucherId = newVoucher.VoucherId, 
                                VoucherImageUrl = voucherImage
                            }).ToList();

                        await _context.VoucherImage.AddRangeAsync(voucherImages, cancellationToken);
                        var voucherImageResult = await _context.SaveChangesAsync(cancellationToken) > 0;

                        if (!voucherImageResult)
                        {
                            _logger.LogInformation("Failed to create new voucher");
                            return Result<Unit>.Failure("Failed to create new voucher.");
                        }
                    }

                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogInformation("Successfully created new voucher");
                    return Result<Unit>.Success(Unit.Value, "Successfully created new voucher.",
                        newVoucher.VoucherId.ToString());
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
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
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
    public class VoucherAddressCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int voucherId, List<VoucherAddressCreationDto> voucherAddressCreationDtos)
            {
                VoucherId = voucherId;
                VoucherAddressCreationDtos = voucherAddressCreationDtos;
            }

            public List<VoucherAddressCreationDto> VoucherAddressCreationDtos { get; }
            public int VoucherId { get; }
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
                var voucher = await _context.Voucher.FindAsync(new object[] {request.VoucherId}, cancellationToken);

                if (voucher == null)
                {
                    _logger.LogInformation("Voucher doesn't exist");
                    return Result<Unit>.NotFound("Voucher doesn't exist.");
                }

                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.VoucherAddressCreationDtos.Count == 0)
                    {
                        _logger.LogInformation("Voucher address list must be provided");
                        return Result<Unit>.Failure("Voucher address list must be provided.");
                    }

                    List<VoucherAddress> newVoucherAddresses = new();

                    _mapper.Map(request.VoucherAddressCreationDtos, newVoucherAddresses);

                    foreach (var voucherAddress in newVoucherAddresses)
                    {
                        voucherAddress.VoucherId = voucher.VoucherId;
                    }

                    await _context.VoucherAddress.AddRangeAsync(newVoucherAddresses, cancellationToken);
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new voucher address");
                        return Result<Unit>.Failure("Failed to create new voucher address.");
                    }

                    _logger.LogInformation("Successfully created new voucher address");
                    return Result<Unit>.Success(Unit.Value, "Successfully created new voucher address.",
                        newVoucherAddresses.First().VoucherId.ToString());
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
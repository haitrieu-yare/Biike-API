using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public VoucherCreationDto VoucherCreationDto { get; init; } = null!;
        }

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

                    Voucher newVoucher = new();

                    _mapper.Map(request.VoucherCreationDto, newVoucher);

                    if (newVoucher.EndDate.CompareTo(newVoucher.StartDate) < 0)
                    {
                        _logger.LogInformation("EndDate must be set later than StartDate");
                        return Result<Unit>.Failure("EndDate must be set later than StartDate.");
                    }

                    newVoucher.Remaining = newVoucher.Quantity;

                    await _context.Voucher.AddAsync(newVoucher, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new voucher");
                        return Result<Unit>.Failure("Failed to create new voucher.");
                    }

                    _logger.LogInformation("Successfully created new voucher");
                    return Result<Unit>.Success(
                        Unit.Value, "Successfully created new voucher.", newVoucher.VoucherId.ToString());
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
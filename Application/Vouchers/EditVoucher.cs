using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
    public class EditVoucher
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int VoucherId { get; set; }
            public VoucherEditDto NewVoucher { get; set; } = null!;
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<EditVoucher> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<EditVoucher> logger)
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

                    var oldVoucher = await _context.Voucher
                        .FindAsync(new object[] {request.VoucherId}, cancellationToken);

                    if (oldVoucher == null) return null!;

                    _mapper.Map(request.NewVoucher, oldVoucher);

                    if (oldVoucher.EndDate.CompareTo(oldVoucher.StartDate) < 0)
                    {
                        _logger.LogInformation("EndDate must be set later than StartDate.");
                        return Result<Unit>.Failure("EndDate must be set later than StartDate.");
                    }

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation($"Failed to update voucher by voucherId {request.VoucherId}.");
                        return Result<Unit>.Failure($"Failed to update voucher by voucherId {request.VoucherId}.");
                    }

                    _logger.LogInformation($"Successfully updated voucher by voucherId {request.VoucherId}.");
                    return Result<Unit>.Success(
                        Unit.Value, $"Successfully updated voucher by voucherId {request.VoucherId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled.");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    _logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.VoucherCodes.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.VoucherCodes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherCodeEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int voucherCodeId, VoucherCodeDto voucherCodeDto)
            {
                VoucherCodeId = voucherCodeId;
                VoucherCodeDto = voucherCodeDto;
            }

            public int VoucherCodeId { get; }
            public VoucherCodeDto VoucherCodeDto { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly ILogger<Handler> _logger;

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

                    VoucherCode oldVoucherCode =
                        await _context.VoucherCode.FindAsync(new object[] {request.VoucherCodeId}, cancellationToken);

                    if (oldVoucherCode == null)
                    {
                        _logger.LogInformation("Voucher code doesn't exist");
                        return Result<Unit>.NotFound("Voucher code doesn't exist.");
                    }

                    _mapper.Map(request.VoucherCodeDto, oldVoucherCode);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update voucher code by voucherCodeId {VoucherCodeId}",
                            request.VoucherCodeId);
                        return Result<Unit>.Failure(
                            $"Failed to update voucher code by voucherCodeId {request.VoucherCodeId}.");
                    }

                    _logger.LogInformation("Successfully updated voucher code by voucherCodeId {VoucherCodeId}",
                        request.VoucherCodeId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully updated voucher code by voucherCodeId {request.VoucherCodeId}.");
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
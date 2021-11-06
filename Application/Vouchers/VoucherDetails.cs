using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherDetails
    {
        public class Query : IRequest<Result<VoucherDto>>
        {
            public int VoucherId { get; init; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<VoucherDto>>
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

            public async Task<Result<VoucherDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var voucher = await _context.Voucher
                        .Where(v => v.VoucherId == request.VoucherId)
                        .ProjectTo<VoucherDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (voucher == null)
                    {
                        _logger.LogInformation("Voucher doesn't exist");
                        return Result<VoucherDto>.NotFound("Voucher doesn't exist.");
                    }

                    _logger.LogInformation("Successfully retrieved voucher " + "by voucherId {request.VoucherId}",
                        request.VoucherId);
                    return Result<VoucherDto>.Success(voucher,
                        $"Successfully retrieved voucher by voucherId {request.VoucherId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<VoucherDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.VoucherCodes.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.VoucherCodes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherCodeDetails
    {
        public class Query : IRequest<Result<VoucherCodeDto>>
        {
            public Query(int voucherCodeId)
            {
                VoucherCodeId = voucherCodeId;
            }

            public int VoucherCodeId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<VoucherCodeDto>>
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

            public async Task<Result<VoucherCodeDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var voucherCode = await _context.VoucherCode
                        .Where(v => v.VoucherCodeId == request.VoucherCodeId)
                        .ProjectTo<VoucherCodeDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (voucherCode == null)
                    {
                        _logger.LogInformation("Voucher code doesn't exist");
                        return Result<VoucherCodeDto>.NotFound("Voucher code doesn't exist.");
                    }

                    _logger.LogInformation("Successfully retrieved voucher code by voucherId {VoucherCodeId}",
                        request.VoucherCodeId);
                    return Result<VoucherCodeDto>.Success(voucherCode,
                        $"Successfully retrieved voucher code by voucherId {request.VoucherCodeId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<VoucherCodeDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}
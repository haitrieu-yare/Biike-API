using System;
using System.Collections.Generic;
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
    public class VoucherCodeList
    {
        public class Query : IRequest<Result<List<VoucherCodeDto>>>
        {
            public Query(int page, int limit)
            {
                Page = page;
                Limit = limit;
            }

            public int Page { get; }
            public int Limit { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<List<VoucherCodeDto>>>
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

            public async Task<Result<List<VoucherCodeDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<VoucherCodeDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<VoucherCodeDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.VoucherCode.CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<VoucherCodeDto> voucherCodes = new();

                    if (request.Page <= lastPage)
                        voucherCodes = await _context.VoucherCode.OrderBy(v => v.VoucherCodeId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<VoucherCodeDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(request.Page, request.Limit, voucherCodes.Count, lastPage,
                        totalRecord);

                    _logger.LogInformation("Successfully retrieved list of all voucher codes");
                    return Result<List<VoucherCodeDto>>.Success(voucherCodes,
                        "Successfully retrieved list of all voucher codes.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<VoucherCodeDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}
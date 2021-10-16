using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.VoucherCategories.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.VoucherCategories
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherCategoryList
    {
        public class Query : IRequest<Result<List<VoucherCategoryDto>>>
        {
            public int Page { get; init; }
            public int Limit { get; init; }
        }

        public class Handler : IRequestHandler<Query, Result<List<VoucherCategoryDto>>>
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

            public async Task<Result<List<VoucherCategoryDto>>> Handle(Query request,
                CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<VoucherCategoryDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<VoucherCategoryDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.VoucherCategory.CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<VoucherCategoryDto> voucherCategories = new();

                    if (request.Page <= lastPage)
                        voucherCategories = await _context.VoucherCategory
                            .OrderBy(v => v.VoucherCategoryId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<VoucherCategoryDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, voucherCategories.Count, lastPage, totalRecord
                    );

                    _logger.LogInformation("Successfully retrieved list of all voucher categories");
                    return Result<List<VoucherCategoryDto>>.Success(
                        voucherCategories, "Successfully retrieved list of all voucher categories.", paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<VoucherCategoryDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}
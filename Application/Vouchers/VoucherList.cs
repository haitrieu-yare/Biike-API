using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherList
    {
        public class Query : IRequest<Result<List<VoucherDto>>>
        {
            public Query(int page, int limit, int categoryId, string? userCoordinate, bool isAdmin)
            {
                Page = page;
                Limit = limit;
                CategoryId = categoryId;
                UserCoordinate = userCoordinate;
                IsAdmin = isAdmin;
            }

            public int Page { get; }
            public int Limit { get; }
            public int CategoryId { get; }
            public string? UserCoordinate { get; }
            public bool IsAdmin { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<List<VoucherDto>>>
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

            public async Task<Result<List<VoucherDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<VoucherDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<VoucherDto>>.Failure("Limit must be larger than 0.");
                    }

                    if (request.CategoryId < 0)
                    {
                        _logger.LogInformation("CategoryId must be larger or equal 0");
                        return Result<List<VoucherDto>>.Failure("CategoryId must be larger or equal 0.");
                    }

                    var currentTime = CurrentTime.GetCurrentTime();

                    IQueryable<Voucher> voucherQueryable = _context.Voucher;

                    if (!request.IsAdmin)
                    {
                        voucherQueryable = voucherQueryable.Where(v => v.EndDate.CompareTo(currentTime) > 0);
                    }

                    if (request.CategoryId != 0)
                    {
                        voucherQueryable = voucherQueryable.Where(v => v.VoucherCategoryId == request.CategoryId);
                    }

                    var totalRecord = await voucherQueryable.CountAsync(cancellationToken);
                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);

                    List<VoucherDto> vouchers = new();

                    if (request.Page <= lastPage)
                    {
                        voucherQueryable = voucherQueryable.AsSingleQuery();
                        
                        if (!string.IsNullOrEmpty(request.UserCoordinate))
                        {
                            CultureInfo culture = new ("en-US");
                            var userCoordinate = request.UserCoordinate.Split(",");
                            double userLatitude = Convert.ToDouble(userCoordinate[0], culture);
                            double userLongitude = Convert.ToDouble(userCoordinate[1], culture);

                            var addresses = await _context.Address
                                .Include(a => a.VoucherAddresses)
                                .ToListAsync(cancellationToken);
                            
                            addresses.Sort(new ApplicationUtils.AddressComparer(userLatitude, userLongitude));
                        
                            HashSet<int> voucherSet = new();

                            foreach (var voucherAddress in addresses.SelectMany(address => address.VoucherAddresses))
                            {
                                voucherSet.Add(voucherAddress.VoucherId);
                            }

                            voucherQueryable = voucherQueryable
                                .Where(v => voucherSet.Contains(v.VoucherId));
                        }
                        else
                        {
                            voucherQueryable = voucherQueryable.OrderBy(v => v.VoucherCategoryId);
                        }
                        
                        vouchers = await voucherQueryable
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<VoucherDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);
                    }

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, vouchers.Count, lastPage, totalRecord);

                    _logger.LogInformation("Successfully retrieved list of all vouchers");
                    return Result<List<VoucherDto>>.Success(vouchers, "Successfully retrieved list of all vouchers.",
                        paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<VoucherDto>>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is FormatException)
                {
                    _logger.LogInformation("User coordinate is not a valid pair of latitude and longitude");
                    return Result<List<VoucherDto>>.Failure(
                        "User coordinate is not a valid pair of latitude and longitude.");
                }
                catch (Exception ex) when (ex is OverflowException)
                {
                    _logger.LogInformation("Latitude or longitude value is too big or too small");
                    return Result<List<VoucherDto>>.Failure("Latitude or longitude value is too big or too small.");
                }
                // catch (Exception ex)
                // {
                //     _logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
                //     return Result<List<VoucherDto>>.Failure(ex.InnerException?.Message ?? ex.Message);
                // }
            }
        }
    }
}
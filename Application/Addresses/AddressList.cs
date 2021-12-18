using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Addresses.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Addresses
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AddressList
    {
        public class Query : IRequest<Result<List<AddressDto>>>
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
        public class Handler : IRequestHandler<Query, Result<List<AddressDto>>>
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

            public async Task<Result<List<AddressDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (request.Page <= 0)
                    {
                        _logger.LogInformation("Page must be larger than 0");
                        return Result<List<AddressDto>>.Failure("Page must be larger than 0.");
                    }

                    if (request.Limit <= 0)
                    {
                        _logger.LogInformation("Limit must be larger than 0");
                        return Result<List<AddressDto>>.Failure("Limit must be larger than 0.");
                    }

                    var totalRecord = await _context.Address.CountAsync(cancellationToken);

                    var lastPage = ApplicationUtils.CalculateLastPage(totalRecord, request.Limit);
                    List<AddressDto> addresses = new();

                    if (request.Page <= lastPage)
                    {
                        addresses = await _context.Address.OrderBy(s => s.AddressId)
                            .Skip((request.Page - 1) * request.Limit)
                            .Take(request.Limit)
                            .ProjectTo<AddressDto>(_mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);
                    }

                    PaginationDto paginationDto = new(
                        request.Page, request.Limit, addresses.Count, lastPage, totalRecord);

                    _logger.LogInformation("Successfully retrieved list of all addresses");
                    return Result<List<AddressDto>>.Success(addresses, "Successfully retrieved list of all addresses.",
                        paginationDto);
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<List<AddressDto>>.Failure("Request was cancelled.");
                }
            }
        }
    }
}
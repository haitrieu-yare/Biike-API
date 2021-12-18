using System;
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
    public class AddressDetails
    {
        public class Query : IRequest<Result<AddressDto>>
        {
            public Query(int addressId)
            {
                AddressId = addressId;
            }
            public int AddressId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<AddressDto>>
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

            public async Task<Result<AddressDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var address = await _context.Address
                        .Where(a => a.AddressId == request.AddressId)
                        .ProjectTo<AddressDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(cancellationToken);
                    
                    if (address == null)
                    {
                        _logger.LogInformation("Address doesn't exist");
                        return Result<AddressDto>.NotFound("Address doesn't exist.");
                    }

                    _logger.LogInformation("Successfully retrieved list of all address");
                    return Result<AddressDto>.Success(address, "Successfully retrieved list of all address.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<AddressDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}
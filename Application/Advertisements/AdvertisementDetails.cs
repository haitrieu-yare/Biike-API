using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Advertisements.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Advertisements
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AdvertisementDetails
    {
        public class Query : IRequest<Result<AdvertisementDto>>
        {
            public Query(int advertisementId, bool isAdmin)
            {
                AdvertisementId = advertisementId;
                IsAdmin = isAdmin;
            }

            public int AdvertisementId { get; }
            public bool IsAdmin { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Query, Result<AdvertisementDto>>
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

            public async Task<Result<AdvertisementDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var query = _context.Advertisement.AsQueryable();

                    if (!request.IsAdmin)
                    {
                        query = query.Where(a => a.IsActive == true);
                    }

                    var advertisement = await query.AsSingleQuery()
                        .Where(v => v.AdvertisementId == request.AdvertisementId)
                        .ProjectTo<AdvertisementDto>(_mapper.ConfigurationProvider)
                        .SingleOrDefaultAsync(cancellationToken);

                    if (advertisement == null)
                    {
                        _logger.LogInformation("Advertisement doesn't exist");
                        return Result<AdvertisementDto>.NotFound("Advertisement doesn't exist.");
                    }

                    _logger.LogInformation(
                        "Successfully retrieved advertisement by advertisementId {request.AdvertisementId}",
                        request.AdvertisementId);
                    return Result<AdvertisementDto>.Success(advertisement,
                        $"Successfully retrieved advertisement by advertisementId {request.AdvertisementId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled");
                    return Result<AdvertisementDto>.Failure("Request was cancelled.");
                }
            }
        }
    }
}
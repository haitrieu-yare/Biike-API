using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Addresses.DTOs;
using Application.Core;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Addresses
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AddressEdit
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(AddressDto addressDto)
            {
                AddressDto = addressDto;
            }

            public AddressDto AddressDto { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
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

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var address = await _context.Address.FindAsync(new object[] {request.AddressDto.AddressId!},
                        cancellationToken);

                    if (address == null)
                    {
                        _logger.LogInformation("Address doesn't exist");
                        return Result<Unit>.NotFound("Address doesn't exist.");
                    }

                    _mapper.Map(request.AddressDto, address);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update address with AddressId {AddressId}",
                            address.AddressId);
                        return Result<Unit>.Failure($"Failed to update address with AddressId {address.AddressId}.");
                    }

                    _logger.LogInformation("Successfully update address with AddressId {AddressId}", address.AddressId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully update address with AddressId {address.AddressId}.");
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
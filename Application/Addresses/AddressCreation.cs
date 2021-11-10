using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Addresses.DTOs;
using Application.Core;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Addresses
{
    public class AddressCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(AddressCreationDto addressCreationDto)
            {
                AddressCreationDto = addressCreationDto;
            }

            public AddressCreationDto AddressCreationDto { get; }
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

                    Address address = new();

                    _mapper.Map(request.AddressCreationDto, address);

                    await _context.Address.AddAsync(address, cancellationToken);
                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new address");
                        return Result<Unit>.Failure("Failed to create new address.");
                    }

                    _logger.LogInformation("Successfully created new address");
                    return Result<Unit>.Success(Unit.Value, "Successfully created new address.",
                        address.AddressId.ToString());
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
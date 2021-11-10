using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Addresses
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class AddressDeletion
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Command(int addressId)
            {
                AddressId = addressId;
            }

            public int AddressId { get; }
        }

        // ReSharper disable once UnusedType.Global
        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<Handler> _logger;

            public Handler(DataContext context, ILogger<Handler> logger)
            {
                _context = context;
                _logger = logger;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var address = await _context.Address.FindAsync(new object[] {request.AddressId}, cancellationToken);

                    if (address == null)
                    {
                        _logger.LogInformation("Address doesn't exist");
                        return Result<Unit>.NotFound("Address doesn't exist.");
                    }

                    var voucherAddresses = await _context.VoucherAddress.Where(v => v.AddressId == request.AddressId)
                        .ToListAsync(cancellationToken);

                    var advertisingAddresses = await _context.AdvertisingAddress
                        .Where(v => v.AddressId == request.AddressId)
                        .ToListAsync(cancellationToken);

                    _context.AdvertisingAddress.RemoveRange(advertisingAddresses);
                    _context.VoucherAddress.RemoveRange(voucherAddresses);
                    _context.Address.Remove(address);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to delete address with AddressId {AddressId}",
                            address.AddressId);
                        return Result<Unit>.Failure($"Failed to delete address with AddressId {address.AddressId}.");
                    }

                    _logger.LogInformation("Successfully deleted address with AddressId {AddressId}",
                        address.AddressId);
                    return Result<Unit>.Success(Unit.Value,
                        $"Successfully deleted address with AddressId {address.AddressId}.");
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
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.VoucherCategories.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.VoucherCategories
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class VoucherCategoryCreation
    {
        public class Command : IRequest<Result<Unit>>
        {
            public VoucherCategoryCreationDto VoucherCategoryCreationDto { get; init; } = null!;
        }

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

                    VoucherCategory newVoucherCategory = new();

                    _mapper.Map(request.VoucherCategoryCreationDto, newVoucherCategory);

                    await _context.VoucherCategory.AddAsync(newVoucherCategory, cancellationToken);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to create new voucher's category");
                        return Result<Unit>.Failure("Failed to create new voucher's category.");
                    }

                    _logger.LogInformation("Successfully created voucher's category");
                    return Result<Unit>.Success(
                        Unit.Value, "Successfully created voucher's category.",
                        newVoucherCategory.VoucherCategoryId.ToString());
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
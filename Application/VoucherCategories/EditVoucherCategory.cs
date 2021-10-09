using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.VoucherCategories.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.VoucherCategories
{
    public class EditVoucherCategory
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int VoucherCategoryId { get; set; }
            public VoucherCategoryDto NewVoucherCategoryDto { get; set; } = null!;
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly ILogger<EditVoucherCategory> _logger;
            private readonly IMapper _mapper;

            public Handler(DataContext context, IMapper mapper, ILogger<EditVoucherCategory> logger)
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

                    var oldVoucherCategory = await _context.VoucherCategory
                        .FindAsync(new object[] {request.VoucherCategoryId}, cancellationToken);

                    if (oldVoucherCategory == null) return null!;

                    _mapper.Map(request.NewVoucherCategoryDto, oldVoucherCategory);

                    var result = await _context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        _logger.LogInformation("Failed to update voucher's category " +
                                               $"by VoucherCategoryId {request.VoucherCategoryId}.");
                        return Result<Unit>.Failure("Failed to update voucher's category " +
                                                    $"by VoucherCategoryId {request.VoucherCategoryId}.");
                    }

                    _logger.LogInformation("Successfully updated voucher's category " +
                                           $"by VoucherCategoryId {request.VoucherCategoryId}.");
                    return Result<Unit>.Success(Unit.Value, "Successfully updated voucher's category " +
                                                            $"by VoucherCategoryId {request.VoucherCategoryId}.");
                }
                catch (Exception ex) when (ex is TaskCanceledException)
                {
                    _logger.LogInformation("Request was cancelled.");
                    return Result<Unit>.Failure("Request was cancelled.");
                }
                catch (Exception ex) when (ex is DbUpdateException)
                {
                    _logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
                    return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
                }
            }
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.VoucherCategories.DTOs;
using AutoMapper;
using Domain.Entities;
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
			public int VoucherCategoryId { get; init; }
			public VoucherCategoryDto NewVoucherCategoryDto { get; init; } = null!;
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

					VoucherCategory oldVoucherCategory =
						await _context.VoucherCategory.FindAsync(new object[] { request.VoucherCategoryId },
							cancellationToken);

					if (oldVoucherCategory == null)
					{
						_logger.LogInformation("Voucher category doesn't exist");
						return Result<Unit>.NotFound("Voucher category doesn't exist.");
					}

					_mapper.Map(request.NewVoucherCategoryDto, oldVoucherCategory);

					bool result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation(
							"Failed to update voucher's category " + "by VoucherCategoryId {request.VoucherCategoryId}",
							request.VoucherCategoryId);
						return Result<Unit>.Failure("Failed to update voucher's category " +
						                            $"by VoucherCategoryId {request.VoucherCategoryId}.");
					}

					_logger.LogInformation(
						"Successfully updated voucher's category " + "by VoucherCategoryId {request.VoucherCategoryId}",
						request.VoucherCategoryId);
					return Result<Unit>.Success(Unit.Value,
						"Successfully updated voucher's category " +
						$"by VoucherCategoryId {request.VoucherCategoryId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation("{Error}", ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}
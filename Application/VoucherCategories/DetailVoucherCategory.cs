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
	public class DetailVoucherCategory
	{
		public class Query : IRequest<Result<VoucherCategoryDto>>
		{
			public int VoucherCategoryId { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<VoucherCategoryDto>>
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

			public async Task<Result<VoucherCategoryDto>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					VoucherCategory voucherCategoryDb = await _context.VoucherCategory
						.FindAsync(new object[] { request.VoucherCategoryId }, cancellationToken);

					if (voucherCategoryDb == null)
					{
						_logger.LogInformation("Voucher category doesn't exist");
						return Result<VoucherCategoryDto>.NotFound("Voucher category doesn't exist.");
					}

					VoucherCategoryDto voucherCategory = new();

					_mapper.Map(voucherCategoryDb, voucherCategory);

					_logger.LogInformation("Successfully retrieved voucherCategory " +
					                       "by voucherCategoryId {request.VoucherCategoryId}",
						request.VoucherCategoryId);
					return Result<VoucherCategoryDto>.Success(voucherCategory,
						$"Successfully retrieved voucherCategory by voucherCategoryId {request.VoucherCategoryId}.");
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<VoucherCategoryDto>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.VoucherCategories.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.VoucherCategories
{
	public class DetailVoucherCategory
	{
		public class Query : IRequest<Result<VoucherCategoryDTO>>
		{
			public int VoucherCategoryId { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<VoucherCategoryDTO>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<DetailVoucherCategory> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<DetailVoucherCategory> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<VoucherCategoryDTO>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					VoucherCategoryDTO voucherCategory = new VoucherCategoryDTO();

					VoucherCategory voucherCategoryDB = await _context.VoucherCategory
						.FindAsync(new object[] { request.VoucherCategoryId }, cancellationToken);

					_mapper.Map(voucherCategoryDB, voucherCategory);

					_logger.LogInformation("Successfully retrieved voucherCategory " +
						$"by voucherCategoryId {request.VoucherCategoryId}.");
					return Result<VoucherCategoryDTO>.Success(voucherCategory,
						$"Successfully retrieved voucherCategory by voucherCategoryId {request.VoucherCategoryId}.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<VoucherCategoryDTO>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
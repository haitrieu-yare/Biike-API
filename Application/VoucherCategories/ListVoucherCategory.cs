using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.VoucherCategories.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.VoucherCategories
{
	public class ListVoucherCategory
	{
		public class Query : IRequest<Result<List<VoucherCategoryDto>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<VoucherCategoryDto>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Handler> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Handler> logger)
			{
				_context = context;
				_mapper = mapper;
				_logger = logger;
			}

			public async Task<Result<List<VoucherCategoryDto>>> Handle(Query request,
				CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<VoucherCategoryDto>>.Failure("Page must larger than 0.");
					}
					else if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must larger than 0");
						return Result<List<VoucherCategoryDto>>.Failure("Limit must larger than 0.");
					}

					int totalRecord = await _context.VoucherCategory.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<VoucherCategoryDto> voucherCategories = new List<VoucherCategoryDto>();

					if (request.Page <= lastPage)
					{
						voucherCategories = await _context.VoucherCategory
							.OrderBy(v => v.VoucherCategoryId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<VoucherCategoryDto>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDto paginationDto = new PaginationDto(
						request.Page, request.Limit, voucherCategories.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all voucher categories");
					return Result<List<VoucherCategoryDto>>.Success(
						voucherCategories, "Successfully retrieved list of all voucher categories.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<VoucherCategoryDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
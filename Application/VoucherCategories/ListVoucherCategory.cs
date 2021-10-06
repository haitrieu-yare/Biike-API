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
		public class Query : IRequest<Result<List<VoucherCategoryDTO>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<VoucherCategoryDTO>>>
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

			public async Task<Result<List<VoucherCategoryDTO>>> Handle(Query request,
				CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<VoucherCategoryDTO>>.Failure("Page must larger than 0.");
					}
					else if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must larger than 0");
						return Result<List<VoucherCategoryDTO>>.Failure("Limit must larger than 0.");
					}

					int totalRecord = await _context.VoucherCategory.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<VoucherCategoryDTO> voucherCategories = new List<VoucherCategoryDTO>();

					if (request.Page <= lastPage)
					{
						voucherCategories = await _context.VoucherCategory
							.OrderBy(v => v.VoucherCategoryId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<VoucherCategoryDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, voucherCategories.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all voucher categories");
					return Result<List<VoucherCategoryDTO>>.Success(
						voucherCategories, "Successfully retrieved list of all voucher categories.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<VoucherCategoryDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
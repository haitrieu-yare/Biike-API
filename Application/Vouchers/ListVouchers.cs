using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Persistence;

namespace Application.Vouchers
{
	public class ListVouchers
	{
		public class Query : IRequest<Result<List<VoucherDTO>>>
		{
			public int Page { get; set; }
			public int Limit { get; set; }
		}

		public class Handler : IRequestHandler<Query, Result<List<VoucherDTO>>>
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

			public async Task<Result<List<VoucherDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must larger than 0");
						return Result<List<VoucherDTO>>.Failure("Page must larger than 0.");
					}
					else if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must larger than 0");
						return Result<List<VoucherDTO>>.Failure("Limit must larger than 0.");
					}

					int totalRecord = await _context.Voucher.CountAsync(cancellationToken);

					#region Calculate last page
					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);
					#endregion

					List<VoucherDTO> vouchers = new List<VoucherDTO>();

					if (request.Page <= lastPage)
					{
						vouchers = await _context.Voucher
							.OrderBy(v => v.VoucherId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<VoucherDTO>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);
					}

					PaginationDTO paginationDto = new PaginationDTO(
						request.Page, request.Limit, vouchers.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all vouchers");
					return Result<List<VoucherDTO>>.Success(
						vouchers, "Successfully retrieved list of all vouchers.", paginationDto);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<VoucherDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
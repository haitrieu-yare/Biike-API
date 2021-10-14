using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
	public class ListVouchers
	{
		public class Query : IRequest<Result<List<VoucherDto>>>
		{
			public int Page { get; init; }
			public int Limit { get; init; }
		}

		public class Handler : IRequestHandler<Query, Result<List<VoucherDto>>>
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

			public async Task<Result<List<VoucherDto>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					if (request.Page <= 0)
					{
						_logger.LogInformation("Page must be larger than 0");
						return Result<List<VoucherDto>>.Failure("Page must be larger than 0.");
					}

					if (request.Limit <= 0)
					{
						_logger.LogInformation("Limit must be larger than 0");
						return Result<List<VoucherDto>>.Failure("Limit must be larger than 0.");
					}

					int totalRecord = await _context.Voucher.CountAsync(cancellationToken);

					int lastPage = Utils.CalculateLastPage(totalRecord, request.Limit);

					List<VoucherDto> vouchers = new();

					if (request.Page <= lastPage)
						vouchers = await _context.Voucher
							.OrderBy(v => v.VoucherId)
							.Skip((request.Page - 1) * request.Limit)
							.Take(request.Limit)
							.ProjectTo<VoucherDto>(_mapper.ConfigurationProvider)
							.ToListAsync(cancellationToken);

					PaginationDto paginationDto = new(
						request.Page, request.Limit, vouchers.Count, lastPage, totalRecord
					);

					_logger.LogInformation("Successfully retrieved list of all vouchers");
					return Result<List<VoucherDto>>.Success(
						vouchers, "Successfully retrieved list of all vouchers.", paginationDto);
				}
				catch (Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<VoucherDto>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
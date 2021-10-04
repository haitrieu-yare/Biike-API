using System.Collections.Generic;
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
		public class Query : IRequest<Result<List<VoucherDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<VoucherDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<ListVouchers> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<ListVouchers> logger)
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

					var vouchers = await _context.Voucher
						.ProjectTo<VoucherDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all vouchers.");
					return Result<List<VoucherDTO>>.Success(vouchers, "Successfully retrieved list of all vouchers.");
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<List<VoucherDTO>>.Failure("Request was cancelled.");
				}
			}
		}
	}
}
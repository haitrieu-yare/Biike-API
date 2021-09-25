using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.VoucherCategories
{
	public class List
	{
		public class Query : IRequest<Result<List<VoucherCategoryDTO>>> { }

		public class Handler : IRequestHandler<Query, Result<List<VoucherCategoryDTO>>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<List> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<List> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<List<VoucherCategoryDTO>>> Handle(Query request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var voucherCategories = await _context.VoucherCategory
						.ProjectTo<VoucherCategoryDTO>(_mapper.ConfigurationProvider)
						.ToListAsync(cancellationToken);

					_logger.LogInformation("Successfully retrieved list of all voucher categories");
					return Result<List<VoucherCategoryDTO>>.Success(voucherCategories);
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<List<VoucherCategoryDTO>>.Failure("Request was cancelled");
				}
			}
		}
	}
}
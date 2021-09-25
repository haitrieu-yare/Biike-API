using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.VoucherCategories
{
	public class Create
	{
		public class Command : IRequest<Result<Unit>>
		{
			public VoucherCategoryDTO VoucherCategoryDTO { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Create> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Create> logger)
			{
				_logger = logger;
				_mapper = mapper;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var newVoucherCategory = new VoucherCategory();
					_mapper.Map(request.VoucherCategoryDTO, newVoucherCategory);

					await _context.VoucherCategory.AddAsync(newVoucherCategory, cancellationToken);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new voucher category");
						return Result<Unit>.Failure("Failed to create new voucher category");
					}
					else
					{
						_logger.LogInformation("Successfully created voucher category");
						return Result<Unit>.Success(Unit.Value);
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled");
					return Result<Unit>.Failure("Request was cancelled");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.Message);
					return Result<Unit>.Failure(ex.Message);
				}
			}
		}
	}
}
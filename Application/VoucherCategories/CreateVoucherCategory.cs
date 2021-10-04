using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.VoucherCategories.DTOs;
using AutoMapper;
using Domain.Entities;
using MediatR;
using Persistence;

namespace Application.VoucherCategories
{
	public class CreateVoucherCategory
	{
		public class Command : IRequest<Result<Unit>>
		{
			public VoucherCategoryCreateDTO VoucherCategoryCreateDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<CreateVoucherCategory> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<CreateVoucherCategory> logger)
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

					VoucherCategory newVoucherCategory = new VoucherCategory();

					_mapper.Map(request.VoucherCategoryCreateDTO, newVoucherCategory);

					await _context.VoucherCategory.AddAsync(newVoucherCategory, cancellationToken);

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to create new voucher's category.");
						return Result<Unit>.Failure("Failed to create new voucher's category.");
					}
					else
					{
						_logger.LogInformation("Successfully created voucher's category.");
						return Result<Unit>.Success(Unit.Value, "Successfully created voucher's category.");
					}
				}
				catch (System.Exception ex) when (ex is TaskCanceledException)
				{
					_logger.LogInformation("Request was cancelled.");
					return Result<Unit>.Failure("Request was cancelled.");
				}
				catch (System.Exception ex) when (ex is DbUpdateException)
				{
					_logger.LogInformation(ex.InnerException?.Message ?? ex.Message);
					return Result<Unit>.Failure(ex.InnerException?.Message ?? ex.Message);
				}
			}
		}
	}
}
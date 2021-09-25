using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using Application.Vouchers.DTOs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Vouchers
{
	public class Edit
	{
		public class Command : IRequest<Result<Unit>>
		{
			public int Id { get; set; }
			public VoucherEditDTO NewVoucher { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly IMapper _mapper;
			private readonly ILogger<Edit> _logger;
			public Handler(DataContext context, IMapper mapper, ILogger<Edit> logger)
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

					var oldVoucher = await _context.Voucher
						.FindAsync(new object[] { request.Id }, cancellationToken);
					if (oldVoucher == null) return null;

					_mapper.Map(request.NewVoucher, oldVoucher);
					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update voucher");
						return Result<Unit>.Failure("Failed to update voucher");
					}
					else
					{
						_logger.LogInformation("Successfully updated voucher");
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
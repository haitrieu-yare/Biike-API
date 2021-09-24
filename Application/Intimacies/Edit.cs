using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.Intimacies
{
	public class Edit
	{
		public class Command : IRequest<Result<Unit>>
		{
			public IntimacyEditDTO IntimacyEditDTO { get; set; }
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<Edit> _logger;
			public Handler(DataContext context, ILogger<Edit> logger)
			{
				_logger = logger;
				_context = context;
			}

			public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
			{
				try
				{
					cancellationToken.ThrowIfCancellationRequested();

					var oldIntimacy = await _context.Intimacy
						.FindAsync(new object[]
						{
							request.IntimacyEditDTO.UserOneId,
							request.IntimacyEditDTO.UserTwoId
						}, cancellationToken);
					if (oldIntimacy == null) return null;

					if (request.IntimacyEditDTO.BlockAction && oldIntimacy.IsBlock)
					{
						_logger.LogInformation("User one has already blocked user two");
						return Result<Unit>.Failure("User one has already blocked user two");
					}
					else if (request.IntimacyEditDTO.BlockAction && !oldIntimacy.IsBlock)
					{
						oldIntimacy.IsBlock = true;
						oldIntimacy.BlockTime = DateTime.Now;
					}
					else if (!request.IntimacyEditDTO.BlockAction && oldIntimacy.IsBlock)
					{
						oldIntimacy.IsBlock = false;
						oldIntimacy.UnblockTime = DateTime.Now;
					}
					else if (!request.IntimacyEditDTO.BlockAction && !oldIntimacy.IsBlock)
					{
						_logger.LogInformation("User one hasn't blocked user two");
						return Result<Unit>.Failure("User one hasn't blocked user two");
					}

					var result = await _context.SaveChangesAsync(cancellationToken) > 0;

					if (!result)
					{
						_logger.LogInformation("Failed to update route");
						return Result<Unit>.Failure("Failed to update route");
					}
					else
					{
						_logger.LogInformation("Successfully updated route");
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
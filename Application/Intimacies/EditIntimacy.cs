using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Core;
using Application.Intimacies.DTOs;
using Domain;
using MediatR;
using Persistence;

namespace Application.Intimacies
{
	public class EditIntimacy
	{
		public class Command : IRequest<Result<Unit>>
		{
			public IntimacyCreateEditDTO IntimacyCreateEditDTO { get; set; } = null!;
		}

		public class Handler : IRequestHandler<Command, Result<Unit>>
		{
			private readonly DataContext _context;
			private readonly ILogger<EditIntimacy> _logger;
			public Handler(DataContext context, ILogger<EditIntimacy> logger)
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
							request.IntimacyCreateEditDTO.UserOneId!,
							request.IntimacyCreateEditDTO.UserTwoId!
						}, cancellationToken);
					if (oldIntimacy == null) return null!;

					if (oldIntimacy.IsBlock)
					{
						oldIntimacy.IsBlock = !oldIntimacy.IsBlock;
						oldIntimacy.UnblockTime = CurrentTime.GetCurrentTime();
					}
					else if (!oldIntimacy.IsBlock)
					{
						oldIntimacy.IsBlock = !oldIntimacy.IsBlock;
						oldIntimacy.BlockTime = CurrentTime.GetCurrentTime();
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
						return Result<Unit>.Success(Unit.Value, "Successfully updated route");
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
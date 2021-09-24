using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Core;
using AutoMapper;
using Domain;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;

namespace Application.TripTransactions
{
	public class AutoCreate
	{
		private readonly DataContext _context;
		private readonly IMapper _mapper;
		private readonly ILogger<AutoCreate> _logger;
		public AutoCreate(DataContext context, IMapper mapper, ILogger<AutoCreate> logger)
		{
			_logger = logger;
			_mapper = mapper;
			_context = context;
		}

		public async Task<Result<Unit>> Run(Trip trip, CancellationToken cancellationToken)
		{
			try
			{
				cancellationToken.ThrowIfCancellationRequested();

				var tripTransaction = new TripTransaction
				{
					TripId = trip.Id,
					TransactionDate = DateTime.Now,
				};

				var wallet = await _context.Wallet
					.Where(w => w.AppUserId == trip.BikerId)
					.Where(w => w.Status == (int)WalletStatus.Current)
					.SingleOrDefaultAsync(cancellationToken);
				tripTransaction.WalletId = wallet.Id;

				//TODO: Xử lý cách tính điểm dựa trên feedback và số Km đi được
				int newPoint = 10;

				tripTransaction.AmountOfPoint = newPoint;
				wallet.Point += newPoint;

				var user = await _context.AppUser
					.Where(u => u.Id == wallet.AppUserId)
					.SingleOrDefaultAsync(cancellationToken);
				user.TotalPoint += newPoint;

				await _context.TripTransaction.AddAsync(tripTransaction, cancellationToken);
				var result = await _context.SaveChangesAsync(cancellationToken) > 0;

				if (!result)
				{
					_logger.LogInformation("Failed to create new trip transaction");
					return Result<Unit>.Failure("Failed to create new trip transaction");
				}
				else
				{
					_logger.LogInformation("Successfully created trip transaction");
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence;
using Quartz;

namespace Application.Wallets
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WalletJob : IJob
    {
        private readonly DataContext _context;
        private readonly ILogger<WalletJob> _logger;

        public WalletJob(DataContext context, ILogger<WalletJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("[Start] Wallet Job at time: {Time}",
                CurrentTime.GetCurrentTime().ToString("MM/dd/yyyy hh:mm:ss.fff"));

            List<Wallet> wallets = await _context.Wallet
                .Include(w => w.User)
                .Where(w => w.Status != (int) WalletStatus.Expired)
                .ToListAsync();

            wallets.ForEach(w =>
            {
                if (w.Status == (int) WalletStatus.Old) w.Status = (int) WalletStatus.Expired;
            });

            wallets.ForEach(w =>
            {
                if (w.Status == (int) WalletStatus.Current) w.Status = (int) WalletStatus.Old;
            });

            List<Wallet> newWallets = new();

            wallets.ForEach(w =>
            {
                if (w.Status != (int) WalletStatus.Old) return;

                var currentTime = CurrentTime.GetCurrentTime();
                var toDate = currentTime;
                var fromDate = toDate;

                switch (currentTime.Month)
                {
                    case >= 1 and <= 4:
                        fromDate = DateTime.Parse($"{currentTime.Year}/01/01 00:00:00");
                        toDate = DateTime.Parse($"{currentTime.Year}/08/31 23:59:59.9999999");
                        break;
                    case >= 5 and <= 8:
                        fromDate = DateTime.Parse($"{currentTime.Year}/05/01 00:00:00");
                        toDate = DateTime.Parse($"{currentTime.Year}/12/31 23:59:59.9999999");
                        break;
                    case >= 9 and <= 12:
                        fromDate = DateTime.Parse($"{currentTime.Year}/09/01 00:00:00");
                        toDate = DateTime.Parse($"{currentTime.Year + 1}/04/30 23:59:59.9999999");
                        break;
                }

                newWallets.Add(new Wallet {User = w.User, FromDate = fromDate, ToDate = toDate});
            });

            await _context.Wallet.AddRangeAsync(newWallets);

            var result = await _context.SaveChangesAsync() > 0;

            if (result)
                _logger.LogInformation("Wallet Job run successfully");
            else
                _logger.LogInformation("Wallet Job run failed");

            _logger.LogInformation("[End] Wallet Job at time: {Time}",
                CurrentTime.GetCurrentTime().ToString("MM/dd/yyyy hh:mm:ss.fff"));
        }
    }
}
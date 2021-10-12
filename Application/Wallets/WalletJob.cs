using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Application.Wallets
{
	public class WalletJob : IJob
	{
		private readonly ILogger<WalletJob> _logger;

		public WalletJob(ILogger<WalletJob> logger)
		{
			_logger = logger;
		}

		public Task Execute(IJobExecutionContext context)
		{
			_logger.LogInformation("Hello Cronjob ILogger. Time: {Time}", DateTime.UtcNow.AddHours(7));
			return Task.CompletedTask;
		}
	}
}
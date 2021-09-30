using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Application;
using Persistence;

namespace API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var host = CreateHostBuilder(args).Build();

			using var scope = host.Services.CreateScope();

			var services = scope.ServiceProvider;

			try
			{
				var context = services.GetRequiredService<DataContext>();
				await context.Database.MigrateAsync();
				bool firstTimeInsertingUsers = await Seed.SeedAllData(context) > 0;

				if (firstTimeInsertingUsers)
				{
					var loggerHashing = services.GetRequiredService<ILogger<Hashing>>();
					await new Hashing(context, loggerHashing).CreatePasswordForUsers();

					var loggerFirebase = services.GetRequiredService<ILogger<Firebase>>();
					await new Firebase(context, loggerFirebase).ImportUserFromDatabaseToFirebase();
				}
			}
			catch (System.Exception ex)
			{
				var logger = services.GetRequiredService<ILogger<Program>>();
				logger.LogError(ex, "An error occured during migration");
			}

			await host.RunAsync();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				});
	}
}

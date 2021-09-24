using System.Threading.Tasks;
using Persistence.Data;

namespace Persistence
{
	public class Seed
	{
		public static async Task SeedAllData(DataContext context)
		{
			await AppUserSeed.SeedData(context);
			await WalletSeed.SeedData(context);
			await BikeSeed.SeedData(context);
			await IntimacySeed.SeedData(context);

			await AreaSeed.SeedData(context);
			await StationSeed.SeedData(context);
			await RouteSeed.SeedData(context);

			await TripSeed.SeedData(context);
			await FeedbackSeed.SeedData(context);
			await TripTransactionSeed.SeedData(context);

			await VoucherCategorySeed.SeedData(context);
			await VoucherSeed.SeedData(context);
			await RedemptionSeed.SeedData(context);
		}
	}
}
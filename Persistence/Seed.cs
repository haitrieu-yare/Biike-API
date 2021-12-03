using System.Threading.Tasks;
using Persistence.Data;

namespace Persistence
{
    public static class Seed
    {
        public static async Task<int> SeedAllData(DataContext context)
        {
            await RoleSeed.SeedData(context);
            
            var result = await UserSeed.SeedData(context);
            await WalletSeed.SeedData(context);
            await BikeSeed.SeedData(context);
            await IntimacySeed.SeedData(context);

            await AreaSeed.SeedData(context);
            await StationSeed.SeedData(context);
            await RouteSeed.SeedData(context);

            // await TripSeed.SeedData(context);
            // await FeedbackSeed.SeedData(context);
            // await TripTransactionSeed.SeedData(context);

            await VoucherCategorySeed.SeedData(context);
            await VoucherSeed.SeedData(context);
            // await RedemptionSeed.SeedData(context);

            await ConfigurationSeed.SeedData(context);

            return result;
        }
    }
}
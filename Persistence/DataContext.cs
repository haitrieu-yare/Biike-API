using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions options) : base(options)
		{
		}
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			#region Station
			modelBuilder.Entity<Station>()
				.HasOne(s => s.Area)
				.WithMany(a => a.Stations)
				.HasForeignKey(s => s.AreaId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region Route
			modelBuilder.Entity<Route>()
				.HasIndex(r => new { r.DepartureId, r.DestinationId })
				.IsUnique();

			modelBuilder.Entity<Route>()
				.HasOne(r => r.Departure)
				.WithMany(s => s.DepartureRoutes)
				.HasForeignKey(r => r.DepartureId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Route>()
				.HasOne(r => r.Destination)
				.WithMany(s => s.DestinationRoutes)
				.HasForeignKey(r => r.DestinationId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region Trip
			modelBuilder.Entity<Trip>()
				.HasOne(t => t.Route)
				.WithMany(r => r.Trips)
				.HasForeignKey(t => t.RouteId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Trip>()
				.HasOne(t => t.Keer)
				.WithMany(u => u.KeerTrips)
				.HasForeignKey(t => t.KeerId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Trip>()
				.HasOne(t => t.Biker)
				.WithMany(u => u.BikerTrips)
				.HasForeignKey(t => t.BikerId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region Bike
			modelBuilder.Entity<Bike>()
				.HasOne(b => b.AppUser)
				.WithMany(u => u.Bikes)
				.HasForeignKey(b => b.AppUserId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region Intimacy
			modelBuilder.Entity<Intimacy>().HasKey(i => new { i.UserOneId, i.UserTwoId });

			modelBuilder.Entity<Intimacy>()
				.HasOne(i => i.UserTwo)
				.WithMany(u => u.UserTwoIntimacies)
				.HasForeignKey(i => i.UserTwoId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Intimacy>()
				.HasOne(i => i.UserOne)
				.WithMany(u => u.UserOneIntimacies)
				.HasForeignKey(i => i.UserOneId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region Feedback
			modelBuilder.Entity<Feedback>().HasKey(i => new { i.AppUserId, i.TripId });

			modelBuilder.Entity<Feedback>()
				.HasOne(f => f.Trip)
				.WithMany(t => t.FeedbackList)
				.HasForeignKey(f => f.TripId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Feedback>()
				.HasOne(f => f.AppUser)
				.WithMany(u => u.FeedBackList)
				.HasForeignKey(f => f.AppUserId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region TripTransaction
			modelBuilder.Entity<TripTransaction>()
				.HasOne(ts => ts.Trip)
				.WithMany(t => t.TripTransactions)
				.HasForeignKey(ts => ts.TripId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<TripTransaction>()
				.HasOne(ts => ts.Wallet)
				.WithMany(w => w.TripTransactions)
				.HasForeignKey(ts => ts.WalletId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region Wallet
			modelBuilder.Entity<Wallet>()
				.HasOne(w => w.AppUser)
				.WithMany(u => u.Wallets)
				.HasForeignKey(w => w.AppUserId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region Voucher
			modelBuilder.Entity<Voucher>()
				.HasOne(v => v.VoucherCategory)
				.WithMany(vc => vc.Vouchers)
				.HasForeignKey(v => v.VoucherCategoryId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region Redemption
			modelBuilder.Entity<Redemption>()
				.HasOne(r => r.Voucher)
				.WithMany(v => v.Redemptions)
				.HasForeignKey(r => r.VoucherId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Redemption>()
				.HasOne(r => r.Wallet)
				.WithMany(w => w.Redemptions)
				.HasForeignKey(r => r.WalletId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			base.OnModelCreating(modelBuilder);
		}

		public DbSet<Area> Area { get; set; }
		public DbSet<Station> Station { get; set; }
		public DbSet<Route> Route { get; set; }
		public DbSet<AppUser> AppUser { get; set; }
		public DbSet<Intimacy> Intimacy { get; set; }
		public DbSet<Bike> Bike { get; set; }
		public DbSet<Wallet> Wallet { get; set; }
		public DbSet<Trip> Trip { get; set; }
		public DbSet<Feedback> Feedback { get; set; }
		public DbSet<TripTransaction> TripTransaction { get; set; }
		public DbSet<VoucherCategory> VoucherCategory { get; set; }
		public DbSet<Voucher> Voucher { get; set; }
		public DbSet<Redemption> Redemption { get; set; }
	}
}
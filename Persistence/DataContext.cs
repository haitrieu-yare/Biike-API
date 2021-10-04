using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Persistence
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
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

			#region User
			modelBuilder.Entity<User>().ToTable("AppUser");

			modelBuilder.Entity<User>()
				.HasIndex(u => u.Email).IsUnique();

			modelBuilder.Entity<User>()
				.HasIndex(u => u.PhoneNumber).IsUnique();
			#endregion

			#region Bike
			modelBuilder.Entity<Bike>()
				.HasIndex(u => u.PlateNumber).IsUnique();

			modelBuilder.Entity<Bike>()
				.HasOne(b => b.User)
				.WithMany(u => u.Bikes)
				.HasForeignKey(b => b.UserId)
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
				.WithMany(u => u!.BikerTrips)
				.HasForeignKey(t => t.BikerId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region Feedback
			modelBuilder.Entity<Feedback>().HasKey(i => i.FeedbackId);

			modelBuilder.Entity<Feedback>()
				.HasIndex(i => new { i.UserId, i.TripId }).IsUnique(); ;

			modelBuilder.Entity<Feedback>()
				.HasOne(f => f.Trip)
				.WithMany(t => t.FeedbackList)
				.HasForeignKey(f => f.TripId)
				.OnDelete(DeleteBehavior.NoAction);

			modelBuilder.Entity<Feedback>()
				.HasOne(f => f.User)
				.WithMany(u => u.FeedBackList)
				.HasForeignKey(f => f.UserId)
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
				.HasOne(w => w.User)
				.WithMany(u => u.Wallets)
				.HasForeignKey(w => w.UserId)
				.OnDelete(DeleteBehavior.NoAction);
			#endregion

			#region VoucherCategory
			modelBuilder.Entity<VoucherCategory>()
				.HasIndex(vc => vc.CategoryName)
				.IsUnique();
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
		}

		public DbSet<Area> Area => Set<Area>();
		public DbSet<Station> Station => Set<Station>();
		public DbSet<Route> Route => Set<Route>();
		public DbSet<User> User => Set<User>();
		public DbSet<Intimacy> Intimacy => Set<Intimacy>();
		public DbSet<Bike> Bike => Set<Bike>();
		public DbSet<Wallet> Wallet => Set<Wallet>();
		public DbSet<Trip> Trip => Set<Trip>();
		public DbSet<Feedback> Feedback => Set<Feedback>();
		public DbSet<TripTransaction> TripTransaction => Set<TripTransaction>();
		public DbSet<VoucherCategory> VoucherCategory => Set<VoucherCategory>();
		public DbSet<Voucher> Voucher => Set<Voucher>();
		public DbSet<Redemption> Redemption => Set<Redemption>();
	}
}
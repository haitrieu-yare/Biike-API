using Domain;
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
			modelBuilder.Entity<Intimacy>()
				.HasOne(i => i.UserTwo)
				.WithMany(u => u.UserTwoIntimacies)
				.HasForeignKey(u => u.UserTwoId)
				.OnDelete(DeleteBehavior.Restrict);

			modelBuilder.Entity<Intimacy>()
				.HasOne(pt => pt.UserOne)
				.WithMany(u => u.UserOneIntimacies)
				.HasForeignKey(u => u.UserOneId);

			modelBuilder.Entity<Intimacy>().HasKey(i => new { i.UserOneId, i.UserTwoId });
		}

		public DbSet<Area> Area { get; set; }
		public DbSet<Station> Station { get; set; }
		public DbSet<Route> Route { get; set; }
		public DbSet<Trip> Trip { get; set; }
		public DbSet<AppUser> AppUser { get; set; }
		public DbSet<Feedback> Feedback { get; set; }
		public DbSet<Wallet> Wallet { get; set; }
		public DbSet<TripTransaction> TripTransaction { get; set; }
		public DbSet<Intimacy> Intimacy { get; set; }
	}
}
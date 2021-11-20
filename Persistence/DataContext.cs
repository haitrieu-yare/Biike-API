using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistence
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Area> Area => Set<Area>();
        public DbSet<Station> Station => Set<Station>();
        public DbSet<Route> Route => Set<Route>();
        public DbSet<Report> Report => Set<Report>();
        public DbSet<Address> Address => Set<Address>();
        public DbSet<Advertisement> Advertisement => Set<Advertisement>();
        public DbSet<UserAddress> UserAddress => Set<UserAddress>();
        public DbSet<AdvertisementAddress> AdvertisementAddress => Set<AdvertisementAddress>();
        public DbSet<VoucherAddress> VoucherAddress => Set<VoucherAddress>();
        public DbSet<AdvertisementImage> AdvertisementImage => Set<AdvertisementImage>();
        public DbSet<VoucherImage> VoucherImage => Set<VoucherImage>();
        public DbSet<User> User => Set<User>();
        public DbSet<Intimacy> Intimacy => Set<Intimacy>();
        public DbSet<Bike> Bike => Set<Bike>();
        public DbSet<Wallet> Wallet => Set<Wallet>();
        public DbSet<Trip> Trip => Set<Trip>();
        public DbSet<Feedback> Feedback => Set<Feedback>();
        public DbSet<TripTransaction> TripTransaction => Set<TripTransaction>();
        public DbSet<VoucherCategory> VoucherCategory => Set<VoucherCategory>();
        public DbSet<Voucher> Voucher => Set<Voucher>();
        public DbSet<VoucherCode> VoucherCode => Set<VoucherCode>();
        public DbSet<Redemption> Redemption => Set<Redemption>();

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
                .HasIndex(r => new {r.DepartureId, r.DestinationId})
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
            
            modelBuilder.Entity<Route>()
                .HasOne(r => r.Area)
                .WithMany(a => a.Routes)
                .HasForeignKey(s => s.AreaId)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region User

            modelBuilder.Entity<User>().ToTable("AppUser");

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email).IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.PhoneNumber).IsUnique();

            #endregion
            
            #region UserAddress

            modelBuilder.Entity<UserAddress>()
                .HasOne(a => a.User)
                .WithMany(a => a.UserAddresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

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

            #region AdvertisementAddress

            modelBuilder.Entity<AdvertisementAddress>()
                .HasOne(a => a.Advertisement)
                .WithMany(a => a.AdvertisementAddresses)
                .HasForeignKey(a => a.AdvertisementId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<AdvertisementAddress>()
                .HasOne(a => a.Address)
                .WithMany(a => a.AdvertisementAddresses)
                .HasForeignKey(a => a.AddressId)
                .OnDelete(DeleteBehavior.NoAction);
            
            #endregion
            
            #region AdvertisementImage

            modelBuilder.Entity<AdvertisementImage>()
                .HasOne(a => a.Advertisement)
                .WithMany(a => a.AdvertisementImages)
                .HasForeignKey(a => a.AdvertisementId)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region Intimacy

            modelBuilder.Entity<Intimacy>().HasKey(i => new {i.UserOneId, i.UserTwoId});

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
            
            #region Report

            modelBuilder.Entity<Report>()
                .HasOne(i => i.UserTwo)
                .WithMany(u => u.UserTwoReports)
                .HasForeignKey(i => i.UserTwoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Report>()
                .HasOne(i => i.UserOne)
                .WithMany(u => u.UserOneReports)
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
                .HasIndex(i => new {i.UserId, i.TripId}).IsUnique();

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

            #region Voucher Code

            modelBuilder.Entity<VoucherCode>()
                .HasOne(vc => vc.Voucher)
                .WithMany(v => v.VoucherCodes)
                .HasForeignKey(v => v.VoucherId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<VoucherCode>().HasIndex(vc => vc.VoucherCodeName).IsUnique();

            #endregion
            
            #region VoucherAddress

            modelBuilder.Entity<VoucherAddress>()
                .HasOne(a => a.Voucher)
                .WithMany(a => a.VoucherAddresses)
                .HasForeignKey(a => a.VoucherId)
                .OnDelete(DeleteBehavior.NoAction);
            
            modelBuilder.Entity<VoucherAddress>()
                .HasOne(a => a.Address)
                .WithMany(a => a.VoucherAddresses)
                .HasForeignKey(a => a.AddressId)
                .OnDelete(DeleteBehavior.NoAction);

            #endregion

            #region VoucherImage

            modelBuilder.Entity<VoucherImage>()
                .HasOne(v => v.Voucher)
                .WithMany(v => v.VoucherImages)
                .HasForeignKey(a => a.VoucherId)
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
    }
}
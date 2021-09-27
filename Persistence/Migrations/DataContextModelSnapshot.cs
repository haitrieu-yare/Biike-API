﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistence;

namespace Persistence.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Domain.Entities.Area", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Area");
                });

            modelBuilder.Entity("Domain.Entities.Bike", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("PlateNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("PlateNumber")
                        .IsUnique();

                    b.HasIndex("UserId");

                    b.ToTable("Bike");
                });

            modelBuilder.Entity("Domain.Entities.Feedback", b =>
                {
                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<int>("TripId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Criteria")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FeedbackContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Star")
                        .HasColumnType("int");

                    b.HasKey("UserId", "TripId");

                    b.HasIndex("TripId");

                    b.ToTable("Feedback");
                });

            modelBuilder.Entity("Domain.Entities.Intimacy", b =>
                {
                    b.Property<int>("UserOneId")
                        .HasColumnType("int");

                    b.Property<int>("UserTwoId")
                        .HasColumnType("int");

                    b.Property<DateTime>("BlockTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsBlock")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("UnblockTime")
                        .HasColumnType("datetime2");

                    b.HasKey("UserOneId", "UserTwoId");

                    b.HasIndex("UserTwoId");

                    b.ToTable("Intimacy");
                });

            modelBuilder.Entity("Domain.Entities.Redemption", b =>
                {
                    b.Property<int>("RedemptionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsUsed")
                        .HasColumnType("bit");

                    b.Property<DateTime>("RedemptionDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("VoucherCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VoucherId")
                        .HasColumnType("int");

                    b.Property<int>("VoucherPoint")
                        .HasColumnType("int");

                    b.Property<int>("WalletId")
                        .HasColumnType("int");

                    b.HasKey("RedemptionId");

                    b.HasIndex("VoucherId");

                    b.HasIndex("WalletId");

                    b.ToTable("Redemption");
                });

            modelBuilder.Entity("Domain.Entities.Route", b =>
                {
                    b.Property<int>("RouteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("DefaultPoint")
                        .HasColumnType("int");

                    b.Property<int>("DepartureId")
                        .HasColumnType("int");

                    b.Property<int>("DestinationId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("RouteId");

                    b.HasIndex("DestinationId");

                    b.HasIndex("DepartureId", "DestinationId")
                        .IsUnique();

                    b.ToTable("Route");
                });

            modelBuilder.Entity("Domain.Entities.Station", b =>
                {
                    b.Property<int>("StationId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AreaId")
                        .HasColumnType("int");

                    b.Property<string>("Coordinate")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("StationId");

                    b.HasIndex("AreaId");

                    b.ToTable("Station");
                });

            modelBuilder.Entity("Domain.Entities.Trip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("BikerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("BookTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CancelPersonId")
                        .HasColumnType("int");

                    b.Property<string>("CancelReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FinishedTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsScheduled")
                        .HasColumnType("bit");

                    b.Property<int>("KeerId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PickupTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("PlateNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RouteId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("BikerId");

                    b.HasIndex("KeerId");

                    b.HasIndex("RouteId");

                    b.ToTable("Trip");
                });

            modelBuilder.Entity("Domain.Entities.TripTransaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AmountOfPoint")
                        .HasColumnType("int");

                    b.Property<DateTime>("TransactionDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("TripId")
                        .HasColumnType("int");

                    b.Property<int>("WalletId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("TripId");

                    b.HasIndex("WalletId");

                    b.ToTable("TripTransaction");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("BirthDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<bool>("IsBikeVerified")
                        .HasColumnType("bit");

                    b.Property<string>("LastLoginDevice")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LastTimeLogin")
                        .HasColumnType("datetime2");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<double>("Star")
                        .HasColumnType("float");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<int>("TotalPoint")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("AppUser");
                });

            modelBuilder.Entity("Domain.Entities.Voucher", b =>
                {
                    b.Property<int>("VoucherId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AmountOfPoint")
                        .HasColumnType("int");

                    b.Property<string>("Brand")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<int>("Remaining")
                        .HasColumnType("int");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("TermsAndConditions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("VoucherCategoryId")
                        .HasColumnType("int");

                    b.Property<string>("VoucherName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("VoucherId");

                    b.HasIndex("VoucherCategoryId");

                    b.ToTable("Voucher");
                });

            modelBuilder.Entity("Domain.Entities.VoucherCategory", b =>
                {
                    b.Property<int>("VoucherCategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CategoryName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("VoucherCategoryId");

                    b.HasIndex("CategoryName")
                        .IsUnique();

                    b.ToTable("VoucherCategory");
                });

            modelBuilder.Entity("Domain.Entities.Wallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("FromDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("Point")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<DateTime>("ToDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Wallet");
                });

            modelBuilder.Entity("Domain.Entities.Bike", b =>
                {
                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("Bikes")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.Feedback", b =>
                {
                    b.HasOne("Domain.Entities.Trip", "Trip")
                        .WithMany("FeedbackList")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("FeedBackList")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Trip");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.Intimacy", b =>
                {
                    b.HasOne("Domain.Entities.User", "UserOne")
                        .WithMany("UserOneIntimacies")
                        .HasForeignKey("UserOneId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Entities.User", "UserTwo")
                        .WithMany("UserTwoIntimacies")
                        .HasForeignKey("UserTwoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("UserOne");

                    b.Navigation("UserTwo");
                });

            modelBuilder.Entity("Domain.Entities.Redemption", b =>
                {
                    b.HasOne("Domain.Entities.Voucher", "Voucher")
                        .WithMany("Redemptions")
                        .HasForeignKey("VoucherId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Wallet", "Wallet")
                        .WithMany("Redemptions")
                        .HasForeignKey("WalletId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Voucher");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Domain.Entities.Route", b =>
                {
                    b.HasOne("Domain.Entities.Station", "Departure")
                        .WithMany("DepartureRoutes")
                        .HasForeignKey("DepartureId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Station", "Destination")
                        .WithMany("DestinationRoutes")
                        .HasForeignKey("DestinationId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Departure");

                    b.Navigation("Destination");
                });

            modelBuilder.Entity("Domain.Entities.Station", b =>
                {
                    b.HasOne("Domain.Entities.Area", "Area")
                        .WithMany("Stations")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("Domain.Entities.Trip", b =>
                {
                    b.HasOne("Domain.Entities.User", "Biker")
                        .WithMany("BikerTrips")
                        .HasForeignKey("BikerId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("Domain.Entities.User", "Keer")
                        .WithMany("KeerTrips")
                        .HasForeignKey("KeerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Route", "Route")
                        .WithMany("Trips")
                        .HasForeignKey("RouteId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Biker");

                    b.Navigation("Keer");

                    b.Navigation("Route");
                });

            modelBuilder.Entity("Domain.Entities.TripTransaction", b =>
                {
                    b.HasOne("Domain.Entities.Trip", "Trip")
                        .WithMany("TripTransactions")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Entities.Wallet", "Wallet")
                        .WithMany("TripTransactions")
                        .HasForeignKey("WalletId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Trip");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Domain.Entities.Voucher", b =>
                {
                    b.HasOne("Domain.Entities.VoucherCategory", "VoucherCategory")
                        .WithMany("Vouchers")
                        .HasForeignKey("VoucherCategoryId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("VoucherCategory");
                });

            modelBuilder.Entity("Domain.Entities.Wallet", b =>
                {
                    b.HasOne("Domain.Entities.User", "User")
                        .WithMany("Wallets")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Domain.Entities.Area", b =>
                {
                    b.Navigation("Stations");
                });

            modelBuilder.Entity("Domain.Entities.Route", b =>
                {
                    b.Navigation("Trips");
                });

            modelBuilder.Entity("Domain.Entities.Station", b =>
                {
                    b.Navigation("DepartureRoutes");

                    b.Navigation("DestinationRoutes");
                });

            modelBuilder.Entity("Domain.Entities.Trip", b =>
                {
                    b.Navigation("FeedbackList");

                    b.Navigation("TripTransactions");
                });

            modelBuilder.Entity("Domain.Entities.User", b =>
                {
                    b.Navigation("BikerTrips");

                    b.Navigation("Bikes");

                    b.Navigation("FeedBackList");

                    b.Navigation("KeerTrips");

                    b.Navigation("UserOneIntimacies");

                    b.Navigation("UserTwoIntimacies");

                    b.Navigation("Wallets");
                });

            modelBuilder.Entity("Domain.Entities.Voucher", b =>
                {
                    b.Navigation("Redemptions");
                });

            modelBuilder.Entity("Domain.Entities.VoucherCategory", b =>
                {
                    b.Navigation("Vouchers");
                });

            modelBuilder.Entity("Domain.Entities.Wallet", b =>
                {
                    b.Navigation("Redemptions");

                    b.Navigation("TripTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}

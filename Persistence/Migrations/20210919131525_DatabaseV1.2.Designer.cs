﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Persistence;

namespace Persistence.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20210919131525_DatabaseV1.2")]
    partial class DatabaseV12
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Domain.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("FullName")
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

                    b.HasKey("Id");

                    b.HasIndex("Email")
                        .IsUnique();

                    b.HasIndex("PhoneNumber")
                        .IsUnique();

                    b.ToTable("AppUser");
                });

            modelBuilder.Entity("Domain.Area", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Area");
                });

            modelBuilder.Entity("Domain.Bike", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<string>("Brand")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Color")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("PlateNumber")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("PlateNumber")
                        .IsUnique()
                        .HasFilter("[PlateNumber] IS NOT NULL");

                    b.ToTable("Bike");
                });

            modelBuilder.Entity("Domain.Feedback", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<string>("Criteria")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FeedbackContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Star")
                        .HasColumnType("float");

                    b.Property<int>("TripId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId");

                    b.HasIndex("TripId");

                    b.ToTable("Feedback");
                });

            modelBuilder.Entity("Domain.Intimacy", b =>
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

            modelBuilder.Entity("Domain.Route", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DefaultPoint")
                        .HasColumnType("int");

                    b.Property<int>("DepartureId")
                        .HasColumnType("int");

                    b.Property<int>("DestinationId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("DepartureId");

                    b.HasIndex("DestinationId");

                    b.ToTable("Route");
                });

            modelBuilder.Entity("Domain.Station", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("AreaId")
                        .HasColumnType("int");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<string>("Latitude")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Longitude")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AreaId");

                    b.ToTable("Station");
                });

            modelBuilder.Entity("Domain.Trip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("BikerId")
                        .HasColumnType("int");

                    b.Property<DateTime>("BookTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("CancelPersonId")
                        .HasColumnType("int");

                    b.Property<string>("CancelReason")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("FinishedTime")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsScheduled")
                        .HasColumnType("bit");

                    b.Property<int>("KeerId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("PickupTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("PlateNumber")
                        .IsRequired()
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

            modelBuilder.Entity("Domain.TripTransaction", b =>
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

                    b.Property<bool>("isBiker")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("TripId");

                    b.HasIndex("WalletId");

                    b.ToTable("TripTransaction");
                });

            modelBuilder.Entity("Domain.Wallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<int>("Point")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("AppUserId")
                        .IsUnique();

                    b.ToTable("Wallet");
                });

            modelBuilder.Entity("Domain.Bike", b =>
                {
                    b.HasOne("Domain.AppUser", "AppUser")
                        .WithMany("Bikes")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("Domain.Feedback", b =>
                {
                    b.HasOne("Domain.AppUser", "AppUser")
                        .WithMany("FeedBackList")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Trip", "Trip")
                        .WithMany("FeedbackList")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("AppUser");

                    b.Navigation("Trip");
                });

            modelBuilder.Entity("Domain.Intimacy", b =>
                {
                    b.HasOne("Domain.AppUser", "UserOne")
                        .WithMany("UserOneIntimacies")
                        .HasForeignKey("UserOneId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.AppUser", "UserTwo")
                        .WithMany("UserTwoIntimacies")
                        .HasForeignKey("UserTwoId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("UserOne");

                    b.Navigation("UserTwo");
                });

            modelBuilder.Entity("Domain.Route", b =>
                {
                    b.HasOne("Domain.Station", "Departure")
                        .WithMany("DepartureRoutes")
                        .HasForeignKey("DepartureId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Station", "Destination")
                        .WithMany("DestinationRoutes")
                        .HasForeignKey("DestinationId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Departure");

                    b.Navigation("Destination");
                });

            modelBuilder.Entity("Domain.Station", b =>
                {
                    b.HasOne("Domain.Area", "Area")
                        .WithMany("Stations")
                        .HasForeignKey("AreaId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Area");
                });

            modelBuilder.Entity("Domain.Trip", b =>
                {
                    b.HasOne("Domain.AppUser", "Biker")
                        .WithMany("BikerTrips")
                        .HasForeignKey("BikerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.AppUser", "Keer")
                        .WithMany("KeerTrips")
                        .HasForeignKey("KeerId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Route", "Route")
                        .WithMany("Trips")
                        .HasForeignKey("RouteId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Biker");

                    b.Navigation("Keer");

                    b.Navigation("Route");
                });

            modelBuilder.Entity("Domain.TripTransaction", b =>
                {
                    b.HasOne("Domain.Trip", "Trip")
                        .WithMany("TripTransactions")
                        .HasForeignKey("TripId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("Domain.Wallet", "Wallet")
                        .WithMany("TripTransactions")
                        .HasForeignKey("WalletId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Trip");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Domain.Wallet", b =>
                {
                    b.HasOne("Domain.AppUser", "AppUser")
                        .WithOne("Wallet")
                        .HasForeignKey("Domain.Wallet", "AppUserId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("AppUser");
                });

            modelBuilder.Entity("Domain.AppUser", b =>
                {
                    b.Navigation("BikerTrips");

                    b.Navigation("Bikes");

                    b.Navigation("FeedBackList");

                    b.Navigation("KeerTrips");

                    b.Navigation("UserOneIntimacies");

                    b.Navigation("UserTwoIntimacies");

                    b.Navigation("Wallet");
                });

            modelBuilder.Entity("Domain.Area", b =>
                {
                    b.Navigation("Stations");
                });

            modelBuilder.Entity("Domain.Route", b =>
                {
                    b.Navigation("Trips");
                });

            modelBuilder.Entity("Domain.Station", b =>
                {
                    b.Navigation("DepartureRoutes");

                    b.Navigation("DestinationRoutes");
                });

            modelBuilder.Entity("Domain.Trip", b =>
                {
                    b.Navigation("FeedbackList");

                    b.Navigation("TripTransactions");
                });

            modelBuilder.Entity("Domain.Wallet", b =>
                {
                    b.Navigation("TripTransactions");
                });
#pragma warning restore 612, 618
        }
    }
}

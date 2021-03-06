using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    public partial class DatabaseV10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                columns: table => new
                {
                    AddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddressName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressDetail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressCoordinate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.AddressId);
                });

            migrationBuilder.CreateTable(
                name: "Area",
                columns: table => new
                {
                    AreaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailExtension = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Area", x => x.AreaId);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "VoucherCategory",
                columns: table => new
                {
                    VoucherCategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherCategory", x => x.VoucherCategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotificationType = table.Column<int>(type: "int", nullable: false),
                    UserType = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    CoverImage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.NotificationId);
                    table.ForeignKey(
                        name: "FK_Notification_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "AreaId");
                });

            migrationBuilder.CreateTable(
                name: "Station",
                columns: table => new
                {
                    StationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Coordinate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCentralPoint = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Station", x => x.StationId);
                    table.ForeignKey(
                        name: "FK_Station_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "AreaId");
                });

            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PhoneNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Avatar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LastLoginDevice = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastTimeLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Star = table.Column<double>(type: "float", nullable: false),
                    TotalPoint = table.Column<int>(type: "int", nullable: false),
                    MaxTotalPoint = table.Column<int>(type: "int", nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsPhoneVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsBikeVerified = table.Column<bool>(type: "bit", nullable: false),
                    IsKeNowAvailable = table.Column<bool>(type: "bit", nullable: false),
                    BirthDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUser", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_AppUser_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "RoleId");
                });

            migrationBuilder.CreateTable(
                name: "Voucher",
                columns: table => new
                {
                    VoucherId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherCategoryId = table.Column<int>(type: "int", nullable: false),
                    VoucherName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Remaining = table.Column<int>(type: "int", nullable: false),
                    AmountOfPoint = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Voucher", x => x.VoucherId);
                    table.ForeignKey(
                        name: "FK_Voucher_VoucherCategory_VoucherCategoryId",
                        column: x => x.VoucherCategoryId,
                        principalTable: "VoucherCategory",
                        principalColumn: "VoucherCategoryId");
                });

            migrationBuilder.CreateTable(
                name: "Route",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartureId = table.Column<int>(type: "int", nullable: false),
                    DestinationId = table.Column<int>(type: "int", nullable: false),
                    AreaId = table.Column<int>(type: "int", nullable: false),
                    DefaultPoint = table.Column<int>(type: "int", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Route", x => x.RouteId);
                    table.ForeignKey(
                        name: "FK_Route_Area_AreaId",
                        column: x => x.AreaId,
                        principalTable: "Area",
                        principalColumn: "AreaId");
                    table.ForeignKey(
                        name: "FK_Route_Station_DepartureId",
                        column: x => x.DepartureId,
                        principalTable: "Station",
                        principalColumn: "StationId");
                    table.ForeignKey(
                        name: "FK_Route_Station_DestinationId",
                        column: x => x.DestinationId,
                        principalTable: "Station",
                        principalColumn: "StationId");
                });

            migrationBuilder.CreateTable(
                name: "Advertisement",
                columns: table => new
                {
                    AdvertisementId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    AdvertisementUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TotalClickCount = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisement", x => x.AdvertisementId);
                    table.ForeignKey(
                        name: "FK_Advertisement_AppUser_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Bike",
                columns: table => new
                {
                    BikeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    BikeOwner = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BikePicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BikeLicensePicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlateNumberPicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DrivingLicenseFrontPicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DrivingLicenseBackPicture = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    FailedVerificationReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlateNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BikeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BikeVolume = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bike", x => x.BikeId);
                    table.ForeignKey(
                        name: "FK_Bike_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "BikeAvailability",
                columns: table => new
                {
                    BikeAvailabilityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    StationId = table.Column<int>(type: "int", nullable: false),
                    FromTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BikeAvailability", x => x.BikeAvailabilityId);
                    table.ForeignKey(
                        name: "FK_BikeAvailability_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_BikeAvailability_Station_StationId",
                        column: x => x.StationId,
                        principalTable: "Station",
                        principalColumn: "StationId");
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    ConfigurationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConfigurationName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ConfigurationValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    LastUpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ConfigurationId);
                    table.ForeignKey(
                        name: "FK_Configuration_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Intimacy",
                columns: table => new
                {
                    UserOneId = table.Column<int>(type: "int", nullable: false),
                    UserTwoId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsBlock = table.Column<bool>(type: "bit", nullable: false),
                    BlockTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UnblockTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intimacy", x => new { x.UserOneId, x.UserTwoId });
                    table.ForeignKey(
                        name: "FK_Intimacy_AppUser_UserOneId",
                        column: x => x.UserOneId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Intimacy_AppUser_UserTwoId",
                        column: x => x.UserTwoId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "MomoTransaction",
                columns: table => new
                {
                    MomoTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TransactionId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    Point = table.Column<int>(type: "int", nullable: false),
                    ConversionRate = table.Column<double>(type: "float", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MomoTransaction", x => x.MomoTransactionId);
                    table.ForeignKey(
                        name: "FK_MomoTransaction_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "PointHistory",
                columns: table => new
                {
                    PointHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    HistoryType = table.Column<int>(type: "int", nullable: false),
                    RelatedId = table.Column<int>(type: "int", nullable: false),
                    Point = table.Column<int>(type: "int", nullable: false),
                    TotalPoint = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointHistory", x => x.PointHistoryId);
                    table.ForeignKey(
                        name: "FK_PointHistory_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Report",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserOneId = table.Column<int>(type: "int", nullable: false),
                    UserOneName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserTwoId = table.Column<int>(type: "int", nullable: false),
                    UserTwoName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReportReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_Report_AppUser_UserOneId",
                        column: x => x.UserOneId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Report_AppUser_UserTwoId",
                        column: x => x.UserTwoId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Sos",
                columns: table => new
                {
                    SosId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SosName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SosPhone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sos", x => x.SosId);
                    table.ForeignKey(
                        name: "FK_Sos_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "UserAddress",
                columns: table => new
                {
                    UserAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserAddressName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAddressDetail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAddressCoordinate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserAddressNote = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddress", x => x.UserAddressId);
                    table.ForeignKey(
                        name: "FK_UserAddress_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "Wallet",
                columns: table => new
                {
                    WalletId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Point = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet", x => x.WalletId);
                    table.ForeignKey(
                        name: "FK_Wallet_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateTable(
                name: "VoucherAddress",
                columns: table => new
                {
                    VoucherAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherId = table.Column<int>(type: "int", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherAddress", x => x.VoucherAddressId);
                    table.ForeignKey(
                        name: "FK_VoucherAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_VoucherAddress_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "VoucherId");
                });

            migrationBuilder.CreateTable(
                name: "VoucherCode",
                columns: table => new
                {
                    VoucherCodeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherId = table.Column<int>(type: "int", nullable: false),
                    VoucherCodeName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsRedeemed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherCode", x => x.VoucherCodeId);
                    table.ForeignKey(
                        name: "FK_VoucherCode_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "VoucherId");
                });

            migrationBuilder.CreateTable(
                name: "VoucherImage",
                columns: table => new
                {
                    VoucherImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VoucherId = table.Column<int>(type: "int", nullable: false),
                    VoucherImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherImage", x => x.VoucherImageId);
                    table.ForeignKey(
                        name: "FK_VoucherImage_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "VoucherId");
                });

            migrationBuilder.CreateTable(
                name: "Trip",
                columns: table => new
                {
                    TripId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeerId = table.Column<int>(type: "int", nullable: false),
                    BikerId = table.Column<int>(type: "int", nullable: true),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    BookTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FirstPersonArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FirstPersonArrivalId = table.Column<int>(type: "int", nullable: true),
                    SecondPersonArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SecondPersonArrivalId = table.Column<int>(type: "int", nullable: true),
                    PickupTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinishedTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CancelTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsScheduled = table.Column<bool>(type: "bit", nullable: false),
                    CancelPersonId = table.Column<int>(type: "int", nullable: true),
                    CancelReason = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trip", x => x.TripId);
                    table.ForeignKey(
                        name: "FK_Trip_AppUser_BikerId",
                        column: x => x.BikerId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Trip_AppUser_KeerId",
                        column: x => x.KeerId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Trip_Route_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Route",
                        principalColumn: "RouteId");
                });

            migrationBuilder.CreateTable(
                name: "AdvertisementAddress",
                columns: table => new
                {
                    AdvertisementAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvertisementId = table.Column<int>(type: "int", nullable: false),
                    AddressId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementAddress", x => x.AdvertisementAddressId);
                    table.ForeignKey(
                        name: "FK_AdvertisementAddress_Address_AddressId",
                        column: x => x.AddressId,
                        principalTable: "Address",
                        principalColumn: "AddressId");
                    table.ForeignKey(
                        name: "FK_AdvertisementAddress_Advertisement_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisement",
                        principalColumn: "AdvertisementId");
                });

            migrationBuilder.CreateTable(
                name: "AdvertisementImage",
                columns: table => new
                {
                    AdvertisementImageId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdvertisementId = table.Column<int>(type: "int", nullable: false),
                    AdvertisementImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdvertisementImage", x => x.AdvertisementImageId);
                    table.ForeignKey(
                        name: "FK_AdvertisementImage_Advertisement_AdvertisementId",
                        column: x => x.AdvertisementId,
                        principalTable: "Advertisement",
                        principalColumn: "AdvertisementId");
                });

            migrationBuilder.CreateTable(
                name: "Redemption",
                columns: table => new
                {
                    RedemptionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    VoucherId = table.Column<int>(type: "int", nullable: false),
                    VoucherCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VoucherPoint = table.Column<int>(type: "int", nullable: false),
                    IsUsed = table.Column<bool>(type: "bit", nullable: false),
                    RedemptionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Redemption", x => x.RedemptionId);
                    table.ForeignKey(
                        name: "FK_Redemption_Voucher_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Voucher",
                        principalColumn: "VoucherId");
                    table.ForeignKey(
                        name: "FK_Redemption_Wallet_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallet",
                        principalColumn: "WalletId");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    FeedbackContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TripStar = table.Column<int>(type: "int", nullable: false),
                    TripTip = table.Column<int>(type: "int", nullable: false),
                    Criteria = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_Feedback_AppUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AppUser",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_Feedback_Trip_TripId",
                        column: x => x.TripId,
                        principalTable: "Trip",
                        principalColumn: "TripId");
                });

            migrationBuilder.CreateTable(
                name: "TripTransaction",
                columns: table => new
                {
                    TripTransactionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TripId = table.Column<int>(type: "int", nullable: false),
                    WalletId = table.Column<int>(type: "int", nullable: false),
                    AmountOfPoint = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TripTransaction", x => x.TripTransactionId);
                    table.ForeignKey(
                        name: "FK_TripTransaction_Trip_TripId",
                        column: x => x.TripId,
                        principalTable: "Trip",
                        principalColumn: "TripId");
                    table.ForeignKey(
                        name: "FK_TripTransaction_Wallet_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallet",
                        principalColumn: "WalletId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advertisement_CreatorId",
                table: "Advertisement",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementAddress_AddressId",
                table: "AdvertisementAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementAddress_AdvertisementId",
                table: "AdvertisementAddress",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisementImage_AdvertisementId",
                table: "AdvertisementImage",
                column: "AdvertisementId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_Email",
                table: "AppUser",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_PhoneNumber",
                table: "AppUser",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUser_RoleId",
                table: "AppUser",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Bike_PlateNumber",
                table: "Bike",
                column: "PlateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bike_UserId",
                table: "Bike",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BikeAvailability_StationId",
                table: "BikeAvailability",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_BikeAvailability_UserId",
                table: "BikeAvailability",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_ConfigurationName",
                table: "Configuration",
                column: "ConfigurationName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Configuration_UserId",
                table: "Configuration",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_TripId",
                table: "Feedback",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_UserId_TripId",
                table: "Feedback",
                columns: new[] { "UserId", "TripId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Intimacy_UserTwoId",
                table: "Intimacy",
                column: "UserTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_MomoTransaction_OrderId",
                table: "MomoTransaction",
                column: "OrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MomoTransaction_TransactionId",
                table: "MomoTransaction",
                column: "TransactionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MomoTransaction_UserId",
                table: "MomoTransaction",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_AreaId",
                table: "Notification",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_PointHistory_UserId",
                table: "PointHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Redemption_VoucherId",
                table: "Redemption",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_Redemption_WalletId",
                table: "Redemption",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_UserOneId",
                table: "Report",
                column: "UserOneId");

            migrationBuilder.CreateIndex(
                name: "IX_Report_UserTwoId",
                table: "Report",
                column: "UserTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_Route_AreaId",
                table: "Route",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Route_DepartureId_DestinationId",
                table: "Route",
                columns: new[] { "DepartureId", "DestinationId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Route_DestinationId",
                table: "Route",
                column: "DestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_Sos_UserId",
                table: "Sos",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Station_AreaId",
                table: "Station",
                column: "AreaId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_BikerId",
                table: "Trip",
                column: "BikerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_KeerId",
                table: "Trip",
                column: "KeerId");

            migrationBuilder.CreateIndex(
                name: "IX_Trip_RouteId",
                table: "Trip",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_TripTransaction_TripId",
                table: "TripTransaction",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_TripTransaction_WalletId",
                table: "TripTransaction",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddress_UserId",
                table: "UserAddress",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Voucher_VoucherCategoryId",
                table: "Voucher",
                column: "VoucherCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAddress_AddressId",
                table: "VoucherAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAddress_VoucherId",
                table: "VoucherAddress",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherCategory_CategoryName",
                table: "VoucherCategory",
                column: "CategoryName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoucherCode_VoucherCodeName",
                table: "VoucherCode",
                column: "VoucherCodeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoucherCode_VoucherId",
                table: "VoucherCode",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherImage_VoucherId",
                table: "VoucherImage",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_UserId",
                table: "Wallet",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdvertisementAddress");

            migrationBuilder.DropTable(
                name: "AdvertisementImage");

            migrationBuilder.DropTable(
                name: "Bike");

            migrationBuilder.DropTable(
                name: "BikeAvailability");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Intimacy");

            migrationBuilder.DropTable(
                name: "MomoTransaction");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "PointHistory");

            migrationBuilder.DropTable(
                name: "Redemption");

            migrationBuilder.DropTable(
                name: "Report");

            migrationBuilder.DropTable(
                name: "Sos");

            migrationBuilder.DropTable(
                name: "TripTransaction");

            migrationBuilder.DropTable(
                name: "UserAddress");

            migrationBuilder.DropTable(
                name: "VoucherAddress");

            migrationBuilder.DropTable(
                name: "VoucherCode");

            migrationBuilder.DropTable(
                name: "VoucherImage");

            migrationBuilder.DropTable(
                name: "Advertisement");

            migrationBuilder.DropTable(
                name: "Trip");

            migrationBuilder.DropTable(
                name: "Wallet");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropTable(
                name: "Voucher");

            migrationBuilder.DropTable(
                name: "Route");

            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropTable(
                name: "VoucherCategory");

            migrationBuilder.DropTable(
                name: "Station");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Area");
        }
    }
}

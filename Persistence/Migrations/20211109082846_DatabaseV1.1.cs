using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "VoucherAddress");

            migrationBuilder.DropColumn(
                name: "VoucherAddressCoordinate",
                table: "VoucherAddress");

            migrationBuilder.DropColumn(
                name: "VoucherAddressDetail",
                table: "VoucherAddress");

            migrationBuilder.DropColumn(
                name: "VoucherAddressName",
                table: "VoucherAddress");

            migrationBuilder.DropColumn(
                name: "AdvertisingAddressCoordinate",
                table: "AdvertisingAddress");

            migrationBuilder.DropColumn(
                name: "AdvertisingAddressDetail",
                table: "AdvertisingAddress");

            migrationBuilder.DropColumn(
                name: "AdvertisingAddressName",
                table: "AdvertisingAddress");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "AdvertisingAddress");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "VoucherAddress",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "AdvertisingAddress",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAddress_AddressId",
                table: "VoucherAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisingAddress_AddressId",
                table: "AdvertisingAddress",
                column: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdvertisingAddress_Address_AddressId",
                table: "AdvertisingAddress",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "AddressId");

            migrationBuilder.AddForeignKey(
                name: "FK_VoucherAddress_Address_AddressId",
                table: "VoucherAddress",
                column: "AddressId",
                principalTable: "Address",
                principalColumn: "AddressId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdvertisingAddress_Address_AddressId",
                table: "AdvertisingAddress");

            migrationBuilder.DropForeignKey(
                name: "FK_VoucherAddress_Address_AddressId",
                table: "VoucherAddress");

            migrationBuilder.DropTable(
                name: "Address");

            migrationBuilder.DropIndex(
                name: "IX_VoucherAddress_AddressId",
                table: "VoucherAddress");

            migrationBuilder.DropIndex(
                name: "IX_AdvertisingAddress_AddressId",
                table: "AdvertisingAddress");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "VoucherAddress");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "AdvertisingAddress");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "VoucherAddress",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "VoucherAddressCoordinate",
                table: "VoucherAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VoucherAddressDetail",
                table: "VoucherAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "VoucherAddressName",
                table: "VoucherAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdvertisingAddressCoordinate",
                table: "AdvertisingAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdvertisingAddressDetail",
                table: "AdvertisingAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdvertisingAddressName",
                table: "AdvertisingAddress",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "AdvertisingAddress",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV19 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallet_AppUserId",
                table: "Wallet");

            migrationBuilder.AddColumn<DateTime>(
                name: "FromDate",
                table: "Wallet",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "ToDate",
                table: "Wallet",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TotalPoint",
                table: "AppUser",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_AppUserId",
                table: "Wallet",
                column: "AppUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Wallet_AppUserId",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "FromDate",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "ToDate",
                table: "Wallet");

            migrationBuilder.DropColumn(
                name: "TotalPoint",
                table: "AppUser");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_AppUserId",
                table: "Wallet",
                column: "AppUserId",
                unique: true);
        }
    }
}

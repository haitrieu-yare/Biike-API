using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV110 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAddress_AddressId",
                table: "UserAddress");

            migrationBuilder.DropIndex(
                name: "IX_AdvertisingAddress_AddressId",
                table: "AdvertisingAddress");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddress_AddressId",
                table: "UserAddress",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisingAddress_AddressId",
                table: "AdvertisingAddress",
                column: "AddressId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserAddress_AddressId",
                table: "UserAddress");

            migrationBuilder.DropIndex(
                name: "IX_AdvertisingAddress_AddressId",
                table: "AdvertisingAddress");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddress_AddressId",
                table: "UserAddress",
                column: "AddressId");

            migrationBuilder.CreateIndex(
                name: "IX_AdvertisingAddress_AddressId",
                table: "AdvertisingAddress",
                column: "AddressId");
        }
    }
}

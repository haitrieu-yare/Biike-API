using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV11 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "Bike",
                newName: "PlateNumberPicture");

            migrationBuilder.AddColumn<string>(
                name: "BikeLicensePicture",
                table: "Bike",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BikePicture",
                table: "Bike",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BikeLicensePicture",
                table: "Bike");

            migrationBuilder.DropColumn(
                name: "BikePicture",
                table: "Bike");

            migrationBuilder.RenameColumn(
                name: "PlateNumberPicture",
                table: "Bike",
                newName: "Picture");
        }
    }
}

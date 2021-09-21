using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV14 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Station");

            migrationBuilder.RenameColumn(
                name: "Longitude",
                table: "Station",
                newName: "Coordinate");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Coordinate",
                table: "Station",
                newName: "Longitude");

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Station",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}

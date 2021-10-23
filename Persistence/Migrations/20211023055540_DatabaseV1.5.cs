using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Area",
                newName: "AreaId");

            migrationBuilder.AddColumn<int>(
                name: "AreaId",
                table: "Route",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Route_AreaId",
                table: "Route",
                column: "AreaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Route_Area_AreaId",
                table: "Route",
                column: "AreaId",
                principalTable: "Area",
                principalColumn: "AreaId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Route_Area_AreaId",
                table: "Route");

            migrationBuilder.DropIndex(
                name: "IX_Route_AreaId",
                table: "Route");

            migrationBuilder.DropColumn(
                name: "AreaId",
                table: "Route");

            migrationBuilder.RenameColumn(
                name: "AreaId",
                table: "Area",
                newName: "Id");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV16 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Route_Area_AreaId",
                table: "Route");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "Route",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Route_Area_AreaId",
                table: "Route",
                column: "AreaId",
                principalTable: "Area",
                principalColumn: "AreaId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Route_Area_AreaId",
                table: "Route");

            migrationBuilder.AlterColumn<int>(
                name: "AreaId",
                table: "Route",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Route_Area_AreaId",
                table: "Route",
                column: "AreaId",
                principalTable: "Area",
                principalColumn: "AreaId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

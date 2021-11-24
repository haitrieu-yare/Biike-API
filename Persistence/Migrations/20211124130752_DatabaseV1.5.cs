using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV15 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Sos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Sos_UserId",
                table: "Sos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sos_AppUser_UserId",
                table: "Sos",
                column: "UserId",
                principalTable: "AppUser",
                principalColumn: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sos_AppUser_UserId",
                table: "Sos");

            migrationBuilder.DropIndex(
                name: "IX_Sos_UserId",
                table: "Sos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Sos");
        }
    }
}

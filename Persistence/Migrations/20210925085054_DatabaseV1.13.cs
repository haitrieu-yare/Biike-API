using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV113 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VourcherCode",
                table: "Redemption",
                newName: "VoucherCode");

            migrationBuilder.AddColumn<int>(
                name: "VoucherPoint",
                table: "Redemption",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VoucherPoint",
                table: "Redemption");

            migrationBuilder.RenameColumn(
                name: "VoucherCode",
                table: "Redemption",
                newName: "VourcherCode");
        }
    }
}

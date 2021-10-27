using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV111 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAddress_AddressId",
                table: "VoucherAddress",
                column: "AddressId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoucherAddress_VoucherId",
                table: "VoucherAddress",
                column: "VoucherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoucherAddress");
        }
    }
}

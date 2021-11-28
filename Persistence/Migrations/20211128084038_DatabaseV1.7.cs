using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Persistence.Migrations
{
    public partial class DatabaseV17 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FirstPersonArrivalId",
                table: "Trip",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FirstPersonArrivalTime",
                table: "Trip",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondPersonArrivalId",
                table: "Trip",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SecondPersonArrivalTime",
                table: "Trip",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstPersonArrivalId",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "FirstPersonArrivalTime",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "SecondPersonArrivalId",
                table: "Trip");

            migrationBuilder.DropColumn(
                name: "SecondPersonArrivalTime",
                table: "Trip");
        }
    }
}

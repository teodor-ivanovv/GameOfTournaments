using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GameOfTournaments.Data.Migrations
{
    public partial class DeletableGame : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "Games",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "Time",
                table: "Games",
                type: "timestamp with time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "Games");
        }
    }
}

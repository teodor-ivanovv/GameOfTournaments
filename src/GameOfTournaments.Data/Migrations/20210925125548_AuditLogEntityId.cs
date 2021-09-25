using Microsoft.EntityFrameworkCore.Migrations;

namespace GameOfTournaments.Data.Migrations
{
    public partial class AuditLogEntityId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                table: "AuditLogs",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "AuditLogs");
        }
    }
}

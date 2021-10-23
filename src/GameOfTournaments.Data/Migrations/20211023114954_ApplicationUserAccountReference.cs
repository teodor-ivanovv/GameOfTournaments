using Microsoft.EntityFrameworkCore.Migrations;

namespace GameOfTournaments.Data.Migrations
{
    public partial class ApplicationUserAccountReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsersAccounts_ApplicationUserId",
                table: "ApplicationUsersAccounts");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserAccountId",
                table: "AspNetUsers",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsersAccounts_ApplicationUserId",
                table: "ApplicationUsersAccounts",
                column: "ApplicationUserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ApplicationUsersAccounts_ApplicationUserId",
                table: "ApplicationUsersAccounts");

            migrationBuilder.DropColumn(
                name: "ApplicationUserAccountId",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsersAccounts_ApplicationUserId",
                table: "ApplicationUsersAccounts",
                column: "ApplicationUserId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class MainMigration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "ProfilePicture",
                value: "https://imgur.com/a/fgzQBjj.jpeg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "ProfilePicture",
                value: "https://imgur.com/YlChUAS.jpeg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "ProfilePicture",
                value: "https://imgur.com/a/fgzQBjj");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                column: "ProfilePicture",
                value: "https://imgur.com/YlChUAS");
        }
    }
}

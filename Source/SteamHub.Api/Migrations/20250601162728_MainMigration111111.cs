using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class MainMigration111111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "ProfilePicture",
                value: "https://imgur.com/2dMXXul.jpeg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                column: "ProfilePicture",
                value: "https://imgur.com/BLQIZ4A.jpeg");
        }
    }
}

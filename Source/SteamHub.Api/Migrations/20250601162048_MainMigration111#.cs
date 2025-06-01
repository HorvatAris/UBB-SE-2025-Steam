using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class MainMigration111 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "ProfilePicture",
                value: "https://imgur.com/dzzEUC5.jpeg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "ProfilePicture",
                value: "https://imgur.com/vfYNvcb.jpeg");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                column: "ProfilePicture",
                value: "https://i.imgur.com/JPNXxsg.jpeg");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                column: "ProfilePicture",
                value: "https://i.imgur.com/JPNXxsg.jpeg");
        }
    }
}

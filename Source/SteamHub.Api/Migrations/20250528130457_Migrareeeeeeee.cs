using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class Migrareeeeeeee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Achievements",
                columns: new[] { "AchievementId", "AchievementName", "AchievementType", "Description", "Icon", "Points" },
                values: new object[,]
                {
                    { 6, "OWNEDGAMES1", "Owned Games", "You own 1 game, you get 1 point", "https://cdn-icons-png.flaticon.com/512/5139/5139999.png", 1 },
                    { 7, "OWNEDGAMES2", "Owned Games", "You own 5 games, you get 3 points", "https://cdn-icons-png.flaticon.com/512/5139/5139999.png", 3 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Achievements",
                keyColumn: "AchievementId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Achievements",
                keyColumn: "AchievementId",
                keyValue: 7);
        }
    }
}

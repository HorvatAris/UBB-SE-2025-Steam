using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class NewMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Friendships",
                columns: new[] { "FriendshipId", "FriendId", "UserId", "UserProfileProfileId" },
                values: new object[,]
                {
                    { 1, 1, 5, null },
                    { 2, 2, 5, null },
                    { 3, 3, 5, null },
                    { 4, 4, 5, null },
                    { 5, 6, 5, null },
                    { 6, 7, 5, null },
                    { 7, 8, 5, null },
                    { 8, 6, 4, null },
                    { 9, 7, 4, null },
                    { 10, 7, 6, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 10);
        }
    }
}

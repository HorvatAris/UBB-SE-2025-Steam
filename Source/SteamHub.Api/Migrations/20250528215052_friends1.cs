using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class friends1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Friendships",
                columns: new[] { "FriendshipId", "FriendId", "UserId", "UserProfileProfileId" },
                values: new object[,]
                {
                    { 11, 5, 1, null },
                    { 12, 5, 2, null },
                    { 13, 5, 3, null },
                    { 14, 5, 4, null },
                    { 15, 5, 6, null },
                    { 16, 5, 7, null },
                    { 17, 5, 8, null },
                    { 18, 3, 1, null },
                    { 19, 3, 2, null },
                    { 20, 4, 3, null },
                    { 21, 8, 6, null },
                    { 22, 2, 1, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "Friendships",
                keyColumn: "FriendshipId",
                keyValue: 22);
        }
    }
}

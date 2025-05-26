using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class MigrationWithTeam9242 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameTag_Games_GamesGameId",
                table: "GameTag");

            migrationBuilder.DropForeignKey(
                name: "FK_GameTag_Tags_TagsTagId",
                table: "GameTag");

            migrationBuilder.DropForeignKey(
                name: "FK_Items_Games_CorrespondingGameId",
                table: "Items");

            migrationBuilder.DropForeignKey(
                name: "FK_ItemTrades_Games_GameOfTradeId",
                table: "ItemTrades");

            migrationBuilder.DropForeignKey(
                name: "FK_StoreTransactions_Games_GameId",
                table: "StoreTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInventories_Games_GameId",
                table: "UserInventories");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersGames_Games_GameId",
                table: "UsersGames");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropIndex(
                name: "IX_UsersGames_GameId",
                table: "UsersGames");

            migrationBuilder.DropIndex(
                name: "IX_UserInventories_GameId",
                table: "UserInventories");

            migrationBuilder.DropIndex(
                name: "IX_StoreTransactions_GameId",
                table: "StoreTransactions");

            migrationBuilder.DropIndex(
                name: "IX_ItemTrades_GameOfTradeId",
                table: "ItemTrades");

            migrationBuilder.DropIndex(
                name: "IX_Items_CorrespondingGameId",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_GameTag_TagsTagId",
                table: "GameTag");

            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeveloper",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLogin",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<byte[]>(
                name: "ProfilePicture",
                table: "Users",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.CreateTable(
                name: "Achievements",
                columns: table => new
                {
                    achievement_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    achievement_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    achievement_type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    points = table.Column<int>(type: "int", nullable: false),
                    icon_url = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Achievements", x => x.achievement_id);
                });

            migrationBuilder.CreateTable(
                name: "ChatConversations",
                columns: table => new
                {
                    conversation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user1_id = table.Column<int>(type: "int", nullable: false),
                    user2_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatConversations", x => x.conversation_id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    conversation_id = table.Column<int>(type: "int", nullable: false),
                    sender_id = table.Column<int>(type: "int", nullable: false),
                    message_content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    message_format = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    timestamp = table.Column<long>(type: "bigint", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.message_id);
                });

            migrationBuilder.CreateTable(
                name: "Collections",
                columns: table => new
                {
                    collection_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    cover_picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    is_public = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "CAST(GETDATE() AS DATE)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Collections", x => x.collection_id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    feature_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    value = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    equipped = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.feature_id);
                });

            migrationBuilder.CreateTable(
                name: "ForumComments",
                columns: table => new
                {
                    comment_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    score = table.Column<int>(type: "int", nullable: false),
                    creation_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    author_id = table.Column<int>(type: "int", nullable: false),
                    post_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumComments", x => x.comment_id);
                });

            migrationBuilder.CreateTable(
                name: "ForumPosts",
                columns: table => new
                {
                    post_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    score = table.Column<int>(type: "int", nullable: false),
                    creation_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    author_id = table.Column<int>(type: "int", nullable: false),
                    game_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForumPosts", x => x.post_id);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SenderUsername = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SenderEmail = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SenderProfilePhotoPath = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReceiverUsername = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => x.RequestId);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    FriendshipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User1Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    User2Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.FriendshipId);
                });

            migrationBuilder.CreateTable(
                name: "Friendships",
                columns: table => new
                {
                    friendship_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    friend_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friendships", x => x.friendship_id);
                });

            migrationBuilder.CreateTable(
                name: "NewsComments",
                schema: "dbo",
                columns: table => new
                {
                    cid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    postId = table.Column<int>(type: "int", nullable: false),
                    authorId = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    uploadDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsComments", x => x.cid);
                });

            migrationBuilder.CreateTable(
                name: "NewsPosts",
                schema: "dbo",
                columns: table => new
                {
                    pid = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    authorId = table.Column<int>(type: "int", nullable: false),
                    content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    uploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    nrLikes = table.Column<int>(type: "int", nullable: false),
                    nrDislikes = table.Column<int>(type: "int", nullable: false),
                    nrComments = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsPosts", x => x.pid);
                });

            migrationBuilder.CreateTable(
                name: "NewsRatings",
                schema: "dbo",
                columns: table => new
                {
                    postId = table.Column<int>(type: "int", nullable: false),
                    authorId = table.Column<int>(type: "int", nullable: false),
                    ratingType = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsRatings", x => new { x.postId, x.authorId });
                });

            migrationBuilder.CreateTable(
                name: "OwnedGames",
                columns: table => new
                {
                    game_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    cover_picture = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedGames", x => x.game_id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetCodes",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    reset_code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expiration_time = table.Column<DateTime>(type: "datetime2", nullable: false),
                    used = table.Column<bool>(type: "bit", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetCodes", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ReviewsUsers",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProfilePicture = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewsUsers", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WalletBalance = table.Column<float>(type: "real", nullable: false),
                    PointsBalance = table.Column<float>(type: "real", nullable: false),
                    UserRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "UserDislikedComment",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false),
                    comment_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDislikedComment", x => new { x.userId, x.comment_id });
                });

            migrationBuilder.CreateTable(
                name: "UserDislikedPost",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false),
                    post_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDislikedPost", x => new { x.userId, x.post_id });
                });

            migrationBuilder.CreateTable(
                name: "UserLikedComment",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false),
                    comment_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikedComment", x => new { x.userId, x.comment_id });
                });

            migrationBuilder.CreateTable(
                name: "UserLikedPost",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false),
                    post_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLikedPost", x => new { x.userId, x.post_id });
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    profile_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    profile_picture = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    last_modified = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.profile_id);
                });

            migrationBuilder.CreateTable(
                name: "UserSessions",
                columns: table => new
                {
                    session_id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    expires_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSessions", x => x.session_id);
                });

            migrationBuilder.CreateTable(
                name: "Wallet",
                columns: table => new
                {
                    wallet_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    money_for_games = table.Column<decimal>(type: "decimal(10,2)", nullable: false, defaultValue: 0m),
                    points = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallet", x => x.wallet_id);
                });

            migrationBuilder.CreateTable(
                name: "Feature_User",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    feature_id = table.Column<int>(type: "int", nullable: false),
                    equipped = table.Column<bool>(type: "bit", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feature_User", x => new { x.user_id, x.feature_id });
                    table.ForeignKey(
                        name: "FK_Feature_User_Features_feature_id",
                        column: x => x.feature_id,
                        principalTable: "Features",
                        principalColumn: "feature_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OwnedGames_Collection",
                columns: table => new
                {
                    collection_id = table.Column<int>(type: "int", nullable: false),
                    game_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnedGames_Collection", x => new { x.collection_id, x.game_id });
                    table.ForeignKey(
                        name: "FK_OwnedGames_Collection_Collections_collection_id",
                        column: x => x.collection_id,
                        principalTable: "Collections",
                        principalColumn: "collection_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OwnedGames_Collection_OwnedGames_game_id",
                        column: x => x.game_id,
                        principalTable: "OwnedGames",
                        principalColumn: "game_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    ReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRecommended = table.Column<bool>(type: "bit", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(3,1)", nullable: false),
                    HelpfulVotes = table.Column<int>(type: "int", nullable: false),
                    FunnyVotes = table.Column<int>(type: "int", nullable: false),
                    HoursPlayed = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.ReviewId);
                    table.ForeignKey(
                        name: "FK_Reviews_ReviewsUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ReviewsUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SoldGames",
                columns: table => new
                {
                    sold_game_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<int>(type: "int", nullable: false),
                    game_id = table.Column<int>(type: "int", nullable: true),
                    sold_date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoldGames", x => x.sold_game_id);
                    table.ForeignKey(
                        name: "FK_SoldGames_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserAchievements",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false),
                    achievement_id = table.Column<int>(type: "int", nullable: false),
                    unlocked_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAchievements", x => new { x.user_id, x.achievement_id });
                    table.ForeignKey(
                        name: "FK_UserAchievements_Achievements_achievement_id",
                        column: x => x.achievement_id,
                        principalTable: "Achievements",
                        principalColumn: "achievement_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAchievements_User_user_id",
                        column: x => x.user_id,
                        principalTable: "User",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "CreatedAt", "IsDeveloper", "LastLogin", "Password", "ProfilePicture" },
                values: new object[] { new DateTime(2025, 5, 26, 19, 41, 51, 933, DateTimeKind.Local).AddTicks(5037), false, new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(1912), "secret", new byte[0] });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "IsDeveloper", "LastLogin", "Password", "ProfilePicture" },
                values: new object[] { new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3555), false, new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3577), "secret", new byte[0] });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "IsDeveloper", "LastLogin", "Password", "ProfilePicture" },
                values: new object[] { new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3584), false, new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3587), "secret", new byte[0] });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "IsDeveloper", "LastLogin", "Password", "ProfilePicture" },
                values: new object[] { new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3591), false, new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3594), "secret", new byte[0] });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "CreatedAt", "IsDeveloper", "LastLogin", "Password", "ProfilePicture" },
                values: new object[] { new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3600), false, new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3603), "secret", new byte[0] });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                columns: new[] { "CreatedAt", "IsDeveloper", "LastLogin", "Password", "ProfilePicture" },
                values: new object[] { new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3668), false, new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3671), "secret", new byte[0] });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                columns: new[] { "CreatedAt", "IsDeveloper", "LastLogin", "Password", "ProfilePicture" },
                values: new object[] { new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3677), false, new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3679), "secret", new byte[0] });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                columns: new[] { "CreatedAt", "IsDeveloper", "LastLogin", "Password", "ProfilePicture" },
                values: new object[] { new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3684), false, new DateTime(2025, 5, 26, 19, 41, 51, 940, DateTimeKind.Local).AddTicks(3687), "secret", new byte[0] });

            migrationBuilder.CreateIndex(
                name: "IX_Collections_user_id",
                table: "Collections",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feature_User_feature_id",
                table: "Feature_User",
                column: "feature_id");

            migrationBuilder.CreateIndex(
                name: "UQ_SenderReceiver",
                table: "FriendRequests",
                columns: new[] { "SenderUsername", "ReceiverUsername" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_FriendId",
                table: "Friendships",
                column: "friend_id");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_UserId",
                table: "Friendships",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "UQ_Friendship",
                table: "Friendships",
                columns: new[] { "user_id", "friend_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OwnedGames_UserId",
                table: "OwnedGames",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_OwnedGames_Collection_game_id",
                table: "OwnedGames_Collection",
                column: "game_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_UserId",
                table: "Reviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SoldGames_user_id",
                table: "SoldGames",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAchievements_achievement_id",
                table: "UserAchievements",
                column: "achievement_id");

            migrationBuilder.CreateIndex(
                name: "IX_Wallet_user_id",
                table: "Wallet",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatConversations");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Feature_User");

            migrationBuilder.DropTable(
                name: "ForumComments");

            migrationBuilder.DropTable(
                name: "ForumPosts");

            migrationBuilder.DropTable(
                name: "FriendRequests");

            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "Friendships");

            migrationBuilder.DropTable(
                name: "NewsComments",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NewsPosts",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "NewsRatings",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "OwnedGames_Collection");

            migrationBuilder.DropTable(
                name: "PasswordResetCodes");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "SoldGames");

            migrationBuilder.DropTable(
                name: "UserAchievements");

            migrationBuilder.DropTable(
                name: "UserDislikedComment");

            migrationBuilder.DropTable(
                name: "UserDislikedPost");

            migrationBuilder.DropTable(
                name: "UserLikedComment");

            migrationBuilder.DropTable(
                name: "UserLikedPost");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "UserSessions");

            migrationBuilder.DropTable(
                name: "Wallet");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "Collections");

            migrationBuilder.DropTable(
                name: "OwnedGames");

            migrationBuilder.DropTable(
                name: "ReviewsUsers");

            migrationBuilder.DropTable(
                name: "Achievements");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsDeveloper",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLogin",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PublisherUserId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GameplayPath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinimumRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumberOfRecentPurchases = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Rating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RecommendedRequirements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RejectMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrailerPath = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_Games_GameStatus_StatusId",
                        column: x => x.StatusId,
                        principalTable: "GameStatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_Users_PublisherUserId",
                        column: x => x.PublisherUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "GameId", "Description", "Discount", "GameplayPath", "ImagePath", "MinimumRequirements", "Name", "NumberOfRecentPurchases", "Price", "PublisherUserId", "Rating", "RecommendedRequirements", "RejectMessage", "StatusId", "TrailerPath" },
                values: new object[,]
                {
                    { 1, "A rogue-like third-person shooter where players fight through hordes of monsters to escape an alien planet.", 0.20m, "https://www.youtube.com/watch?v=Cwk3qmD28CE", "https://upload.wikimedia.org/wikipedia/en/c/c1/Risk_of_Rain_2.jpg", "4GB RAM, 2.5GHz Processor, GTX 580", "Risk of Rain 2", 0, 24.99m, 1, 4.2m, "8GB RAM, 3.0GHz Processor, GTX 680", "Minimum requirements are too high", 2, "https://www.youtube.com/watch?v=pJ-aR--gScM" },
                    { 2, "A multiplayer horror game where survivors must evade a killer.", 0.40m, "https://www.youtube.com/watch?v=3wUHKO0ieyY", "https://pbs.twimg.com/media/FOEzJiXX0AcxBTi.jpg", "8GB RAM, i3-4170, GTX 760", "Dead by Daylight", 0, 19.99m, 1, 4.8m, "16GB RAM, i5-6500, GTX 1060", null, 0, "https://www.youtube.com/watch?v=JGhIXLO3ul8" },
                    { 3, "A tactical first-person shooter featuring team-based gameplay.", 0.50m, "https://www.youtube.com/watch?v=P22HqM9w500", "https://sm.ign.com/ign_nordic/cover/c/counter-st/counter-strike-2_jc2d.jpg", "8GB RAM, i5-2500K, GTX 660", "Counter-Strike 2", 0, 20.99m, 1, 4.9m, "16GB RAM, i7-7700K, GTX 1060", null, 1, "https://www.youtube.com/watch?v=c80dVYcL69E" },
                    { 4, "A story-driven first-person shooter that revolutionized the genre.", 0.60m, "https://www.youtube.com/watch?v=jElU1mD8JnI", "https://media.moddb.com/images/mods/1/47/46951/d1jhx20-dc797b78-5feb-4005-b206-.1.jpg", "512MB RAM, 1.7GHz Processor, DirectX 8 GPU", "Half-Life 2", 0, 9.99m, 1, 4.1m, "1GB RAM, 3.0GHz Processor, DirectX 9 GPU", null, 1, "https://www.youtube.com/watch?v=UKA7JkV51Jw" },
                    { 5, "A classic platformer adventure with iconic characters and worlds.", 0.70m, "https://www.youtube.com/watch?v=rLl9XBg7wSs", "https://play-lh.googleusercontent.com/3ZKfMRp_QrdN-LzsZTbXdXBH-LS1iykSg9ikNq_8T2ppc92ltNbFxS-tORxw2-6kGA", "N/A", "Mario", 0, 59.99m, 1, 5.0m, "N/A", null, 1, "https://www.youtube.com/watch?v=TnGl01FkMMo" },
                    { 6, "An epic adventure game where heroes save the kingdom of Hyrule.", 0.30m, "https://www.youtube.com/watch?v=wW7jkBJ_yK0", "https://m.media-amazon.com/images/I/71oHNyzdN1L.jpg", "N/A", "The Legend of Zelda", 0, 59.99m, 1, 4.5m, "N/A", null, 1, "https://www.youtube.com/watch?v=_X2h3SF7gd4" },
                    { 7, "A puzzle game where you change the rules to solve challenges.", 0.20m, "https://www.youtube.com/watch?v=dAiX8s-Eu7w", "https://is5-ssl.mzstatic.com/image/thumb/Purple113/v4/9e/30/61/9e3061a5-b2f0-87ad-9e90-563f37729be5/source/256x256bb.jpg", "2GB RAM, 1.0GHz Processor", "Baba Is You", 0, 14.99m, 2, 3.9m, "4GB RAM, 2.0GHz Processor", null, 0, "https://www.youtube.com/watch?v=z3_yA4HTJfs" },
                    { 8, "A mind-bending puzzle-platformer with a dark sense of humor.", 0.10m, "https://www.youtube.com/watch?v=ts-j0nFf2e0", "https://cdn2.steamgriddb.com/icon_thumb/0994c8d1d6bc62cc56e9112d2303266b.png", "2GB RAM, 1.7GHz Processor, DirectX 9 GPU", "Portal 2", 0, 9.99m, 2, 4.2m, "4GB RAM, 3.0GHz Processor, GTX 760", null, 0, "https://www.youtube.com/watch?v=tax4e4hBBZc" },
                    { 9, "An exploration-based game where you unravel cosmic mysteries.", 0.15m, "https://www.youtube.com/watch?v=huL_TawYrMs", "https://images.nintendolife.com/62a79995ed766/outer-wilds-echoes-of-the-eye-cover.cover_large.jpg", "6GB RAM, i5-2300, GTX 560", "Outer Wilds", 0, 24.99m, 2, 4.8m, "8GB RAM, i7-6700, GTX 970", null, 0, "https://www.youtube.com/watch?v=d9u6KYVq5kw" },
                    { 10, "A rogue-like dungeon crawler where you defy the god of the dead.", 0.20m, "https://www.youtube.com/watch?v=4fVO0qUBe4E", "https://image.api.playstation.com/vulcan/ap/rnd/202104/0517/9AcM3vy5t77zPiJyKHwRfnNT.png", "4GB RAM, Dual Core 2.4GHz, Intel HD 5000", "Hades", 0, 24.99m, 2, 4.0m, "8GB RAM, Dual Core 3.0GHz, GTX 760", null, 0, "https://www.youtube.com/watch?v=91sW0DMkZzI" },
                    { 11, "A deck-building rogue-like where strategy is key to survival.", 0.25m, "https://www.youtube.com/watch?v=JO3EIPtw-4I", "https://image.api.playstation.com/cdn/EP3717/CUSA15338_00/Sn5xbNutqfQdWYIjbeCIN0bwTJOV7UPG.png", "2GB RAM, 2.0GHz Processor", "Slay the Spire", 0, 19.99m, 2, 4.0m, "4GB RAM, 3.0GHz Processor", null, 0, "https://www.youtube.com/watch?v=75qT5KOs-Ew" },
                    { 12, "A platformer about climbing a mountain and facing inner demons.", 0.30m, "https://www.youtube.com/watch?v=FfRjHZWSYqY", "https://images.nintendolife.com/ef02c2e24c59e/celeste-cover.cover_large.jpg", "2GB RAM, 2.0GHz Processor", "Celeste", 0, 19.99m, 2, 3.8m, "4GB RAM, 2.4GHz Processor", null, 0, "https://www.youtube.com/watch?v=iofYDsP2vhQ" },
                    { 13, "An action-adventure game set in a beautifully animated underground world.", 0.35m, "https://www.youtube.com/watch?v=UAO2urG23S4", "https://image.api.playstation.com/cdn/EP1805/CUSA13285_00/DmwPWlU0468FbsjrtI92FhQz1xBYMoog.png", "4GB RAM, 2.0GHz Processor", "Hollow Knight", 0, 14.99m, 2, 4.1m, "8GB RAM, 3.2GHz Processor", null, 0, "https://www.youtube.com/watch?v=UAO2urG23S4" },
                    { 14, "A farming simulator RPG where you build a life in the countryside.", 0.20m, "https://www.youtube.com/watch?v=ot7uXNQskhs", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQHWjybGuWhdyJqjmtziGvtHvCnQf23yY0R6g&s", "2GB RAM, 2.0GHz Processor", "Stardew Valley", 0, 14.99m, 2, 4.1m, "4GB RAM, 3.0GHz Processor", null, 0, "https://www.youtube.com/watch?v=ot7uXNQskhs" },
                    { 15, "A sandbox game that lets you build and explore infinite worlds.", 0.14m, "https://www.youtube.com/watch?v=ANgI2o_Jinc", "https://cdn2.steamgriddb.com/icon/f0b57183da91a7972b2b3c06b0db5542/32/512x512.png", "4GB RAM, Intel HD 4000", "Minecraft", 1420, 29.99m, 2, 4.8m, "8GB RAM, GTX 1060", null, 1, "https://www.youtube.com/watch?v=MmB9b5njVbA" },
                    { 16, "A survival game in a dark and whimsical world filled with strange creatures.", 0.25m, "https://www.youtube.com/watch?v=htXgxyLpPMg", "https://image.api.playstation.com/cdn/EP2107/CUSA00327_00/i5qwqMWJj33IIr2m9TM29GQNnFCi4ZqI.png?w=440", "1.7GHz Processor, 1GB RAM", "Don't Starve", 0, 9.99m, 2, 3.6m, "2.0GHz Processor, 2GB RAM", null, 0, "https://www.youtube.com/watch?v=ochPlhMFk84" },
                    { 17, "A classic run and gun game with hand-drawn animations and tough bosses.", 0.40m, "https://www.youtube.com/watch?v=DNIMD8ZpMSQ", "https://upload.wikimedia.org/wikipedia/en/thumb/e/eb/Cuphead_%28artwork%29.png/250px-Cuphead_%28artwork%29.png", "3GB RAM, Intel Core2 Duo E8400", "Cuphead", 0, 19.99m, 2, 4.8m, "4GB RAM, i3-3240", null, 0, "https://www.youtube.com/watch?v=NN-9SQXoi50" },
                    { 18, "A black-and-white puzzle platformer with a haunting atmosphere.", 0.30m, "https://www.youtube.com/watch?v=dYeuLZY7fZk", "https://image.api.playstation.com/cdn/EP2054/CUSA01369_00/W45kellY9yrwSDpmQEL9tFqZQW7N4FEz.png?w=440", "512MB RAM, 1.5GHz Processor", "Limbo", 0, 9.99m, 2, 4.6m, "2GB RAM, 2.0GHz Processor", null, 0, "https://www.youtube.com/watch?v=Y4HSyVXKYz8" },
                    { 19, "A futuristic open-world RPG where you explore the neon-lit streets of Nightcity.", 0.25m, "https://www.youtube.com/watch?v=8X2kIfS6fb8", "https://upload.wikimedia.org/wikipedia/en/9/9f/Cyberpunk_2077_box_art.jpg", "Intel i5-3570K, 8GB RAM, GTX 780", "Cyberstrike 2077", 950, 59.99m, 3, 4.2m, "Intel i7-4790, 12GB RAM, GTX 1060", null, 1, "https://www.youtube.com/watch?v=FknHjl7eQ6o" },
                    { 20, "Immerse yourself in the Viking age in this brutal and breathtaking action RPG.", 0.10m, "https://www.youtube.com/watch?v=gncB1_e9n8E", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQDtZKyDW9Jrnh8ix-Y38qG5fddbUgYEW7yxA&s", "Intel i5-4460, 8GB RAM, GTX 960", "Shadow of Valhalla", 780, 44.99m, 3, 4.5m, "Intel i7-6700K, 16GB RAM, GTX 1080", null, 1, "https://www.youtube.com/watch?v=ssrNcwxALS4" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersGames_GameId",
                table: "UsersGames",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInventories_GameId",
                table: "UserInventories",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreTransactions_GameId",
                table: "StoreTransactions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTrades_GameOfTradeId",
                table: "ItemTrades",
                column: "GameOfTradeId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CorrespondingGameId",
                table: "Items",
                column: "CorrespondingGameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameTag_TagsTagId",
                table: "GameTag",
                column: "TagsTagId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_PublisherUserId",
                table: "Games",
                column: "PublisherUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_StatusId",
                table: "Games",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameTag_Games_GamesGameId",
                table: "GameTag",
                column: "GamesGameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameTag_Tags_TagsTagId",
                table: "GameTag",
                column: "TagsTagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Games_CorrespondingGameId",
                table: "Items",
                column: "CorrespondingGameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemTrades_Games_GameOfTradeId",
                table: "ItemTrades",
                column: "GameOfTradeId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StoreTransactions_Games_GameId",
                table: "StoreTransactions",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInventories_Games_GameId",
                table: "UserInventories",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersGames_Games_GameId",
                table: "UsersGames",
                column: "GameId",
                principalTable: "Games",
                principalColumn: "GameId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

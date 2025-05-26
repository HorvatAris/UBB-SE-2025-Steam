using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BusinessLayer.Migrations
{
    /// <inheritdoc />
    public partial class MainMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

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
                name: "Features",
                columns: table => new
                {
                    feature_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    value = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    source = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "PointsOffers",
                columns: table => new
                {
                    offer_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    numberOfPoints = table.Column<int>(type: "int", nullable: false),
                    value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointsOffers", x => x.offer_id);
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
                name: "Users",
                columns: table => new
                {
                    user_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hashed_password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    developer = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    last_login = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.user_id);
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
                name: "ChatMessages",
                columns: table => new
                {
                    message_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    conversation_id = table.Column<int>(type: "int", nullable: false),
                    sender_id = table.Column<int>(type: "int", nullable: false),
                    timestamp = table.Column<long>(type: "bigint", nullable: false),
                    message_format = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    message_content = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.message_id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatConversations_conversation_id",
                        column: x => x.conversation_id,
                        principalTable: "ChatConversations",
                        principalColumn: "conversation_id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_Collections_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
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
                    table.ForeignKey(
                        name: "FK_OwnedGames_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
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
                        name: "FK_SoldGames_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
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
                        name: "FK_UserAchievements_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "user_id",
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

            // INSERT SEED DATA - Following the pattern from the second project
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "user_id", "email", "username", "hashed_password", "developer", "created_at", "last_login" },
                values: new object[,]
                {
                    { 1, "alice@example.com", "AliceGamer", "hashed_password_1", true, new DateTime(2025, 3, 20, 14, 25, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 20, 14, 25, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "bob@example.com", "BobTheBuilder", "hashed_password_2", false, new DateTime(2025, 3, 21, 10, 12, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 21, 10, 12, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "charlie@example.com", "CharlieX", "hashed_password_3", false, new DateTime(2025, 3, 22, 18, 45, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 22, 18, 45, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "diana@example.com", "DianaRocks", "hashed_password_4", false, new DateTime(2025, 3, 19, 22, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 19, 22, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "eve@example.com", "Eve99", "hashed_password_5", true, new DateTime(2025, 3, 23, 8, 5, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 23, 8, 5, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "frank@example.com", "FrankTheTank", "hashed_password_6", false, new DateTime(2025, 3, 24, 16, 20, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 24, 16, 20, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "grace@example.com", "GraceSpeed", "hashed_password_7", false, new DateTime(2025, 3, 25, 11, 40, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 25, 11, 40, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "harry@example.com", "HarryWizard", "hashed_password_8", false, new DateTime(2025, 3, 20, 20, 15, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 20, 20, 15, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "ivy@example.com", "IvyNinja", "hashed_password_9", false, new DateTime(2025, 3, 22, 9, 30, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 22, 9, 30, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "jack@example.com", "JackHacks", "hashed_password_10", true, new DateTime(2025, 3, 24, 23, 55, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 3, 24, 23, 55, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "PointsOffers",
                columns: new[] { "offer_id", "numberOfPoints", "value" },
                values: new object[,]
                {
                    { 1, 5, 2 },
                    { 2, 25, 8 },
                    { 3, 50, 15 },
                    { 4, 100, 20 },
                    { 5, 500, 50 }
                });

            migrationBuilder.InsertData(
                table: "Features",
                columns: new[] { "feature_id", "name", "value", "description", "type", "source", "equipped" },
                values: new object[,]
                {
                    { 1, "Black Hat", 2000, "An elegant hat", "hat", "Assets/Features/Hats/black-hat.png", false },
                    { 2, "Pufu", 10, "Cute doggo", "pet", "Assets/Features/Pets/dog.png", false },
                    { 3, "Kitty", 8, "Cute cat", "pet", "Assets/Features/Pets/cat.png", false },
                    { 4, "Frame", 5, "Violet frame", "frame", "Assets/Features/Frames/frame1.png", false },
                    { 5, "Love Emoji", 7, "lalal", "emoji", "Assets/Features/Emojis/love.png", false },
                    { 6, "Violet Background", 7, "Violet Background", "background", "Assets/Features/Backgrounds/violet.jpg", false }
                });

            migrationBuilder.InsertData(
                table: "ReviewsUsers",
                columns: new[] { "UserId", "Name", "ProfilePicture" },
                values: new object[,]
                {
                    { 2, "Sam Carter", null },
                    { 3, "Taylor Kim", null }
                });

            migrationBuilder.InsertData(
                table: "Achievements",
                columns: new[] { "achievement_id", "achievement_name", "description", "achievement_type", "points", "icon_url" },
                values: new object[,]
                {
                    { 1, "FRIENDSHIP1", "You made a friend, you get a point", "Friendships", 1, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 2, "FRIENDSHIP2", "You made 5 friends, you get 3 points", "Friendships", 3, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 3, "FRIENDSHIP3", "You made 10 friends, you get 5 points", "Friendships", 5, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 4, "FRIENDSHIP4", "You made 50 friends, you get 10 points", "Friendships", 10, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 5, "FRIENDSHIP5", "You made 100 friends, you get 15 points", "Friendships", 15, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 6, "OWNEDGAMES1", "You own 1 game, you get 1 point", "Owned Games", 1, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 7, "OWNEDGAMES2", "You own 5 games, you get 3 points", "Owned Games", 3, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 8, "OWNEDGAMES3", "You own 10 games, you get 5 points", "Owned Games", 5, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 9, "OWNEDGAMES4", "You own 50 games, you get 10 points", "Owned Games", 10, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 10, "SOLDGAMES1", "You sold 1 game, you get 1 point", "Sold Games", 1, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 11, "SOLDGAMES2", "You sold 5 games, you get 3 points", "Sold Games", 3, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 12, "SOLDGAMES3", "You sold 10 games, you get 5 points", "Sold Games", 5, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 13, "SOLDGAMES4", "You sold 50 games, you get 10 points", "Sold Games", 10, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 14, "REVIEW1", "You gave 1 review, you get 1 point", "Number of Reviews Given", 1, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 15, "REVIEW2", "You gave 5 reviews, you get 3 points", "Number of Reviews Given", 3, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 16, "REVIEW3", "You gave 10 reviews, you get 5 points", "Number of Reviews Given", 5, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 17, "REVIEW4", "You gave 50 reviews, you get 10 points", "Number of Reviews Given", 10, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 18, "REVIEWR1", "You got 1 review, you get 1 point", "Number of Reviews Received", 1, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 19, "REVIEWR2", "You got 5 reviews, you get 3 points", "Number of Reviews Received", 3, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 20, "REVIEWR3", "You got 10 reviews, you get 5 points", "Number of Reviews Received", 5, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 21, "REVIEWR4", "You got 50 reviews, you get 10 points", "Number of Reviews Received", 10, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 22, "DEVELOPER", "You are a developer, you get 10 points", "Developer", 10, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 23, "ACTIVITY1", "You have been active for 1 year, you get 1 point", "Years of Activity", 1, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 24, "ACTIVITY2", "You have been active for 2 years, you get 3 points", "Years of Activity", 3, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 25, "ACTIVITY3", "You have been active for 3 years, you get 5 points", "Years of Activity", 5, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 26, "ACTIVITY4", "You have been active for 4 years, you get 10 points", "Years of Activity", 10, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 27, "POSTS1", "You have made 1 post, you get 1 point", "Number of Posts", 1, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 28, "POSTS2", "You have made 5 posts, you get 3 points", "Number of Posts", 3, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 29, "POSTS3", "You have made 10 posts, you get 5 points", "Number of Posts", 5, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                    { 30, "POSTS4", "You have made 50 posts, you get 10 points", "Number of Posts", 10, "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" }
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "profile_id", "user_id", "profile_picture", "bio", "last_modified" },
                values: new object[,]
                {
                    { 1, 1, "ms-appx:///Assets/Collections/image.jpg", "Gaming enthusiast and software developer", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 2, "ms-appx:///Assets/download.jpg", "Game developer and tech lover", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 3, "ms-appx:///Assets/download.jpg", "Casual gamer and streamer", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, 4, "ms-appx:///Assets/Collections/image.jpg", "Casual gamer and streamer", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, 5, "ms-appx:///Assets/download.jpg", "Casual gamer and streamer", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, 6, "ms-appx:///Assets/default_picture.jpg", "Casual gamer and streamer", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, 7, "ms-appx:///Assets/default_picture.jpg", "Casual gamer and streamer", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, 8, "ms-appx:///Assets/default_picture.jpg", "Casual gamer and streamer", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, 9, "ms-appx:///Assets/default_picture.jpg", "Casual gamer and streamer", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, 10, "ms-appx:///Assets/default_picture.jpg", "Casual gamer and streamer", new DateTime(2025, 5, 26, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Wallet",
                columns: new[] { "wallet_id", "user_id", "points", "money_for_games" },
                values: new object[,]
                {
                    { 1, 1, 10, 200m },
                    { 2, 2, 10, 200m },
                    { 3, 3, 10, 200m },
                    { 4, 4, 10, 200m },
                    { 5, 5, 10, 200m },
                    { 6, 6, 10, 200m },
                    { 7, 7, 10, 200m },
                    { 8, 8, 10, 200m },
                    { 9, 9, 10, 200m },
                    { 10, 10, 10, 200m }
                });

            migrationBuilder.InsertData(
                table: "Collections",
                columns: new[] { "collection_id", "user_id", "name", "cover_picture", "is_public", "created_at" },
                values: new object[,]
                {
                    { 1, 1, "All Owned Games", "/Assets/Collections/allgames.jpg", true, new DateOnly(2022, 2, 21) },
                    { 2, 1, "Sports", "/Assets/Collections/sports.jpg", true, new DateOnly(2023, 3, 21) },
                    { 3, 1, "Chill Games", "/Assets/Collections/chill.jpg", true, new DateOnly(2024, 3, 21) },
                    { 4, 1, "X-Mas", "/Assets/Collections/xmas.jpg", false, new DateOnly(2025, 2, 21) },
                    { 5, 2, "Shooters", "/Assets/Collections/shooters.jpg", true, new DateOnly(2025, 3, 21) },
                    { 6, 2, "Pets", "/Assets/Collections/pets.jpg", false, new DateOnly(2025, 1, 21) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_conversation_id",
                table: "ChatMessages",
                column: "conversation_id");

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
                name: "IX_OwnedGames_user_id",
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
                name: "PointsOffers");

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
                name: "ChatConversations");

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
                name: "Users");
        }
    }
}
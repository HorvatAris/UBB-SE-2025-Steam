using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

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
                    UserId = table.Column<int>(type: "int", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    value = table.Column<int>(type: "int", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    source = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Equipped = table.Column<bool>(type: "bit", nullable: false)
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

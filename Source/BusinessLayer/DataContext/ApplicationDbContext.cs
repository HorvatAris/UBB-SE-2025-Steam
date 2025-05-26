﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;
using Google;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.DataContext
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Define DbSets here
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<ChatConversation> ChatConversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<OwnedGame> OwnedGames { get; set; }
        public DbSet<SessionDetails> UserSessions { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewsUser> ReviewsUsers { get; set; }
        public DbSet<Post> NewsPosts { get; set; }
        public DbSet<Comment> NewsComments { get; set; }
        public DbSet<PostRatingType> NewsPostRatingTypes { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<FriendEntity> FriendsTable { get; set; } // Delete this once the relationship functionalities are sorted out
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
        public DbSet<ForumPost> ForumPosts { get; set; }
        public DbSet<ForumComment> ForumComments { get; set; }
        internal DbSet<UserLikedPost> UserLikedPosts { get; set; }
        internal DbSet<UserDislikedPost> UserDislikedPosts { get; set; }
        internal DbSet<UserLikedComment> UserLikedComments { get; set; }
        internal DbSet<UserDislikedComment> UserDislikedComments { get; set; }
        public DbSet<Feature> Features { get; set; }
        public DbSet<FeatureUser> FeatureUsers { get; set; }

        public DbSet<CollectionGame> CollectionGames { get; set; }

        public DbSet<SoldGame> SoldGames { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Collection>()
                .ToTable(tb => tb.HasTrigger("SomeTrigger"));
            modelBuilder.Entity<OwnedGame>()
                .ToTable(tb => tb.HasTrigger("SomeTrigger"));
            // Exclude non-entity models (no corresponding tables)
            modelBuilder.Ignore<Friend>();
            modelBuilder.Ignore<Game>();
            modelBuilder.Ignore<PostDisplay>();
            modelBuilder.Ignore<AchievementWithStatus>();
            modelBuilder.Ignore<AchievementUnlockedData>();
            modelBuilder.Ignore<CommentDisplay>();

            // Configure entities here

            // -- ReviewsUser mapping ---------------------------------------------------
            modelBuilder.Entity<ReviewsUser>(entity =>
            {
                entity.ToTable("ReviewsUsers");

                entity.HasKey(ru => ru.UserId);

                entity.Property(ru => ru.UserId)
                    .HasColumnName("UserId");

                entity.Property(ru => ru.Name)
                    .HasColumnName("Name")
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(ru => ru.ProfilePicture)
                    .HasColumnName("ProfilePicture");

                // Navigation property configuration
                entity.HasMany(ru => ru.Reviews)
                      .WithOne()
                      .HasForeignKey(r => r.UserIdentifier)
                      .HasPrincipalKey(ru => ru.UserId);
            });

            // -- SoldGame mapping --------------------------------------------------------
            modelBuilder.Entity<SoldGame>(entity =>
            {
                entity.ToTable("SoldGames");

                entity.HasKey(sg => sg.SoldGameId);

                entity.Property(sg => sg.SoldGameId)
                      .HasColumnName("sold_game_id")
                      .ValueGeneratedOnAdd();

                entity.Property(sg => sg.UserId)
                      .HasColumnName("user_id")
                      .IsRequired();

                entity.Property(sg => sg.GameId)
                      .HasColumnName("game_id");

                entity.Property(sg => sg.SoldDate)
                      .HasColumnName("sold_date");

                entity.HasOne(e => e.User)
                        .WithMany(u => u.SoldGames)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
            });

            // -- CollectionGame mapping ------------------------------------------------
            modelBuilder.Entity<CollectionGame>(entity =>
            {
                entity.ToTable("OwnedGames_Collection");
                entity.HasKey(cg => new { cg.CollectionId, cg.GameId });

                entity.Property(cg => cg.CollectionId)
                    .HasColumnName("collection_id");
                entity.Property(cg => cg.GameId)
                    .HasColumnName("game_id");

                entity.HasOne(cg => cg.Collection)
                      .WithMany(c => c.CollectionGames)
                      .HasForeignKey(cg => cg.CollectionId);

                entity.HasOne(cg => cg.OwnedGame)
                      .WithMany(og => og.CollectionGames)
                      .HasForeignKey(cg => cg.GameId);
            });

            // -- Feature mapping -------------------------------------------------------
            modelBuilder.Entity<Feature>(entity =>
            {
                entity.ToTable("Features");
                entity.HasKey(f => f.FeatureId);
                entity.Property(f => f.FeatureId)
                      .HasColumnName("feature_id")
                      .ValueGeneratedOnAdd();
                entity.Property(f => f.Name)
                      .HasColumnName("name")
                      .IsRequired();
                entity.Property(f => f.Value)
                      .HasColumnName("value")
                      .IsRequired();
                entity.Property(f => f.Description)
                      .HasColumnName("description");
                entity.Property(f => f.Type)
                      .HasColumnName("type")
                      .IsRequired();
                entity.Property(f => f.Source)
                      .HasColumnName("source");
                entity.Property(f => f.Equipped)
                     .HasColumnName("equipped");
            });

            // -- FeatureUser mapping ---------------------------------------------------
            modelBuilder.Entity<FeatureUser>(entity =>
            {
                entity.ToTable("Feature_User");
                entity.HasKey(fu => new { fu.UserId, fu.FeatureId });
                entity.Property(fu => fu.UserId)
                      .HasColumnName("user_id");
                entity.Property(fu => fu.FeatureId)
                      .HasColumnName("feature_id");
                entity.Property(fu => fu.Equipped)
                      .HasColumnName("equipped")
                      .HasDefaultValue(false);

                entity.HasOne(fu => fu.Feature)
                    .WithMany()
                    .HasForeignKey(fu => fu.FeatureId);
            });

                // -- ForumPost mapping ----------------------------------------------------
                modelBuilder.Entity<ForumPost>(entity =>
            {
                entity.ToTable("ForumPosts");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("post_id").ValueGeneratedOnAdd();
                entity.Property(e => e.Title).HasColumnName("title");
                entity.Property(e => e.Body).HasColumnName("body");
                entity.Property(e => e.TimeStamp).HasColumnName("creation_date");
                entity.Property(e => e.AuthorId).HasColumnName("author_id");
                entity.Property(e => e.Score).HasColumnName("score");
                entity.Property(e => e.GameId).HasColumnName("game_id");
            });

            // -- ForumComment mapping ---------------------------------------------------
            modelBuilder.Entity<ForumComment>(entity =>
            {
                entity.ToTable("ForumComments");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("comment_id").ValueGeneratedOnAdd();
                entity.Property(e => e.Body).HasColumnName("body");
                entity.Property(e => e.TimeStamp).HasColumnName("creation_date");
                entity.Property(e => e.AuthorId).HasColumnName("author_id");
                entity.Property(e => e.Score).HasColumnName("score");
                entity.Property(e => e.PostId).HasColumnName("post_id");
            });

            // -- UserLikedPost mapping ------------------------------------------------------
            modelBuilder.Entity<UserLikedPost>(entity =>
            {
                entity.ToTable("UserLikedPost");
                entity.HasKey(e => new { e.UserId, e.PostId });
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.PostId).HasColumnName("post_id");
            });

            // -- UserDislikedPost mapping ---------------------------------------------------
            modelBuilder.Entity<UserDislikedPost>(entity =>
            {
                entity.ToTable("UserDislikedPost");
                entity.HasKey(e => new { e.UserId, e.PostId });
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.PostId).HasColumnName("post_id");
            });

            // -- UserLikedComment mapping ----------------------------------------------------
            modelBuilder.Entity<UserLikedComment>(entity =>
            {
                entity.ToTable("UserLikedComment");
                entity.HasKey(e => new { e.UserId, e.CommentId });
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.CommentId).HasColumnName("comment_id");
            });

            // -- UserDislikedComment mapping -----------------------------------------------------
            modelBuilder.Entity<UserDislikedComment>(entity =>
            {
                entity.ToTable("UserDislikedComment");
                entity.HasKey(e => new { e.UserId, e.CommentId });
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.CommentId).HasColumnName("comment_id");
            });

            // -- Friend mapping ---------------------------------------------------------
            /* DELETE ONCE FRIENDS FUNCTIONALITY IS SORTED OUT */
            modelBuilder.Entity<FriendEntity>(entity =>
            {
                entity.ToTable("Friends");
                entity.HasKey(e => e.FriendshipId);

                entity.Property(e => e.FriendshipId)
                      .HasColumnName("FriendshipId")
                      .ValueGeneratedOnAdd();

                entity.Property(e => e.User1Username)
                      .HasColumnName("User1Username")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.User2Username)
                      .HasColumnName("User2Username")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(e => e.CreatedDate)
                      .HasColumnName("CreatedDate")
                      .HasDefaultValueSql("GETDATE()");
            });

            // -- FriendRequest mapping ---------------------------------------------------
            modelBuilder.Entity<FriendRequest>(entity =>
            {
                entity.ToTable("FriendRequests");
                entity.HasKey(fr => fr.RequestId);
                entity.Property(fr => fr.RequestId)
                      .HasColumnName("RequestId")
                      .ValueGeneratedOnAdd();

                entity.Property(fr => fr.Username)
                      .HasColumnName("SenderUsername")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(fr => fr.Email)
                      .HasColumnName("SenderEmail")
                      .HasMaxLength(100)
                      .IsRequired();

                entity.Property(fr => fr.ProfilePhotoPath)
                      .HasColumnName("SenderProfilePhotoPath")
                      .HasMaxLength(255);

                entity.Property(fr => fr.ReceiverUsername)
                      .HasColumnName("ReceiverUsername")
                      .HasMaxLength(50)
                      .IsRequired();

                entity.Property(fr => fr.RequestDate)
                      .HasColumnName("RequestDate")
                      .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(fr => new { fr.Username, fr.ReceiverUsername })
                      .IsUnique()
                      .HasDatabaseName("UQ_SenderReceiver");
            });

            // -- NewsPost mapping -------------------------------------------------------
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("NewsPosts", "dbo");
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).HasColumnName("pid").ValueGeneratedOnAdd();
                entity.Property(n => n.AuthorId).HasColumnName("authorId");
                entity.Property(n => n.Content).HasColumnName("content");
                entity.Property(n => n.UploadDate).HasColumnName("uploadDate");
                entity.Property(n => n.NrLikes).HasColumnName("nrLikes");
                entity.Property(n => n.NrDislikes).HasColumnName("nrDislikes");
                entity.Property(n => n.NrComments).HasColumnName("nrComments");

                entity.Ignore(n => n.ActiveUserRating);
            });

            // -- NewsComment mapping ----------------------------------------------------
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("NewsComments", "dbo");
                entity.HasKey(c => c.CommentId);
                entity.Property(c => c.CommentId).HasColumnName("cid").ValueGeneratedOnAdd();
                entity.Property(c => c.AuthorId).HasColumnName("authorId");
                entity.Property(c => c.PostId).HasColumnName("postId");
                entity.Property(c => c.Content).HasColumnName("content");
                entity.Property(c => c.CommentDate).HasColumnName("uploadDate");

                entity.Ignore(c => c.NrLikes);
                entity.Ignore(c => c.NrDislikes);
            });

            // -- NewsRating mapping -----------------------------------------------------
            modelBuilder.Entity<PostRatingType>(entity =>
            {
                entity.ToTable("NewsRatings", "dbo");
                entity.HasKey(r => new { r.PostId, r.AuthorId });
                entity.Property(r => r.PostId).HasColumnName("postId");
                entity.Property(r => r.AuthorId).HasColumnName("authorId");
                entity.Property(r => r.RatingType).HasColumnName("ratingType");
            });

            // -- PasswordResetCode mapping -----------------------------------------------
            modelBuilder.Entity<PasswordResetCode>(entity =>
            {
                // Map to table name
                entity.ToTable("PasswordResetCodes");

                // Set primary key
                entity.HasKey(p => p.Id);

                // Column mappings
                entity.Property(p => p.Id)
                      .HasColumnName("id")
                      .ValueGeneratedOnAdd();
                entity.Property(p => p.UserId)

                      .HasColumnName("user_id").IsRequired();

                entity.Property(p => p.ResetCode)
                      .HasColumnName("reset_code");

                entity.Property(p => p.ExpirationTime)
                      .HasColumnName("expiration_time");

                entity.Property(p => p.Used)
                      .HasColumnName("used");

                entity.Property(p => p.Email)
                      .HasColumnName("email");
            });

            // -- Review mapping ------------------------------------------------------------
            modelBuilder.Entity<Review>(entity =>
            {
                // Map to table name
                entity.ToTable("Reviews");

                // Set primary key
                entity.HasKey(r => r.ReviewIdentifier);

                // Column mappings
                entity.Property(r => r.ReviewIdentifier)
                    .HasColumnName("ReviewId")
                    .ValueGeneratedOnAdd();

                entity.Property(r => r.ReviewTitleText)
                    .HasColumnName("Title")
                    .IsRequired();

                entity.Property(r => r.ReviewContentText)
                    .HasColumnName("Content")
                    .IsRequired();

                entity.Property(r => r.IsRecommended)
                    .HasColumnName("IsRecommended")
                    .HasColumnType("bit");

                entity.Property(r => r.NumericRatingGivenByUser)
                    .HasColumnName("Rating")
                    .HasColumnType("decimal(3,1)");

                entity.Property(r => r.TotalHelpfulVotesReceived)
                    .HasColumnName("HelpfulVotes");

                entity.Property(r => r.TotalFunnyVotesReceived)
                    .HasColumnName("FunnyVotes");

                entity.Property(r => r.TotalHoursPlayedByReviewer)
                    .HasColumnName("HoursPlayed");

                entity.Property(r => r.DateAndTimeWhenReviewWasCreated)
                    .HasColumnName("CreatedAt");

                entity.Property(r => r.UserIdentifier)
                    .HasColumnName("UserId")
                    .IsRequired();

                entity.Property(r => r.GameIdentifier)
                    .HasColumnName("GameId")
                    .IsRequired();
                // ignore display-only properties
                entity.Ignore(r => r.UserName);
                entity.Ignore(r => r.TitleOfGame);
                entity.Ignore(r => r.ProfilePictureBlob);
                entity.Ignore(r => r.HasVotedHelpful);
                entity.Ignore(r => r.HasVotedFunny);
            });

            // -- OwnedGame mapping ---------------------------------------------------------
            modelBuilder.Entity<OwnedGame>(entity =>
            {
                entity.ToTable("OwnedGames");
                entity.HasKey(og => og.GameId);
                entity.Property(og => og.GameId)
                    .HasColumnName("game_id")
                    .ValueGeneratedOnAdd();

                entity.Property(og => og.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();
                entity.HasIndex(og => og.UserId)
                      .HasDatabaseName("IX_OwnedGames_UserId");

                entity.Property(og => og.GameTitle)
                    .HasColumnName("title")
                    .IsRequired();

                entity.Property(og => og.Description)
                    .HasColumnName("description");

                entity.Property(og => og.CoverPicture)
                    .HasColumnName("cover_picture");

                // navigation to join-entity
                entity.HasMany(og => og.CollectionGames)
                      .WithOne(cg => cg.OwnedGame)
                      .HasForeignKey(cg => cg.GameId);
            });

            // -- SessionDetails mapping (UserSessions) -------------------------------------
            modelBuilder.Entity<SessionDetails>(entity =>
            {
                entity.ToTable("UserSessions");
                entity.HasKey(s => s.SessionId);
                entity.Property(s => s.SessionId)
                    .HasColumnName("session_id");
                entity.Property(s => s.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();
                entity.Property(s => s.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");
                entity.Property(s => s.ExpiresAt)
                    .HasColumnName("expires_at")
                    .IsRequired();
            });

            // -- Friendship mapping --------------------------------------------------------
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.ToTable("Friendships");
                entity.HasKey(f => f.FriendshipId);
                entity.Property(f => f.FriendshipId)
                    .HasColumnName("friendship_id")
                    .ValueGeneratedOnAdd();
                entity.Property(f => f.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();
                entity.Property(f => f.FriendId)
                    .HasColumnName("friend_id")
                    .IsRequired();
                entity.HasIndex(f => f.UserId)
                    .HasDatabaseName("IX_Friendships_UserId");
                entity.HasIndex(f => f.FriendId)
                    .HasDatabaseName("IX_Friendships_FriendId");
                // Composite unique constraint
                entity.HasIndex(f => new { f.UserId, f.FriendId })
                    .IsUnique()
                    .HasDatabaseName("UQ_Friendship");
                // Ignore non-mapped properties
                entity.Ignore(f => f.FriendUsername);
                entity.Ignore(f => f.FriendProfilePicture);
            });

            // -- Achievement mapping --------------------------------------------------------
            modelBuilder.Entity<Achievement>(entity =>
            {
                entity.ToTable("Achievements");
                entity.HasKey(a => a.AchievementId);
                entity.Property(a => a.AchievementId)
                    .HasColumnName("achievement_id")
                   .ValueGeneratedOnAdd();
                entity.Property(a => a.AchievementName)
                    .HasColumnName("achievement_name")
                    .IsRequired();
                entity.Property(a => a.Description)
                    .HasColumnName("description");
                entity.Property(a => a.AchievementType)
                    .HasColumnName("achievement_type")
                    .IsRequired();
                entity.Property(a => a.Points)
                    .HasColumnName("points")
                    .IsRequired();
                entity.Property(a => a.Icon)
                    .HasColumnName("icon_url");
            });

            // -- UserAchievement mapping ----------------------------------------------------
            modelBuilder.Entity<UserAchievement>(entity =>
            {
                entity.ToTable("UserAchievements");

                // Composite PK on (UserId, AchievementId)
                entity.HasKey(ua => new { ua.UserId, ua.AchievementId });

                // Map columns
                entity.Property(ua => ua.UserId)
                      .HasColumnName("user_id")
                      .IsRequired();

                entity.Property(ua => ua.AchievementId)
                      .HasColumnName("achievement_id")
                      .IsRequired();

                entity.Property(ua => ua.UnlockedAt)
                      .HasColumnName("unlocked_at")
                      .HasDefaultValueSql("GETDATE()");

                // FKs
                entity.HasOne(ua => ua.User)
                      .WithMany(u => u.UserAchievements) // you'll need to add this nav prop on User
                      .HasForeignKey(ua => ua.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ua => ua.Achievement)
                      .WithMany(a => a.UserAchievements) // and this nav prop on Achievement
                      .HasForeignKey(ua => ua.AchievementId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -- Collection mapping --------------------------------------------------------
            modelBuilder.Entity<Collection>(entity =>
            {
                entity.ToTable("Collections");
                entity.HasKey(c => c.CollectionId);
                entity.Property(c => c.CollectionId)
                    .HasColumnName("collection_id")
                    .ValueGeneratedOnAdd();

                entity.Property(c => c.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();
                entity.HasIndex(c => c.UserId);

                entity.Property(c => c.CollectionName)
                    .HasColumnName("name")
                    .IsRequired();

                entity.Property(c => c.CoverPicture)
                    .HasColumnName("cover_picture");

                entity.Property(c => c.IsPublic)
                    .HasColumnName("is_public")
                    .HasDefaultValue(true);

                entity.Property(c => c.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("date")
                    .HasDefaultValueSql("CAST(GETDATE() AS DATE)");

                // navigation to join-entity
                entity.HasMany(c => c.CollectionGames)
                      .WithOne(cg => cg.Collection)
                      .HasForeignKey(cg => cg.CollectionId);
            });

            // -- UserProfile mapping --------------------------------------------------------
            modelBuilder.Entity<UserProfile>(entity =>
            {
                // Map to table name
                entity.ToTable("UserProfiles");

                // Set primary key
                entity.HasKey(up => up.ProfileId);

                // Column mappings
                entity.Property(up => up.ProfileId)
                    .HasColumnName("profile_id")
                    .ValueGeneratedOnAdd();

                entity.Property(up => up.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();

                entity.Property(up => up.ProfilePicture)
                    .HasColumnName("profile_picture");

                entity.Property(up => up.Bio)
                    .HasColumnName("bio");

                entity.Property(up => up.LastModified)
                    .HasColumnName("last_modified")
                    .HasDefaultValueSql("GETDATE()");

                // These three are not real columns so ignore them
                entity.Ignore(up => up.Email);
                entity.Ignore(up => up.Username);
                entity.Ignore(up => up.ProfilePhotoPath);
                // TODO: add the frame, hat, pet, and emoji properties when they are implemented
            });

            // -- Wallet mapping --------------------------------------------------------
            modelBuilder.Entity<Wallet>(entity =>
            {
                // Map to table name
                entity.ToTable("Wallet");

                // Set primary key
                entity.HasKey(w => w.WalletId);

                // Column mappings
                entity.Property(w => w.WalletId)
                    .HasColumnName("wallet_id")
                    .ValueGeneratedOnAdd();

                entity.Property(w => w.UserId)
                    .HasColumnName("user_id")
                    .IsRequired();
                entity.HasIndex(w => w.UserId)
                    .IsUnique();

                entity.Property(w => w.Points)
                    .HasColumnName("points")
                    .HasDefaultValue(0);

                entity.Property(w => w.Balance)
                    .HasColumnName("money_for_games")
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValue(0m);
            });

            // -- Users mapping --------------------------------------------------------------
            modelBuilder.Entity<User>(entity =>
            {
                // Map to table name
                entity.ToTable("Users");

                // Set primary key
                entity.HasKey(u => u.UserId);

                // Column mappings
                entity.Property(u => u.UserId)
                    .HasColumnName("user_id")
                    .ValueGeneratedOnAdd();

                entity.Property(u => u.Username)
                    .HasColumnName("username")
                    .IsRequired();

                entity.Property(u => u.Email)
                    .HasColumnName("email")
                    .IsRequired();

                entity.Property(u => u.Password)
                    .HasColumnName("hashed_password")
                    .IsRequired();

                entity.Property(u => u.IsDeveloper)
                    .HasColumnName("developer")
                    .HasDefaultValue(false);

                entity.Property(u => u.CreatedAt)
                    .HasColumnName("created_at")
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(u => u.LastLogin)
                    .HasColumnName("last_login");
            });

            modelBuilder.Entity<ChatConversation>(entity =>
            {
                entity.ToTable("ChatConversations");
                entity.HasKey(c => c.ConversationId);
                entity.Property(c => c.ConversationId)
                    .HasColumnName("conversation_id")
                    .ValueGeneratedOnAdd();
                entity.Property(c => c.User1Id)
                    .HasColumnName("user1_id")
                    .IsRequired();
                entity.Property(c => c.User2Id)
                    .HasColumnName("user2_id")
                    .IsRequired();
            });

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.ToTable("ChatMessages");
                entity.HasKey(m => m.MessageId);
                entity.Property(m => m.MessageId)
                    .HasColumnName("message_id")
                    .ValueGeneratedOnAdd();
                entity.Property(m => m.ConversationId)
                    .HasColumnName("conversation_id")
                    .IsRequired();
                entity.Property(m => m.SenderId)
                    .HasColumnName("sender_id")
                    .IsRequired();
                entity.Property(m => m.MessageContent)
                    .HasColumnName("message_content")
                    .IsRequired();
                entity.Property(m => m.MessageFormat)
                    .HasColumnName("message_format")
                    .IsRequired();
                entity.Property(m => m.Timestamp)
                    .HasColumnName("timestamp")
                    .HasDefaultValueSql("GETDATE()");
            });

            // Users seed data
            var usersSeed = new List<object>
            {
                new { user_id = 1, email = "alice@example.com", username = "AliceGamer", hashed_password = "hashed_password_1", developer = true, created_at = DateTime.Parse("2025-03-20 14:25:00"), last_login = DateTime.Parse("2025-03-20 14:25:00") },
                new { user_id = 2, email = "bob@example.com", username = "BobTheBuilder", hashed_password = "hashed_password_2", developer = false, created_at = DateTime.Parse("2025-03-21 10:12:00"), last_login = DateTime.Parse("2025-03-21 10:12:00") },
                new { user_id = 3, email = "charlie@example.com", username = "CharlieX", hashed_password = "hashed_password_3", developer = false, created_at = DateTime.Parse("2025-03-22 18:45:00"), last_login = DateTime.Parse("2025-03-22 18:45:00") },
                new { user_id = 4, email = "diana@example.com", username = "DianaRocks", hashed_password = "hashed_password_4", developer = false, created_at = DateTime.Parse("2025-03-19 22:30:00"), last_login = DateTime.Parse("2025-03-19 22:30:00") },
                new { user_id = 5, email = "eve@example.com", username = "Eve99", hashed_password = "hashed_password_5", developer = true, created_at = DateTime.Parse("2025-03-23 08:05:00"), last_login = DateTime.Parse("2025-03-23 08:05:00") },
                new { user_id = 6, email = "frank@example.com", username = "FrankTheTank", hashed_password = "hashed_password_6", developer = false, created_at = DateTime.Parse("2025-03-24 16:20:00"), last_login = DateTime.Parse("2025-03-24 16:20:00") },
                new { user_id = 7, email = "grace@example.com", username = "GraceSpeed", hashed_password = "hashed_password_7", developer = false, created_at = DateTime.Parse("2025-03-25 11:40:00"), last_login = DateTime.Parse("2025-03-25 11:40:00") },
                new { user_id = 8, email = "harry@example.com", username = "HarryWizard", hashed_password = "hashed_password_8", developer = false, created_at = DateTime.Parse("2025-03-20 20:15:00"), last_login = DateTime.Parse("2025-03-20 20:15:00") },
                new { user_id = 9, email = "ivy@example.com", username = "IvyNinja", hashed_password = "hashed_password_9", developer = false, created_at = DateTime.Parse("2025-03-22 09:30:00"), last_login = DateTime.Parse("2025-03-22 09:30:00") },
                new { user_id = 10, email = "jack@example.com", username = "JackHacks", hashed_password = "hashed_password_10", developer = true, created_at = DateTime.Parse("2025-03-24 23:55:00"), last_login = DateTime.Parse("2025-03-24 23:55:00") },
                new { user_id = 11, email = "user11@example.com", username = "UserEleven", hashed_password = "hashed_password_11", developer = false, created_at = DateTime.Now, last_login = DateTime.Now },
                new { user_id = 12, email = "user12@example.com", username = "UserTwelve", hashed_password = "hashed_password_12", developer = false, created_at = DateTime.Now, last_login = DateTime.Now },
                new { user_id = 13, email = "user13@example.com", username = "UserThirteen", hashed_password = "hashed_password_13", developer = false, created_at = DateTime.Now, last_login = DateTime.Now }
            };

            modelBuilder.Entity<User>().HasData(usersSeed);

            // UserProfiles seed data
            var userProfilesSeed = new List<object>
            {
                new { profile_id = 1, user_id = 1, profile_picture = "ms-appx:///Assets/Collections/image.jpg", bio = "Gaming enthusiast and software developer", last_modified = DateTime.Now },
                new { profile_id = 2, user_id = 2, profile_picture = "ms-appx:///Assets/download.jpg", bio = "Game developer and tech lover", last_modified = DateTime.Now },
                new { profile_id = 3, user_id = 3, profile_picture = "ms-appx:///Assets/download.jpg", bio = "Casual gamer and streamer", last_modified = DateTime.Now },
                new { profile_id = 4, user_id = 4, profile_picture = "ms-appx:///Assets/Collections/image.jpg", bio = "Casual gamer and streamer", last_modified = DateTime.Now },
                new { profile_id = 5, user_id = 5, profile_picture = "ms-appx:///Assets/download.jpg", bio = "Casual gamer and streamer", last_modified = DateTime.Now },
                new { profile_id = 6, user_id = 6, profile_picture = "ms-appx:///Assets/default_picture.jpg", bio = "Casual gamer and streamer", last_modified = DateTime.Now },
                new { profile_id = 7, user_id = 7, profile_picture = "ms-appx:///Assets/default_picture.jpg", bio = "Casual gamer and streamer", last_modified = DateTime.Now },
                new { profile_id = 8, user_id = 8, profile_picture = "ms-appx:///Assets/default_picture.jpg", bio = "Casual gamer and streamer", last_modified = DateTime.Now },
                new { profile_id = 9, user_id = 9, profile_picture = "ms-appx:///Assets/default_picture.jpg", bio = "Casual gamer and streamer", last_modified = DateTime.Now },
                new { profile_id = 10, user_id = 10, profile_picture = "ms-appx:///Assets/default_picture.jpg", bio = "Casual gamer and streamer", last_modified = DateTime.Now },
                new { profile_id = 11, user_id = 11, profile_picture = "ms-appx:///Assets/default_picture.jpg", bio = "Welcome new user!", last_modified = DateTime.Now },
                new { profile_id = 12, user_id = 12, profile_picture = "ms-appx:///Assets/default_picture.jpg", bio = "Welcome new user!", last_modified = DateTime.Now },
                new { profile_id = 13, user_id = 13, profile_picture = "ms-appx:///Assets/default_picture.jpg", bio = "Welcome new user!", last_modified = DateTime.Now }
            };

            modelBuilder.Entity<UserProfile>().HasData(userProfilesSeed);

            // ReviewsUsers seed data
            var reviewsUsersSeed = new List<object>
            {
                new { UserId = 2, Name = "Sam Carter", ProfilePicture = (byte[])null },
                new { UserId = 3, Name = "Taylor Kim", ProfilePicture = (byte[])null }
            };

            modelBuilder.Entity<ReviewsUser>().HasData(reviewsUsersSeed);

            // Features seed data
            var featuresSeed = new List<object>
            {
                new { feature_id = 1, name = "Black Hat", value = 2000, description = "An elegant hat", type = "hat", source = "Assets/Features/Hats/black-hat.png", equipped = false },
                new { feature_id = 2, name = "Pufu", value = 10, description = "Cute doggo", type = "pet", source = "Assets/Features/Pets/dog.png", equipped = false },
                new { feature_id = 3, name = "Kitty", value = 8, description = "Cute cat", type = "pet", source = "Assets/Features/Pets/cat.png", equipped = false },
                new { feature_id = 4, name = "Frame", value = 5, description = "Violet frame", type = "frame", source = "Assets/Features/Frames/frame1.png", equipped = false },
                new { feature_id = 5, name = "Love Emoji", value = 7, description = "lalal", type = "emoji", source = "Assets/Features/Emojis/love.png", equipped = false },
                new { feature_id = 6, name = "Violet Background", value = 7, description = "Violet Background", type = "background", source = "Assets/Features/Backgrounds/violet.jpg", equipped = false }
            };

            modelBuilder.Entity<Feature>().HasData(featuresSeed);

            // Wallets seed data
            var walletsSeed = new List<object>
            {
                new { wallet_id = 1, user_id = 1, points = 10, money_for_games = 200m },
                new { wallet_id = 2, user_id = 2, points = 10, money_for_games = 200m },
                new { wallet_id = 3, user_id = 3, points = 10, money_for_games = 200m },
                new { wallet_id = 4, user_id = 4, points = 10, money_for_games = 200m },
                new { wallet_id = 5, user_id = 5, points = 10, money_for_games = 200m },
                new { wallet_id = 6, user_id = 6, points = 10, money_for_games = 200m },
                new { wallet_id = 7, user_id = 7, points = 10, money_for_games = 200m },
                new { wallet_id = 8, user_id = 8, points = 10, money_for_games = 200m },
                new { wallet_id = 9, user_id = 9, points = 10, money_for_games = 200m },
                new { wallet_id = 10, user_id = 10, points = 10, money_for_games = 200m },
                new { wallet_id = 11, user_id = 11, points = 10, money_for_games = 200m },
                new { wallet_id = 12, user_id = 12, points = 10, money_for_games = 200m },
                new { wallet_id = 13, user_id = 13, points = 10, money_for_games = 200m }
            };

            modelBuilder.Entity<Wallet>().HasData(walletsSeed);

            // Collections seed data
            var collectionsSeed = new List<object>
            {
                new { collection_id = 1, user_id = 1, name = "All Owned Games", cover_picture = "/Assets/Collections/allgames.jpg", is_public = true, created_at = DateOnly.Parse("2022-02-21") },
                new { collection_id = 2, user_id = 1, name = "Sports", cover_picture = "/Assets/Collections/sports.jpg", is_public = true, created_at = DateOnly.Parse("2023-03-21") },
                new { collection_id = 3, user_id = 1, name = "Chill Games", cover_picture = "/Assets/Collections/chill.jpg", is_public = true, created_at = DateOnly.Parse("2024-03-21") },
                new { collection_id = 4, user_id = 1, name = "X-Mas", cover_picture = "/Assets/Collections/xmas.jpg", is_public = false, created_at = DateOnly.Parse("2025-02-21") },
                new { collection_id = 5, user_id = 2, name = "Shooters", cover_picture = "/Assets/Collections/shooters.jpg", is_public = true, created_at = DateOnly.Parse("2025-03-21") },
                new { collection_id = 6, user_id = 2, name = "Pets", cover_picture = "/Assets/Collections/pets.jpg", is_public = false, created_at = DateOnly.Parse("2025-01-21") },
                new { collection_id = 7, user_id = 11, name = "All Owned Games", cover_picture = "/Assets/Collections/allgames.jpg", is_public = true, created_at = DateOnly.Parse("2022-02-21") },
                new { collection_id = 8, user_id = 11, name = "Shooters", cover_picture = "/Assets/Collections/shooters.jpg", is_public = true, created_at = DateOnly.Parse("2025-03-21") },
                new { collection_id = 9, user_id = 11, name = "Sports", cover_picture = "/Assets/Collections/sports.jpg", is_public = true, created_at = DateOnly.Parse("2023-03-21") },
                new { collection_id = 10, user_id = 11, name = "Chill Games", cover_picture = "/Assets/Collections/chill.jpg", is_public = true, created_at = DateOnly.Parse("2024-03-21") },
                new { collection_id = 11, user_id = 11, name = "Pets", cover_picture = "/Assets/Collections/pets.jpg", is_public = false, created_at = DateOnly.Parse("2025-01-21") },
                new { collection_id = 12, user_id = 11, name = "X-Mas", cover_picture = "/Assets/Collections/xmas.jpg", is_public = false, created_at = DateOnly.Parse("2025-02-21") }
            };

            modelBuilder.Entity<Collection>().HasData(collectionsSeed);

            // OwnedGames seed data
            var ownedGamesSeed = new List<object>
            {
                new { game_id = 1, user_id = 11, title = "Call of Duty: MWIII", description = "First?person military shooter", cover_picture = "/Assets/Games/codmw3.png" },
                new { game_id = 2, user_id = 11, title = "Overwatch2", description = "Team?based hero shooter", cover_picture = "/Assets/Games/overwatch2.png" },
                new { game_id = 3, user_id = 11, title = "Counter?Strike2", description = "Tactical shooter", cover_picture = "/Assets/Games/cs2.png" },
                new { game_id = 4, user_id = 11, title = "FIFA25", description = "Football simulation", cover_picture = "/Assets/Games/fifa25.png" },
                new { game_id = 5, user_id = 11, title = "NBA2K25", description = "Basketball simulation", cover_picture = "/Assets/Games/nba2k25.png" },
                new { game_id = 6, user_id = 11, title = "Tony Hawk Pro Skater", description = "Skateboarding sports game", cover_picture = "/Assets/Games/thps.png" },
                new { game_id = 7, user_id = 11, title = "Stardew Valley", description = "Relaxing farming game", cover_picture = "/Assets/Games/stardewvalley.png" },
                new { game_id = 8, user_id = 11, title = "The Sims4: Cats & Dogs", description = "Life sim with pets", cover_picture = "/Assets/Games/sims4pets.png" },
                new { game_id = 9, user_id = 11, title = "Nintendogs", description = "Pet care simulation", cover_picture = "/Assets/Games/nintendogs.png" },
                new { game_id = 10, user_id = 11, title = "Pet Hotel", description = "Manage a hotel for pets", cover_picture = "/Assets/Games/pethotel.png" },
                new { game_id = 11, user_id = 11, title = "Christmas Wonderland", description = "Festive hidden object game", cover_picture = "/Assets/Games/xmas.png" }
            };

            modelBuilder.Entity<OwnedGame>().HasData(ownedGamesSeed);

            // Achievements seed data
            var achievementsSeed = new List<object>
            {
                new { achievement_id = 1, achievement_name = "FRIENDSHIP1", description = "You made a friend, you get a point", achievement_type = "Friendships", points = 1, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 2, achievement_name = "FRIENDSHIP2", description = "You made 5 friends, you get 3 points", achievement_type = "Friendships", points = 3, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 3, achievement_name = "FRIENDSHIP3", description = "You made 10 friends, you get 5 points", achievement_type = "Friendships", points = 5, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 4, achievement_name = "FRIENDSHIP4", description = "You made 50 friends, you get 10 points", achievement_type = "Friendships", points = 10, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 5, achievement_name = "FRIENDSHIP5", description = "You made 100 friends, you get 15 points", achievement_type = "Friendships", points = 15, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 6, achievement_name = "OWNEDGAMES1", description = "You own 1 game, you get 1 point", achievement_type = "Owned Games", points = 1, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 7, achievement_name = "OWNEDGAMES2", description = "You own 5 games, you get 3 points", achievement_type = "Owned Games", points = 3, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 8, achievement_name = "OWNEDGAMES3", description = "You own 10 games, you get 5 points", achievement_type = "Owned Games", points = 5, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 9, achievement_name = "OWNEDGAMES4", description = "You own 50 games, you get 10 points", achievement_type = "Owned Games", points = 10, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 10, achievement_name = "SOLDGAMES1", description = "You sold 1 game, you get 1 point", achievement_type = "Sold Games", points = 1, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 11, achievement_name = "SOLDGAMES2", description = "You sold 5 games, you get 3 points", achievement_type = "Sold Games", points = 3, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 12, achievement_name = "SOLDGAMES3", description = "You sold 10 games, you get 5 points", achievement_type = "Sold Games", points = 5, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 13, achievement_name = "SOLDGAMES4", description = "You sold 50 games, you get 10 points", achievement_type = "Sold Games", points = 10, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 14, achievement_name = "REVIEW1", description = "You gave 1 review, you get 1 point", achievement_type = "Number of Reviews Given", points = 1, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 15, achievement_name = "REVIEW2", description = "You gave 5 reviews, you get 3 points", achievement_type = "Number of Reviews Given", points = 3, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 16, achievement_name = "REVIEW3", description = "You gave 10 reviews, you get 5 points", achievement_type = "Number of Reviews Given", points = 5, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 17, achievement_name = "REVIEW4", description = "You gave 50 reviews, you get 10 points", achievement_type = "Number of Reviews Given", points = 10, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 18, achievement_name = "REVIEWR1", description = "You got 1 review, you get 1 point", achievement_type = "Number of Reviews Received", points = 1, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 19, achievement_name = "REVIEWR2", description = "You got 5 reviews, you get 3 points", achievement_type = "Number of Reviews Received", points = 3, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 20, achievement_name = "REVIEWR3", description = "You got 10 reviews, you get 5 points", achievement_type = "Number of Reviews Received", points = 5, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 21, achievement_name = "REVIEWR4", description = "You got 50 reviews, you get 10 points", achievement_type = "Number of Reviews Received", points = 10, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 22, achievement_name = "DEVELOPER", description = "You are a developer, you get 10 points", achievement_type = "Developer", points = 10, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 23, achievement_name = "ACTIVITY1", description = "You have been active for 1 year, you get 1 point", achievement_type = "Years of Activity", points = 1, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 24, achievement_name = "ACTIVITY2", description = "You have been active for 2 years, you get 3 points", achievement_type = "Years of Activity", points = 3, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 25, achievement_name = "ACTIVITY3", description = "You have been active for 3 years, you get 5 points", achievement_type = "Years of Activity", points = 5, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 26, achievement_name = "ACTIVITY4", description = "You have been active for 4 years, you get 10 points", achievement_type = "Years of Activity", points = 10, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 27, achievement_name = "POSTS1", description = "You have made 1 post, you get 1 point", achievement_type = "Number of Posts", points = 1, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 28, achievement_name = "POSTS2", description = "You have made 5 posts, you get 3 points", achievement_type = "Number of Posts", points = 3, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 29, achievement_name = "POSTS3", description = "You have made 10 posts, you get 5 points", achievement_type = "Number of Posts", points = 5, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { achievement_id = 30, achievement_name = "POSTS4", description = "You have made 50 posts, you get 10 points", achievement_type = "Number of Posts", points = 10, icon_url = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" }
            };

            modelBuilder.Entity<Achievement>().HasData(achievementsSeed);
        }
    }
}

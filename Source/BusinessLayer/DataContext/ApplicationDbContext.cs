﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;
using Google;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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
                new { UserId = 1, Email = "alice@example.com", Username = "AliceGamer", Password = "hashed_password_1", IsDeveloper = true, CreatedAt = DateTime.Parse("2025-03-20 14:25:00"), LastLogin = DateTime.Parse("2025-03-20 14:25:00") },
                new { UserId = 2, Email = "bob@example.com", Username = "BobTheBuilder", Password = "hashed_password_2", IsDeveloper = false, CreatedAt = DateTime.Parse("2025-03-21 10:12:00"), LastLogin = DateTime.Parse("2025-03-21 10:12:00") },
                new { UserId = 3, Email = "charlie@example.com", Username = "CharlieX", Password = "hashed_password_3", IsDeveloper = false, CreatedAt = DateTime.Parse("2025-03-22 18:45:00"), LastLogin = DateTime.Parse("2025-03-22 18:45:00") },
                new { UserId = 4, Email = "diana@example.com", Username = "DianaRocks", Password = "hashed_password_4", IsDeveloper = false, CreatedAt = DateTime.Parse("2025-03-19 22:30:00"), LastLogin = DateTime.Parse("2025-03-19 22:30:00") },
                new { UserId = 5, Email = "eve@example.com", Username = "Eve99", Password = "hashed_password_5", IsDeveloper = true, CreatedAt = DateTime.Parse("2025-03-23 08:05:00"), LastLogin = DateTime.Parse("2025-03-23 08:05:00") },
                new { UserId = 6, Email = "frank@example.com", Username = "FrankTheTank", Password = "hashed_password_6", IsDeveloper = false, CreatedAt = DateTime.Parse("2025-03-24 16:20:00"), LastLogin = DateTime.Parse("2025-03-24 16:20:00") },
                new { UserId = 7, Email = "grace@example.com", Username = "GraceSpeed", Password = "hashed_password_7", IsDeveloper = false, CreatedAt = DateTime.Parse("2025-03-25 11:40:00"), LastLogin = DateTime.Parse("2025-03-25 11:40:00") },
                new { UserId = 8, Email = "harry@example.com", Username = "HarryWizard", Password = "hashed_password_8", IsDeveloper = false, CreatedAt = DateTime.Parse("2025-03-20 20:15:00"), LastLogin = DateTime.Parse("2025-03-20 20:15:00") },
                new { UserId = 9, Email = "ivy@example.com", Username = "IvyNinja", Password = "hashed_password_9", IsDeveloper = false, CreatedAt = DateTime.Parse("2025-03-22 09:30:00"), LastLogin = DateTime.Parse("2025-03-22 09:30:00") },
                new { UserId = 10, Email = "jack@example.com", Username = "JackHacks", Password = "hashed_password_10", IsDeveloper = true, CreatedAt = DateTime.Parse("2025-03-24 23:55:00"), LastLogin = DateTime.Parse("2025-03-24 23:55:00") },
                new { UserId = 11, Email = "user11@example.com", Username = "UserEleven", Password = "hashed_password_11", IsDeveloper = false, CreatedAt = DateTime.Now, LastLogin = DateTime.Now },
                new { UserId = 12, Email = "user12@example.com", Username = "UserTwelve", Password = "hashed_password_12", IsDeveloper = false, CreatedAt = DateTime.Now, LastLogin = DateTime.Now },
                new { UserId = 13, Email = "user13@example.com", Username = "UserThirteen", Password = "hashed_password_13", IsDeveloper = false, CreatedAt = DateTime.Now, LastLogin = DateTime.Now }
            };

            modelBuilder.Entity<User>().HasData(usersSeed);

            // UserProfiles seed data
            var userProfilesSeed = new List<object>
            {
                new { ProfileId = 1, UserId = 1, ProfilePicture = "ms-appx:///Assets/Collections/image.jpg", Bio = "Gaming enthusiast and software developer", LastModified = DateTime.Now },
                new { ProfileId = 2, UserId = 2, ProfilePicture = "ms-appx:///Assets/download.jpg", Bio = "Game developer and tech lover", LastModified = DateTime.Now },
                new { ProfileId = 3, UserId = 3, ProfilePicture = "ms-appx:///Assets/download.jpg", Bio = "Casual gamer and streamer", LastModified = DateTime.Now },
                new { ProfileId = 4, UserId = 4, ProfilePicture = "ms-appx:///Assets/Collections/image.jpg", Bio = "Casual gamer and streamer", LastModified = DateTime.Now },
                new { ProfileId = 5, UserId = 5, ProfilePicture = "ms-appx:///Assets/download.jpg", Bio = "Casual gamer and streamer", LastModified = DateTime.Now },
                new { ProfileId = 6, UserId = 6, ProfilePicture = "ms-appx:///Assets/default_picture.jpg", Bio = "Casual gamer and streamer", LastModified = DateTime.Now },
                new { ProfileId = 7, UserId = 7, ProfilePicture = "ms-appx:///Assets/default_picture.jpg", Bio = "Casual gamer and streamer", LastModified = DateTime.Now },
                new { ProfileId = 8, UserId = 8, ProfilePicture = "ms-appx:///Assets/default_picture.jpg", Bio = "Casual gamer and streamer", LastModified = DateTime.Now },
                new { ProfileId = 9, UserId = 9, ProfilePicture = "ms-appx:///Assets/default_picture.jpg", Bio = "Casual gamer and streamer", LastModified = DateTime.Now },
                new { ProfileId = 10, UserId = 10, ProfilePicture = "ms-appx:///Assets/default_picture.jpg", Bio = "Casual gamer and streamer", LastModified = DateTime.Now },
                new { ProfileId = 11, UserId = 11, ProfilePicture = "ms-appx:///Assets/default_picture.jpg", Bio = "Welcome new user!", LastModified = DateTime.Now },
                new { ProfileId = 12, UserId = 12, ProfilePicture = "ms-appx:///Assets/default_picture.jpg", Bio = "Welcome new user!", LastModified = DateTime.Now },
                new { ProfileId = 13, UserId = 13, ProfilePicture = "ms-appx:///Assets/default_picture.jpg", Bio = "Welcome new user!", LastModified = DateTime.Now }
            };

            modelBuilder.Entity<UserProfile>().HasData(userProfilesSeed);

            // Features seed data
            var featuresSeed = new List<object>
            {
                new { FeatureId = 1, Name = "Black Hat", Value = 2000, Description = "An elegant hat", Type = "hat", Source = "Assets/Features/Hats/black-hat.png", Equipped = false },
                new { FeatureId = 2, Name = "Pufu", Value = 10, Description = "Cute doggo", Type = "pet", Source = "Assets/Features/Pets/dog.png", Equipped = false },
                new { FeatureId = 3, Name = "Kitty", Value = 8, Description = "Cute cat", Type = "pet", Source = "Assets/Features/Pets/cat.png", Equipped = false },
                new { FeatureId = 4, Name = "Frame", Value = 5, Description = "Violet frame", Type = "frame", Source = "Assets/Features/Frames/frame1.png", Equipped = false },
                new { FeatureId = 5, Name = "Love Emoji", Value = 7, Description = "lalal", Type = "emoji", Source = "Assets/Features/Emojis/love.png", Equipped = false },
                new { FeatureId = 6, Name = "Violet Background", Value = 7, Description = "Violet Background", Type = "background", Source = "Assets/Features/Backgrounds/violet.jpg", Equipped = false }
            };

            modelBuilder.Entity<Feature>().HasData(featuresSeed);

            // Wallets seed data
            var walletsSeed = new List<object>
            {
                new { WalletId = 1, UserId = 1, Points = 10, Balance = 200m },
                new { WalletId = 2, UserId = 2, Points = 10, Balance = 200m },
                new { WalletId = 3, UserId = 3, Points = 10, Balance = 200m },
                new { WalletId = 4, UserId = 4, Points = 10, Balance = 200m },
                new { WalletId = 5, UserId = 5, Points = 10, Balance = 200m },
                new { WalletId = 6, UserId = 6, Points = 10, Balance = 200m },
                new { WalletId = 7, UserId = 7, Points = 10, Balance = 200m },
                new { WalletId = 8, UserId = 8, Points = 10, Balance = 200m },
                new { WalletId = 9, UserId = 9, Points = 10, Balance = 200m },
                new { WalletId = 10, UserId = 10, Points = 10, Balance = 200m },
                new { WalletId = 11, UserId = 11, Points = 10, Balance = 200m },
                new { WalletId = 12, UserId = 12, Points = 10, Balance = 200m },
                new { WalletId = 13, UserId = 13, Points = 10, Balance = 200m }
            };

            modelBuilder.Entity<Wallet>().HasData(walletsSeed);

            // Collections seed data
            var collectionsSeed = new List<object>
            {
                new { CollectionId = 1, UserId = 1, CollectionName = "All Owned Games", CoverPicture = "/Assets/Collections/allgames.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2022-02-21") },
                new { CollectionId = 2, UserId = 1, CollectionName = "Sports", CoverPicture = "/Assets/Collections/sports.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2023-03-21") },
                new { CollectionId = 3, UserId = 1, CollectionName = "Chill Games", CoverPicture = "/Assets/Collections/chill.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2024-03-21") },
                new { CollectionId = 4, UserId = 1, CollectionName = "X-Mas", CoverPicture = "/Assets/Collections/xmas.jpg", IsPublic = false, CreatedAt = DateOnly.Parse("2025-02-21") },
                new { CollectionId = 5, UserId = 2, CollectionName = "Shooters", CoverPicture = "/Assets/Collections/shooters.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2025-03-21") },
                new { CollectionId = 6, UserId = 2, CollectionName = "Pets", CoverPicture = "/Assets/Collections/pets.jpg", IsPublic = false, CreatedAt = DateOnly.Parse("2025-01-21") },
                new { CollectionId = 7, UserId = 11, CollectionName = "All Owned Games", CoverPicture = "/Assets/Collections/allgames.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2022-02-21") },
                new { CollectionId = 8, UserId = 11, CollectionName = "Shooters", CoverPicture = "/Assets/Collections/shooters.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2025-03-21") },
                new { CollectionId = 9, UserId = 11, CollectionName = "Sports", CoverPicture = "/Assets/Collections/sports.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2023-03-21") },
                new { CollectionId = 10, UserId = 11, CollectionName = "Chill Games", CoverPicture = "/Assets/Collections/chill.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2024-03-21") },
                new { CollectionId = 11, UserId = 11, CollectionName = "Pets", CoverPicture = "/Assets/Collections/pets.jpg", IsPublic = false, CreatedAt = DateOnly.Parse("2025-01-21") },
                new { CollectionId = 12, UserId = 11, CollectionName = "X-Mas", CoverPicture = "/Assets/Collections/xmas.jpg", IsPublic = false, CreatedAt = DateOnly.Parse("2025-02-21") }
            };

            modelBuilder.Entity<Collection>().HasData(collectionsSeed);

            // OwnedGames seed data
            var ownedGamesSeed = new List<object>
            {
                new { GameId = 1, UserId = 11, GameTitle = "Call of Duty: MWIII", Description = "First?person military shooter", CoverPicture = "/Assets/Games/codmw3.png" },
                new { GameId = 2, UserId = 11, GameTitle = "Overwatch2", Description = "Team?based hero shooter", CoverPicture = "/Assets/Games/overwatch2.png" },
                new { GameId = 3, UserId = 11, GameTitle = "Counter?Strike2", Description = "Tactical shooter", CoverPicture = "/Assets/Games/cs2.png" },
                new { GameId = 4, UserId = 11, GameTitle = "FIFA25", Description = "Football simulation", CoverPicture = "/Assets/Games/fifa25.png" },
                new { GameId = 5, UserId = 11, GameTitle = "NBA2K25", Description = "Basketball simulation", CoverPicture = "/Assets/Games/nba2k25.png" },
                new { GameId = 6, UserId = 11, GameTitle = "Tony Hawk Pro Skater", Description = "Skateboarding sports game", CoverPicture = "/Assets/Games/thps.png" },
                new { GameId = 7, UserId = 11, GameTitle = "Stardew Valley", Description = "Relaxing farming game", CoverPicture = "/Assets/Games/stardewvalley.png" },
                new { GameId = 8, UserId = 11, GameTitle = "The Sims4: Cats & Dogs", Description = "Life sim with pets", CoverPicture = "/Assets/Games/sims4pets.png" },
                new { GameId = 9, UserId = 11, GameTitle = "Nintendogs", Description = "Pet care simulation", CoverPicture = "/Assets/Games/nintendogs.png" },
                new { GameId = 10, UserId = 11, GameTitle = "Pet Hotel", Description = "Manage a hotel for pets", CoverPicture = "/Assets/Games/pethotel.png" },
                new { GameId = 11, UserId = 11, GameTitle = "Christmas Wonderland", Description = "Festive hidden object game", CoverPicture = "/Assets/Games/xmas.png" }
            };

            modelBuilder.Entity<OwnedGame>().HasData(ownedGamesSeed);

            // Achievements seed data
            var achievementsSeed = new List<object>
            {
                new { AchievementId = 1, AchievementName = "FRIENDSHIP1", Description = "You made a friend, you get a point", AchievementType = "Friendships", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 2, AchievementName = "FRIENDSHIP2", Description = "You made 5 friends, you get 3 points", AchievementType = "Friendships", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 3, AchievementName = "FRIENDSHIP3", Description = "You made 10 friends, you get 5 points", AchievementType = "Friendships", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 4, AchievementName = "FRIENDSHIP4", Description = "You made 50 friends, you get 10 points", AchievementType = "Friendships", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 5, AchievementName = "FRIENDSHIP5", Description = "You made 100 friends, you get 15 points", AchievementType = "Friendships", Points = 15, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 6, AchievementName = "OWNEDGAMES1", Description = "You own 1 game, you get 1 point", AchievementType = "Owned Games", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 7, AchievementName = "OWNEDGAMES2", Description = "You own 5 games, you get 3 points", AchievementType = "Owned Games", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 8, AchievementName = "OWNEDGAMES3", Description = "You own 10 games, you get 5 points", AchievementType = "Owned Games", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 9, AchievementName = "OWNEDGAMES4", Description = "You own 50 games, you get 10 points", AchievementType = "Owned Games", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 10, AchievementName = "SOLDGAMES1", Description = "You sold 1 game, you get 1 point", AchievementType = "Sold Games", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 11, AchievementName = "SOLDGAMES2", Description = "You sold 5 games, you get 3 points", AchievementType = "Sold Games", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 12, AchievementName = "SOLDGAMES3", Description = "You sold 10 games, you get 5 points", AchievementType = "Sold Games", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 13, AchievementName = "SOLDGAMES4", Description = "You sold 50 games, you get 10 points", AchievementType = "Sold Games", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 14, AchievementName = "REVIEW1", Description = "You gave 1 review, you get 1 point", AchievementType = "Number of Reviews Given", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 15, AchievementName = "REVIEW2", Description = "You gave 5 reviews, you get 3 points", AchievementType = "Number of Reviews Given", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 16, AchievementName = "REVIEW3", Description = "You gave 10 reviews, you get 5 points", AchievementType = "Number of Reviews Given", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 17, AchievementName = "REVIEW4", Description = "You gave 50 reviews, you get 10 points", AchievementType = "Number of Reviews Given", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 18, AchievementName = "REVIEWR1", Description = "You got 1 review, you get 1 point", AchievementType = "Number of Reviews Received", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 19, AchievementName = "REVIEWR2", Description = "You got 5 reviews, you get 3 points", AchievementType = "Number of Reviews Received", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 20, AchievementName = "REVIEWR3", Description = "You got 10 reviews, you get 5 points", AchievementType = "Number of Reviews Received", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 21, AchievementName = "REVIEWR4", Description = "You got 50 reviews, you get 10 points", AchievementType = "Number of Reviews Received", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 22, AchievementName = "DEVELOPER", Description = "You are a developer, you get 10 points", AchievementType = "Developer", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 23, AchievementName = "ACTIVITY1", Description = "You have been active for 1 year, you get 1 point", AchievementType = "Years of Activity", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 24, AchievementName = "ACTIVITY2", Description = "You have been active for 2 years, you get 3 points", AchievementType = "Years of Activity", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 25, AchievementName = "ACTIVITY3", Description = "You have been active for 3 years, you get 5 points", AchievementType = "Years of Activity", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 26, AchievementName = "ACTIVITY4", Description = "You have been active for 4 years, you get 10 points", AchievementType = "Years of Activity", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 27, AchievementName = "POSTS1", Description = "You have made 1 post, you get 1 point", AchievementType = "Number of Posts", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 28, AchievementName = "POSTS2", Description = "You have made 5 posts, you get 3 points", AchievementType = "Number of Posts", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 29, AchievementName = "POSTS3", Description = "You have made 10 posts, you get 5 points", AchievementType = "Number of Posts", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new { AchievementId = 30, AchievementName = "POSTS4", Description = "You have made 50 posts, you get 10 points", AchievementType = "Number of Posts", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" }
            };

            modelBuilder.Entity<Achievement>().HasData(achievementsSeed);
        }
    }
}

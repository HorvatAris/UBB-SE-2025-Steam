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
        }
    }
}

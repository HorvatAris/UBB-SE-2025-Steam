using SteamHub.ApiContract.Models.PasswordReset;
using SteamHub.ApiContract.Models.Session;

namespace SteamHub.Api.Context
{
    using Azure;
    using Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.VisualBasic;
    using Models;
    using System.Reflection.Emit;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Models.Session;
    using Game = SteamHub.Api.Entities.Game;


    public class DataContext : DbContext
    {
        private readonly IConfiguration configuration;

        public DataContext(DbContextOptions options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }

        public DataContext()
        {
        }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<PointShopItem> PointShopItems { get; set; }

        public DbSet<UserPointShopItemInventory> UserPointShopInventories { get; set; }

        public DbSet<UsersGames> UsersGames { get; set; }
        public DbSet<StoreTransaction> StoreTransactions { get; set; }
        public DbSet<ItemTrade> ItemTrades { get; set; }
        public DbSet<UserInventory> UserInventories { get; set; }
        public DbSet<ItemTradeDetail> ItemTradeDetails { get; set; }
        
        public DbSet<SessionDetails> UserSessions { get; set; }
        
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

        // Added From other team
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<ChatConversation> ChatConversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<OwnedGame> OwnedGames { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ReviewsUser> ReviewsUsers { get; set; }
        public DbSet<Post> NewsPosts { get; set; }
        public DbSet<Comment> NewsComments { get; set; }
        public DbSet<PostRatingType> NewsPostRatingTypes { get; set; }
        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<FriendEntity> FriendsTable { get; set; } // Delete this once the relationship functionalities are sorted out
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


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("Default"));
            }
        }  

        protected override void OnModelCreating(ModelBuilder builder)
        {
           builder.Entity<Role>()
                .Property(role => role.Id).ValueGeneratedNever();

            builder.Entity<Role>().HasData(Enum.GetValues(typeof(RoleEnum))
                .Cast<RoleEnum>()
                .Select(role => new Role
                {
                    Id = role,
                    Name = role.ToString()
                }));

            var tagSeed = new List<Tag>
            {
                new Tag { TagId = 1, TagName = "Rogue-Like" },
                new Tag { TagId = 2, TagName = "Third-Person Shooter" },
                new Tag { TagId = 3, TagName = "Multiplayer" },
                new Tag { TagId = 4, TagName = "Horror" },
                new Tag { TagId = 5, TagName = "First-Person Shooter" },
                new Tag { TagId = 6, TagName = "Action" },
                new Tag { TagId = 7, TagName = "Platformer" },
                new Tag { TagId = 8, TagName = "Adventure" },
                new Tag { TagId = 9, TagName = "Puzzle" },
                new Tag { TagId = 10, TagName = "Exploration" },
                new Tag { TagId = 11, TagName = "Sandbox" },
                new Tag { TagId = 12, TagName = "Survival" },
                new Tag { TagId = 13, TagName = "Arcade" },
                new Tag { TagId = 14, TagName = "RPG" },
                new Tag { TagId = 15, TagName = "Racing" },
                new Tag { TagId = 16, TagName = "Action RPG" },
                new Tag { TagId = 17, TagName = "Battle Royale" },
            };

            var usersSeed = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Email = "gabe.newell@valvestudio.com",
                    PointsBalance = 6000,
                    Username = "GabeN",
                    RoleId = RoleEnum.Developer,
                    WalletBalance = 500,
                    Password = "secret",
                    CreatedAt = new DateTime(2024, 1, 1),
                    IsDeveloper = false,
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                },
                new User
                {
                    UserId = 2,
                    Email = "mathias.new@cdprojektred.com",
                    PointsBalance = 5000,
                    Username = "MattN",
                    RoleId = RoleEnum.Developer,
                    WalletBalance = 420,
                    Password = "secret",
                    CreatedAt = new DateTime(2024, 1, 1),
                    IsDeveloper = false,
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                },
                new User
                {
                    UserId = 3,
                    Email = "john.chen@thatgamecompany.com",
                    PointsBalance = 5000,
                    Username = "JohnC",
                    RoleId = RoleEnum.Developer,
                    WalletBalance = 390,
                    Password = "secret",
                    CreatedAt = new DateTime(2024, 1, 1),
                    IsDeveloper = false,
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                },
                new User
                {
                    UserId = 4,
                    Email = "alice.johnson@example.com",
                    PointsBalance = 6000,
                    Username = "AliceJ",
                    RoleId = RoleEnum.User,
                    WalletBalance = 780,
                    Password = "secret",
                    CreatedAt = new DateTime(2024, 1, 1),
                    IsDeveloper = false,
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                },
                new User
                {
                    UserId = 5,
                    Email = "liam.garcia@example.com",
                    PointsBalance = 7000,
                    Username = "LiamG",
                    RoleId = RoleEnum.User,
                    WalletBalance = 5500,
                    Password = "secret",
                    CreatedAt = new DateTime(2024, 1, 1),
                    IsDeveloper = false,
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                },
                new User
                {
                    UserId = 6,
                    Email = "sophie.williams@example.com",
                    PointsBalance = 6000,
                    Username = "SophieW",
                    RoleId = RoleEnum.User,
                    WalletBalance = 950,
                    Password = "secret",
                    CreatedAt = new DateTime(2024, 1, 1),
                    IsDeveloper = false,
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                },
                new User
                {
                    UserId = 7,
                    Email = "noah.smith@example.com",
                    PointsBalance = 4000,
                    Username = "NoahS",
                    RoleId = RoleEnum.User,
                    WalletBalance = 3300,
                    Password = "secret",
                    CreatedAt = new DateTime(2024, 1, 1),
                    IsDeveloper = false,
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                },
                new User
                {
                    UserId = 8,
                    Email = "emily.brown@example.com",
                    PointsBalance = 5000,
                    Username = "EmilyB",
                    RoleId = RoleEnum.User,
                    WalletBalance = 1100,
                    Password = "secret",
                    CreatedAt = new DateTime(2024, 1, 1),
                    IsDeveloper = false,
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                }
            };

            builder.Entity<Tag>().HasData(tagSeed);
            builder.Entity<User>().HasData(usersSeed);




            builder.Entity<GameStatus>()
                .Property(gameStatus => gameStatus.Id).ValueGeneratedNever();

            builder.Entity<GameStatus>().HasData(Enum.GetValues(typeof(GameStatusEnum))
                .Cast<GameStatusEnum>()
                .Select(gameStatus => new GameStatus
                {
                    Id = gameStatus,
                    Name = gameStatus.ToString()
                }));

            var gameSeed = new List<Game>
            {
                new Game
                {
                    GameId = 1,
                    Name = "Risk of Rain 2",
                    Description = "A rogue-like third-person shooter where players fight through hordes of monsters to escape an alien planet.",
                    ImagePath = "https://upload.wikimedia.org/wikipedia/en/c/c1/Risk_of_Rain_2.jpg",
                    Price = 24.99m,
                    MinimumRequirements = "4GB RAM, 2.5GHz Processor, GTX 580",
                    RecommendedRequirements = "8GB RAM, 3.0GHz Processor, GTX 680",
                    StatusId = GameStatusEnum.Rejected,
                    RejectMessage = "Minimum requirements are too high",
                    Rating = 4.2m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=pJ-aR--gScM",
                    GameplayPath = "https://www.youtube.com/watch?v=Cwk3qmD28CE",
                    Discount = 0.20m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 2,
                    Name = "Dead by Daylight",
                    Description = "A multiplayer horror game where survivors must evade a killer.",
                    ImagePath = "https://pbs.twimg.com/media/FOEzJiXX0AcxBTi.jpg",
                    Price = 19.99m,
                    MinimumRequirements = "8GB RAM, i3-4170, GTX 760",
                    RecommendedRequirements = "16GB RAM, i5-6500, GTX 1060",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.8m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=JGhIXLO3ul8",
                    GameplayPath = "https://www.youtube.com/watch?v=3wUHKO0ieyY",
                    Discount = 0.40m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 3,
                    Name = "Counter-Strike 2",
                    Description = "A tactical first-person shooter featuring team-based gameplay.",
                    ImagePath = "https://sm.ign.com/ign_nordic/cover/c/counter-st/counter-strike-2_jc2d.jpg",
                    Price = 20.99m,
                    MinimumRequirements = "8GB RAM, i5-2500K, GTX 660",
                    RecommendedRequirements = "16GB RAM, i7-7700K, GTX 1060",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.9m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=c80dVYcL69E",
                    GameplayPath = "https://www.youtube.com/watch?v=P22HqM9w500",
                    Discount = 0.50m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 4,
                    Name = "Half-Life 2",
                    Description = "A story-driven first-person shooter that revolutionized the genre.",
                    ImagePath = "https://media.moddb.com/images/mods/1/47/46951/d1jhx20-dc797b78-5feb-4005-b206-.1.jpg",
                    Price = 9.99m,
                    MinimumRequirements = "512MB RAM, 1.7GHz Processor, DirectX 8 GPU",
                    RecommendedRequirements = "1GB RAM, 3.0GHz Processor, DirectX 9 GPU",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.1m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=UKA7JkV51Jw",
                    GameplayPath = "https://www.youtube.com/watch?v=jElU1mD8JnI",
                    Discount = 0.60m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 5,
                    Name = "Mario",
                    Description = "A classic platformer adventure with iconic characters and worlds.",
                    ImagePath = "https://play-lh.googleusercontent.com/3ZKfMRp_QrdN-LzsZTbXdXBH-LS1iykSg9ikNq_8T2ppc92ltNbFxS-tORxw2-6kGA",
                    Price = 59.99m,
                    MinimumRequirements = "N/A",
                    RecommendedRequirements = "N/A",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 5.0m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=TnGl01FkMMo",
                    GameplayPath = "https://www.youtube.com/watch?v=rLl9XBg7wSs",
                    Discount = 0.70m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 6,
                    Name = "The Legend of Zelda",
                    Description = "An epic adventure game where heroes save the kingdom of Hyrule.",
                    ImagePath = "https://m.media-amazon.com/images/I/71oHNyzdN1L.jpg",
                    Price = 59.99m,
                    MinimumRequirements = "N/A",
                    RecommendedRequirements = "N/A",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.5m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=_X2h3SF7gd4",
                    GameplayPath = "https://www.youtube.com/watch?v=wW7jkBJ_yK0",
                    Discount = 0.30m,
                    PublisherUserId = usersSeed[0].UserId
                },
                new Game
                {
                    GameId = 7,
                    Name = "Baba Is You",
                    Description = "A puzzle game where you change the rules to solve challenges.",
                    ImagePath = "https://is5-ssl.mzstatic.com/image/thumb/Purple113/v4/9e/30/61/9e3061a5-b2f0-87ad-9e90-563f37729be5/source/256x256bb.jpg",
                    Price = 14.99m,
                    MinimumRequirements = "2GB RAM, 1.0GHz Processor",
                    RecommendedRequirements = "4GB RAM, 2.0GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 3.9m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=z3_yA4HTJfs",
                    GameplayPath = "https://www.youtube.com/watch?v=dAiX8s-Eu7w",
                    Discount = 0.20m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 8,
                    Name = "Portal 2",
                    Description = "A mind-bending puzzle-platformer with a dark sense of humor.",
                    ImagePath = "https://cdn2.steamgriddb.com/icon_thumb/0994c8d1d6bc62cc56e9112d2303266b.png",
                    Price = 9.99m,
                    MinimumRequirements = "2GB RAM, 1.7GHz Processor, DirectX 9 GPU",
                    RecommendedRequirements = "4GB RAM, 3.0GHz Processor, GTX 760",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.2m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=tax4e4hBBZc",
                    GameplayPath = "https://www.youtube.com/watch?v=ts-j0nFf2e0",
                    Discount = 0.10m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 9,
                    Name = "Outer Wilds",
                    Description = "An exploration-based game where you unravel cosmic mysteries.",
                    ImagePath = "https://images.nintendolife.com/62a79995ed766/outer-wilds-echoes-of-the-eye-cover.cover_large.jpg",
                    Price = 24.99m,
                    MinimumRequirements = "6GB RAM, i5-2300, GTX 560",
                    RecommendedRequirements = "8GB RAM, i7-6700, GTX 970",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.8m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=d9u6KYVq5kw",
                    GameplayPath = "https://www.youtube.com/watch?v=huL_TawYrMs",
                    Discount = 0.15m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 10,
                    Name = "Hades",
                    Description = "A rogue-like dungeon crawler where you defy the god of the dead.",
                    ImagePath = "https://image.api.playstation.com/vulcan/ap/rnd/202104/0517/9AcM3vy5t77zPiJyKHwRfnNT.png",
                    Price = 24.99m,
                    MinimumRequirements = "4GB RAM, Dual Core 2.4GHz, Intel HD 5000",
                    RecommendedRequirements = "8GB RAM, Dual Core 3.0GHz, GTX 760",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.0m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=91sW0DMkZzI",
                    GameplayPath = "https://www.youtube.com/watch?v=4fVO0qUBe4E",
                    Discount = 0.20m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 11,
                    Name = "Slay the Spire",
                    Description = "A deck-building rogue-like where strategy is key to survival.",
                    ImagePath = "https://image.api.playstation.com/cdn/EP3717/CUSA15338_00/Sn5xbNutqfQdWYIjbeCIN0bwTJOV7UPG.png",
                    Price = 19.99m,
                    MinimumRequirements = "2GB RAM, 2.0GHz Processor",
                    RecommendedRequirements = "4GB RAM, 3.0GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.0m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=75qT5KOs-Ew",
                    GameplayPath = "https://www.youtube.com/watch?v=JO3EIPtw-4I",
                    Discount = 0.25m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 12,
                    Name = "Celeste",
                    Description = "A platformer about climbing a mountain and facing inner demons.",
                    ImagePath = "https://images.nintendolife.com/ef02c2e24c59e/celeste-cover.cover_large.jpg",
                    Price = 19.99m,
                    MinimumRequirements = "2GB RAM, 2.0GHz Processor",
                    RecommendedRequirements = "4GB RAM, 2.4GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 3.8m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=iofYDsP2vhQ",
                    GameplayPath = "https://www.youtube.com/watch?v=FfRjHZWSYqY",
                    Discount = 0.30m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 13,
                    Name = "Hollow Knight",
                    Description = "An action-adventure game set in a beautifully animated underground world.",
                    ImagePath = "https://image.api.playstation.com/cdn/EP1805/CUSA13285_00/DmwPWlU0468FbsjrtI92FhQz1xBYMoog.png",
                    Price = 14.99m,
                    MinimumRequirements = "4GB RAM, 2.0GHz Processor",
                    RecommendedRequirements = "8GB RAM, 3.2GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.1m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=UAO2urG23S4",
                    GameplayPath = "https://www.youtube.com/watch?v=UAO2urG23S4",
                    Discount = 0.35m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 14,
                    Name = "Stardew Valley",
                    Description = "A farming simulator RPG where you build a life in the countryside.",
                    ImagePath = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQHWjybGuWhdyJqjmtziGvtHvCnQf23yY0R6g&s",
                    Price = 14.99m,
                    MinimumRequirements = "2GB RAM, 2.0GHz Processor",
                    RecommendedRequirements = "4GB RAM, 3.0GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.1m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=ot7uXNQskhs",
                    GameplayPath = "https://www.youtube.com/watch?v=ot7uXNQskhs",
                    Discount = 0.20m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 15,
                    Name = "Minecraft",
                    Description = "A sandbox game that lets you build and explore infinite worlds.",
                    ImagePath = "https://cdn2.steamgriddb.com/icon/f0b57183da91a7972b2b3c06b0db5542/32/512x512.png",
                    Price = 29.99m,
                    MinimumRequirements = "4GB RAM, Intel HD 4000",
                    RecommendedRequirements = "8GB RAM, GTX 1060",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.8m,
                    NumberOfRecentPurchases = 1420,
                    TrailerPath = "https://www.youtube.com/watch?v=MmB9b5njVbA",
                    GameplayPath = "https://www.youtube.com/watch?v=ANgI2o_Jinc",
                    Discount = 0.14m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 16,
                    Name = "Don't Starve",
                    Description = "A survival game in a dark and whimsical world filled with strange creatures.",
                    ImagePath = "https://image.api.playstation.com/cdn/EP2107/CUSA00327_00/i5qwqMWJj33IIr2m9TM29GQNnFCi4ZqI.png?w=440",
                    Price = 9.99m,
                    MinimumRequirements = "1.7GHz Processor, 1GB RAM",
                    RecommendedRequirements = "2.0GHz Processor, 2GB RAM",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 3.6m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=ochPlhMFk84",
                    GameplayPath = "https://www.youtube.com/watch?v=htXgxyLpPMg",
                    Discount = 0.25m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 17,
                    Name = "Cuphead",
                    Description = "A classic run and gun game with hand-drawn animations and tough bosses.",
                    ImagePath = "https://upload.wikimedia.org/wikipedia/en/thumb/e/eb/Cuphead_%28artwork%29.png/250px-Cuphead_%28artwork%29.png",
                    Price = 19.99m,
                    MinimumRequirements = "3GB RAM, Intel Core2 Duo E8400",
                    RecommendedRequirements = "4GB RAM, i3-3240",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.8m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=NN-9SQXoi50",
                    GameplayPath = "https://www.youtube.com/watch?v=DNIMD8ZpMSQ",
                    Discount = 0.40m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 18,
                    Name = "Limbo",
                    Description = "A black-and-white puzzle platformer with a haunting atmosphere.",
                    ImagePath = "https://image.api.playstation.com/cdn/EP2054/CUSA01369_00/W45kellY9yrwSDpmQEL9tFqZQW7N4FEz.png?w=440",
                    Price = 9.99m,
                    MinimumRequirements = "512MB RAM, 1.5GHz Processor",
                    RecommendedRequirements = "2GB RAM, 2.0GHz Processor",
                    StatusId = GameStatusEnum.Pending,
                    RejectMessage = null,
                    Rating = 4.6m,
                    NumberOfRecentPurchases = 0,
                    TrailerPath = "https://www.youtube.com/watch?v=Y4HSyVXKYz8",
                    GameplayPath = "https://www.youtube.com/watch?v=dYeuLZY7fZk",
                    Discount = 0.30m,
                    PublisherUserId = usersSeed[1].UserId
                },
                new Game
                {
                    GameId = 19,
                    Name = "Cyberstrike 2077",
                    Description = "A futuristic open-world RPG where you explore the neon-lit streets of Nightcity.",
                    ImagePath = "https://upload.wikimedia.org/wikipedia/en/9/9f/Cyberpunk_2077_box_art.jpg",
                    Price = 59.99m,
                    MinimumRequirements = "Intel i5-3570K, 8GB RAM, GTX 780",
                    RecommendedRequirements = "Intel i7-4790, 12GB RAM, GTX 1060",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.2m,
                    NumberOfRecentPurchases = 950,
                    TrailerPath = "https://www.youtube.com/watch?v=FknHjl7eQ6o",
                    GameplayPath = "https://www.youtube.com/watch?v=8X2kIfS6fb8",
                    Discount = 0.25m,
                    PublisherUserId = usersSeed[2].UserId
                },
                new Game
                {
                    GameId = 20,
                    Name = "Shadow of Valhalla",
                    Description = "Immerse yourself in the Viking age in this brutal and breathtaking action RPG.",
                    ImagePath = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQDtZKyDW9Jrnh8ix-Y38qG5fddbUgYEW7yxA&s",
                    Price = 44.99m,
                    MinimumRequirements = "Intel i5-4460, 8GB RAM, GTX 960",
                    RecommendedRequirements = "Intel i7-6700K, 16GB RAM, GTX 1080",
                    StatusId = GameStatusEnum.Approved,
                    RejectMessage = null,
                    Rating = 4.5m,
                    NumberOfRecentPurchases = 780,
                    TrailerPath = "https://www.youtube.com/watch?v=ssrNcwxALS4",
                    GameplayPath = "https://www.youtube.com/watch?v=gncB1_e9n8E",
                    Discount = 0.10m,
                    PublisherUserId = usersSeed[2].UserId
                }
            };

            builder.Entity<Game>().HasData(gameSeed);

            builder.Entity<Game>()
                .HasMany(game => game.Tags)
                .WithMany(tag => tag.Games)
                .UsingEntity<Dictionary<string, object>>("GameTag",
                    x => x.HasData(
                            // Risk of Rain 2
                            new { GamesGameId = gameSeed[0].GameId, TagsTagId = tagSeed[0].TagId }, // Rogue-Like
                            new { GamesGameId = gameSeed[0].GameId, TagsTagId = tagSeed[1].TagId }, // Third-Person Shooter

                            // Dead by Daylight
                            new { GamesGameId = gameSeed[1].GameId, TagsTagId = tagSeed[2].TagId }, // Multiplayer
                            new { GamesGameId = gameSeed[1].GameId, TagsTagId = tagSeed[3].TagId }, // Horror

                            // Counter-Strike 2
                            new { GamesGameId = gameSeed[2].GameId, TagsTagId = tagSeed[2].TagId }, // Multiplayer
                            new { GamesGameId = gameSeed[2].GameId, TagsTagId = tagSeed[4].TagId }, // FPS
                            new { GamesGameId = gameSeed[2].GameId, TagsTagId = tagSeed[5].TagId }, // Action

                            // Half-Life 2
                            new { GamesGameId = gameSeed[3].GameId, TagsTagId = tagSeed[4].TagId }, // FPS
                            new { GamesGameId = gameSeed[3].GameId, TagsTagId = tagSeed[7].TagId }, // Adventure

                            // Mario
                            new { GamesGameId = gameSeed[4].GameId, TagsTagId = tagSeed[6].TagId }, // Platformer
                            new { GamesGameId = gameSeed[4].GameId, TagsTagId = tagSeed[7].TagId }, // Adventure

                            // Zelda
                            new { GamesGameId = gameSeed[5].GameId, TagsTagId = tagSeed[7].TagId }, // Adventure
                            new { GamesGameId = gameSeed[5].GameId, TagsTagId = tagSeed[13].TagId }, // RPG

                            // Baba Is You
                            new { GamesGameId = gameSeed[6].GameId, TagsTagId = tagSeed[8].TagId }, // Puzzle

                            // Portal 2
                            new { GamesGameId = gameSeed[7].GameId, TagsTagId = tagSeed[8].TagId }, // Puzzle
                            new { GamesGameId = gameSeed[7].GameId, TagsTagId = tagSeed[7].TagId }, // Adventure

                            // Outer Wilds
                            new { GamesGameId = gameSeed[8].GameId, TagsTagId = tagSeed[9].TagId }, // Exploration
                            new { GamesGameId = gameSeed[8].GameId, TagsTagId = tagSeed[7].TagId }, // Adventure

                            // Hades
                            new { GamesGameId = gameSeed[9].GameId, TagsTagId = tagSeed[5].TagId }, // Action
                            new { GamesGameId = gameSeed[9].GameId, TagsTagId = tagSeed[0].TagId }, // Rogue-Like

                            // Slay the Spire
                            new { GamesGameId = gameSeed[10].GameId, TagsTagId = tagSeed[0].TagId }, // Rogue-Like
                            new { GamesGameId = gameSeed[10].GameId, TagsTagId = tagSeed[8].TagId }, // Puzzle

                            // Celeste
                            new { GamesGameId = gameSeed[11].GameId, TagsTagId = tagSeed[6].TagId }, // Platformer

                            // Hollow Knight
                            new { GamesGameId = gameSeed[12].GameId, TagsTagId = tagSeed[5].TagId },  // Action
                            new { GamesGameId = gameSeed[12].GameId, TagsTagId = tagSeed[6].TagId },  // Platformer

                            // Stardew Valley
                            new { GamesGameId = gameSeed[13].GameId, TagsTagId = tagSeed[11].TagId }, // Survival
                            new { GamesGameId = gameSeed[13].GameId, TagsTagId = tagSeed[7].TagId },  // Adventure

                            // Minecraft
                            new { GamesGameId = gameSeed[14].GameId, TagsTagId = tagSeed[10].TagId }, // Sandbox
                            new { GamesGameId = gameSeed[14].GameId, TagsTagId = tagSeed[11].TagId }, // Survival

                            // Don't Starve
                            new { GamesGameId = gameSeed[15].GameId, TagsTagId = tagSeed[11].TagId }, // Survival
                            new { GamesGameId = gameSeed[15].GameId, TagsTagId = tagSeed[7].TagId },  // Adventure

                            // Cuphead
                            new { GamesGameId = gameSeed[16].GameId, TagsTagId = tagSeed[5].TagId },  // Action
                            new { GamesGameId = gameSeed[16].GameId, TagsTagId = tagSeed[6].TagId },  // Platformer

                            // Limbo
                            new { GamesGameId = gameSeed[17].GameId, TagsTagId = tagSeed[8].TagId },  // Puzzle
                            new { GamesGameId = gameSeed[17].GameId, TagsTagId = tagSeed[6].TagId },  // Platformer

                            // Cyberstrike 2077
                            new { GamesGameId = gameSeed[18].GameId, TagsTagId = tagSeed[5].TagId },   // Action
                            new { GamesGameId = gameSeed[18].GameId, TagsTagId = tagSeed[13].TagId },  // RPG
                            new { GamesGameId = gameSeed[18].GameId, TagsTagId = tagSeed[15].TagId },  // Action RPG
    
                            // Shadow of Valhalla
                            new { GamesGameId = gameSeed[19].GameId, TagsTagId = tagSeed[15].TagId }  // Action RPG
                        ));

            builder.Entity<Item>()
                .HasOne(item => item.Game)
                .WithMany(game => game.Items)
                .HasForeignKey(item => item.CorrespondingGameId);
            
            var itemsSeed =  new List<Item>
            {
                // Items for Counter-Strike 2
                new Item
                {
                    ItemId = 1,
                    ItemName = "AK-47 | Redline Skin",
                    CorrespondingGameId = 3,
                    Price = 29.99f,
                    Description = "A sleek and aggressive finish for your AK-47.",
                    IsListed = true,
                    ImagePath = "https://steamcdn-a.akamaihd.net/apps/730/icons/econ/default_generated/weapon_ak47_cu_ak47_cobra_light_large.7494bfdf4855fd4e6a2dbd983ed0a243c80ef830.png"
                },
                new Item
                {
                    ItemId = 2,
                    ItemName = "Desert Eagle | Blaze Skin",
                    CorrespondingGameId = 3,
                    Price = 34.99f,
                    Description = "Legendary pistol skin with a fiery design.",
                    IsListed = true,
                    ImagePath = "https://steamcdn-a.akamaihd.net/apps/730/icons/econ/default_generated/weapon_deagle_aa_flames_light_large.dd140c3b359c16ccd8e918ca6ad0b2628151fe1c.png"
                },

                // Items for Half-Life 2
                new Item
                {
                    ItemId = 3,
                    ItemName = "Gravity Gun Replica",
                    CorrespondingGameId = 4,
                    Price = 49.99f,
                    Description = "Iconic weapon that manipulates objects with physics.",
                    IsListed = true,
                    ImagePath = "https://www.toyark.com/wp-content/uploads/2013/05/Half-Life-2-Gravity-Gun-007.jpg"
                },
                new Item
                {
                    ItemId = 4,
                    ItemName = "HEV Suit Gloves",
                    CorrespondingGameId = 4,
                    Price = 19.99f,
                    Description = "Protective gloves from the HEV suit worn by Gordon Freeman.",
                    IsListed = true,
                    ImagePath = "https://preview.redd.it/hl2-revision-update-the-grabbity-gloves-v0-ftz143vjmqcb1.jpg?width=640&crop=smart&auto=webp&s=9b3738a0f4bce98cc6a38b34e6ec319d03c05dd0"
                },

                // Items for Mario - 5
                new Item
                {
                    ItemId = 5,
                    ItemName = "Fire Flower",
                    CorrespondingGameId = 5,
                    Price = 14.99f,
                    Description = "A soft collectible version of the iconic power-up.",
                    IsListed = false,
                    ImagePath = "https://mario.wiki.gallery/images/thumb/7/7e/New_Super_Mario_Bros._U_Deluxe_Fire_Flower.png/1200px-New_Super_Mario_Bros._U_Deluxe_Fire_Flower.png"
                },
                new Item
                {
                    ItemId = 6,
                    ItemName = "Mario Cap",
                    CorrespondingGameId = 5,
                    Price = 24.99f,
                    Description = "The classic red cap worn by Mario himself.",
                    IsListed = false,
                    ImagePath = "https://static.wikia.nocookie.net/mario/images/c/cd/Mario_Cap.png/revision/latest?cb=20180310022043"
                },

                // Items for Zelda - 6
                new Item
                {
                    ItemId = 7,
                    ItemName = "Master Sword Replica",
                    CorrespondingGameId = 6,
                    Price = 69.99f,
                    Description = "Faithful replica of Link's legendary blade.",
                    IsListed = false,
                    ImagePath = "https://upload.wikimedia.org/wikipedia/en/f/f9/Master_Sword_Lead.png"
                },
                new Item
                {
                    ItemId = 8,
                    ItemName = "Hylian Shield",
                    CorrespondingGameId = 6,
                    Price = 59.99f,
                    Description = "Sturdy shield bearing the crest of Hyrule.",
                    IsListed = false,
                    ImagePath = "https://theswordstall.co.uk/cdn/shop/files/Legend-Of-Zelda-Deluxe-Hylian-Shield-Full-Metal-3.jpg?v=1723552799&width=750"
                },

                // Items for Minecraft - 15
                new Item
                {
                    ItemId = 9,
                    ItemName = "Diamond Pickaxe",
                    CorrespondingGameId = 15,
                    Price = 9.99f,
                    Description = "Miniature version of the famous mining tool.",
                    IsListed = false,
                    ImagePath = "https://static.posters.cz/image/1300/merch/replica-minecraft-diamond-pickaxe-i94007.jpg"
                },
                new Item
                {
                    ItemId = 10,
                    ItemName = "Creeper Plush",
                    CorrespondingGameId = 15,
                    Price = 19.99f,
                    Description = "Soft plush of the infamous explosive mob.",
                    IsListed = false,
                    ImagePath = "https://feltright.com/cdn/shop/files/minecraft-creeper.jpg?v=1720033057&width=800"
                },


                // Items for Cyberstrike 2077
                new Item
                {
                    ItemId = 11,
                    ItemName = "Cybernetic Gauntlet",
                    CorrespondingGameId = 19,
                    Price = 34.99f,
                    Description = "A high-tech gauntlet to hack and crush foes in Cyberstrike 2077.",
                    IsListed = true,
                    ImagePath = "https://static.wikia.nocookie.net/shop-heroes/images/4/4a/Gauntlets_Cybernetic_Gauntlets_Blueprint.png/revision/latest?cb=20200724020856"
                },
                new Item
                {
                    ItemId = 12,
                    ItemName = "Neon Visor",
                    CorrespondingGameId = 19,
                    Price = 24.99f,
                    Description = "A visor that enhances your vision in the neon-lit battles of Cyberstrike 2077.",
                    IsListed = false,
                    ImagePath = "https://www.motocentral.co.uk/cdn/shop/files/Ruroc-EOX-Cyberstrike_-From-Moto-Central-_-Fast-Free-UK-Delivery-257043288_1024x.jpg?v=1744036882"
                },

                // Items for Shadow of Valhalla
                new Item
                {
                    ItemId = 13,
                    ItemName = "Viking Axe",
                    CorrespondingGameId = 20,
                    Price = 44.99f,
                    Description = "A mighty axe for the warriors of Shadow of Valhalla.",
                    IsListed = false,
                    ImagePath = "https://valhalla-vikings.co.uk/cdn/shop/products/il_fullxfull.3370240260_td4v.jpg?v=1679150085&width=1080"
                },
                new Item
                {
                    ItemId = 14,
                    ItemName = "Valhalla Shield",
                    CorrespondingGameId = 20,
                    Price = 34.99f,
                    Description = "A robust shield forged for the bravest of fighters in Shadow of Valhalla.",
                    IsListed = true,
                    ImagePath = "https://www.vikingsroar.com/cdn/shop/products/d7f00df1f2c5a9059ec5dd319139da24.webp?v=1652049514"
                }
            };
            builder.Entity<Item>().HasData(itemsSeed);

              
            var pointShopItemsSeed = new List<PointShopItem>
            {
                new PointShopItem {
                    PointShopItemId = 1,
                    Name = "Blue Profile Background",
                    Description = "A cool blue background for your profile",
                    ImagePath = "https://picsum.photos/id/1/200/200",
                    PointPrice = 1000,
                    ItemType = "ProfileBackground"
                },
                new PointShopItem {
                    PointShopItemId = 2,
                    Name = "Red Profile Background",
                    Description = "A vibrant red background for your profile",
                    ImagePath = "https://picsum.photos/id/20/200/200",
                    PointPrice = 1000,
                    ItemType = "ProfileBackground"
                },
                new PointShopItem {
                    PointShopItemId = 3,
                    Name = "Golden Avatar Frame",
                    Description = "A golden frame for your avatar image",
                    ImagePath = "https://picsum.photos/id/30/200/200",
                    PointPrice = 2000,
                    ItemType = "AvatarFrame"
                },
                new PointShopItem {
                    PointShopItemId = 4,
                    Name = "Silver Avatar Frame",
                    Description = "A silver frame for your avatar image",
                    ImagePath = "https://picsum.photos/id/40/200/200",
                    PointPrice = 1500,
                    ItemType = "AvatarFrame"
                },
                new PointShopItem {
                    PointShopItemId = 5,
                    Name = "Happy Emoticon",
                    Description = "Express yourself with this happy emoticon",
                    ImagePath = "https://picsum.photos/id/50/200/200",
                    PointPrice = 500,
                    ItemType = "Emoticon"
                },
                new PointShopItem {
                    PointShopItemId = 6,
                    Name = "Sad Emoticon",
                    Description = "Express yourself with this sad emoticon",
                    ImagePath = "https://picsum.photos/id/60/200/200",
                    PointPrice = 500,
                    ItemType = "Emoticon"
                },
                new PointShopItem {
                    PointShopItemId = 7,
                    Name = "Gamer Avatar",
                    Description = "Cool gamer avatar for your profile",
                    ImagePath = "https://picsum.photos/id/70/200/200",
                    PointPrice = 1200,
                    ItemType = "Avatar"
                },
                new PointShopItem {
                    PointShopItemId = 8,
                    Name = "Ninja Avatar",
                    Description = "Stealthy ninja avatar for your profile",
                    ImagePath = "https://picsum.photos/id/80/200/200",
                    PointPrice = 1200,
                    ItemType = "Avatar"
                },
                new PointShopItem {
                    PointShopItemId = 9,
                    Name = "Space Mini-Profile",
                    Description = "Space-themed mini profile",
                    ImagePath = "https://picsum.photos/id/90/200/200",
                    PointPrice = 3000,
                    ItemType = "MiniProfile"
                },
                new PointShopItem {
                    PointShopItemId = 10,
                    Name = "Fantasy Mini-Profile",
                    Description = "Fantasy-themed mini profile",
                    ImagePath = "https://picsum.photos/id/100/200/200",
                    PointPrice = 3000,
                    ItemType = "MiniProfile"
                }
            };

            builder.Entity<PointShopItem>().HasData(pointShopItemsSeed);

            var userInventorySeed = new List<UserPointShopItemInventory>
            {
                new UserPointShopItemInventory
                {
                    UserId = 4,
                    PointShopItemId = 1,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 4,
                    PointShopItemId = 2,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = true
                },
                new UserPointShopItemInventory
                {
                    UserId = 4,
                    PointShopItemId = 5,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 5,
                    PointShopItemId = 2,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = true
                },
                new UserPointShopItemInventory
                {
                    UserId = 5,
                    PointShopItemId = 6,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 6,
                    PointShopItemId = 3,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 7,
                    PointShopItemId = 4,
                    PurchaseDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = true
                }
            };

            builder.Entity<UserPointShopItemInventory>().HasData(userInventorySeed);

            builder.Entity<UsersGames>()
                .HasOne(userGames => userGames.User)
                .WithMany()
                .HasForeignKey(userGames => userGames.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UsersGames>()
               .HasOne(userGames => userGames.Game)
               .WithMany()
               .HasForeignKey(userGames => userGames.GameId)
               .OnDelete(DeleteBehavior.Cascade);

           var usersGamesSeed = new List<UsersGames>
            {
                new UsersGames
                {
                    UserId = 4,
                    GameId = 3,
                    IsInWishlist = true,
                    IsPurchased = false,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 4,
                    GameId = 5,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 4,
                    GameId = 6,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 4,
                    GameId = 15,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 5,
                    GameId = 5,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 5,
                    GameId = 6,
                    IsInWishlist = false,
                    IsPurchased = false,
                    IsInCart = true
                },
                new UsersGames
                {
                    UserId = 5,
                    GameId = 19,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 6,
                    GameId = 20,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 7,
                    GameId = 20,
                    IsInWishlist = false,
                    IsPurchased = true,
                    IsInCart = false
                },
                new UsersGames
                {
                    UserId = 8,
                    GameId = 15,
                    IsInWishlist = false,
                    IsPurchased = false,
                    IsInCart = true
                },
            };

            builder.Entity<UsersGames>().HasData(usersGamesSeed);

            builder.Entity<StoreTransaction>()
                .HasOne(storeTransaction => storeTransaction.User)
                .WithMany(users => users.StoreTransactions)
                .HasForeignKey(storeTransaction => storeTransaction.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StoreTransaction>()
                .HasOne(storeTransaction => storeTransaction.Game)
                .WithMany(game => game.StoreTransactions)
                .HasForeignKey(storeTransaction => storeTransaction.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            var storeTransactionsSeed = new List<StoreTransaction>
            {
                new StoreTransaction
                {
                    StoreTransactionId = 1,
                    UserId = 4,
                    GameId = 5,
                    Date = new DateTime(2025, 4, 27, 14, 30, 0),
                    Amount = (float)14.99,
                    WithMoney = true
                },
                new StoreTransaction
                {
                    StoreTransactionId = 2,
                    UserId = 7,
                    GameId = 20,
                    Date = new DateTime(2025, 4, 27, 14, 30, 0),
                    Amount = (float)34.99,
                    WithMoney = false
                },
                new StoreTransaction
                {
                    StoreTransactionId = 3,
                    UserId = 4,
                    GameId = 15,
                    Date = new DateTime(2025, 4, 27, 14, 30, 0),
                    Amount = (float)29.99,
                    WithMoney = true
                },
            };

            builder.Entity<StoreTransaction>().HasData(storeTransactionsSeed);

            builder.Entity<ItemTrade>()
                .HasOne(itemTrade => itemTrade.SourceUser)
                .WithMany()
                .HasForeignKey(itemTrade => itemTrade.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ItemTrade>()
                .HasOne(itemTrade => itemTrade.DestinationUser)
                .WithMany()
                .HasForeignKey(itemTrade => itemTrade.DestinationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<ItemTrade>()
                .HasOne(itemTrade => itemTrade.GameOfTrade)
                .WithMany()
                .HasForeignKey(itemTrade => itemTrade.GameOfTradeId)
                .OnDelete(DeleteBehavior.Cascade);

            var itemTradesSeed = new List<ItemTrade>
            {
                new ItemTrade
                {
                    TradeId = 1,
                    SourceUserId = 4,
                    DestinationUserId = 8,
                    GameOfTradeId = 6,
                    TradeDescription = "Trade 1: AliceJ offers Legend of Zelda to EmilyB",
                    TradeDate = new DateTime(2025, 4, 28),
                    TradeStatus = TradeStatus.Pending,
                    AcceptedBySourceUser = false,
                    AcceptedByDestinationUser = false
                },
                new ItemTrade
                {
                    TradeId = 2,
                    SourceUserId = 5,
                    DestinationUserId = 4,
                    GameOfTradeId = 19,
                    TradeDescription = "Trade 2: LiamG offers Cyberstrike 2077 to AliceJ",
                    TradeDate = new DateTime(2025, 4, 28),
                    TradeStatus = TradeStatus.Pending,
                    AcceptedBySourceUser = true,
                    AcceptedByDestinationUser = false
                },
                new ItemTrade
                {
                    TradeId = 3,
                    SourceUserId = 7,
                    DestinationUserId = 6,
                    GameOfTradeId = 20,
                    TradeDescription = "Trade 3: NoahS offers Shadow of Valhalla to SophieW",
                    TradeDate = new DateTime(2025, 4, 28),
                    TradeStatus = TradeStatus.Completed,
                    AcceptedBySourceUser = true,
                    AcceptedByDestinationUser = true
                },
            };

            builder.Entity<ItemTrade>().HasData(itemTradesSeed);

            var userInventoryTableSeed = new List<UserInventory>
            {
                new UserInventory
                {
                    UserId = 4,
                    ItemId = 5,
                    GameId = 5,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 4,
                    ItemId = 7,
                    GameId = 6,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 4,
                    ItemId = 9,
                    GameId = 15,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 4,
                    ItemId = 10,
                    GameId = 15,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 5,
                    ItemId = 6,
                    GameId = 5,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 5,
                    ItemId = 8,
                    GameId = 6,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 5,
                    ItemId = 12,
                    GameId = 19,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 6,
                    ItemId = 13,
                    GameId = 20,
                    AcquiredDate = new DateTime(2025, 4, 27, 14, 30, 0),
                    IsActive = false,
                },
            };

            // have the delete cascaded only for games
            builder.Entity<UserInventory>()
                .HasOne(userInventory => userInventory.Item)
                .WithMany()
                .HasForeignKey(userInventory => userInventory.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserInventory>()
                .HasOne(userInventory => userInventory.User)
                .WithMany()
                .HasForeignKey(userInventory => userInventory.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserInventory>().HasData(userInventoryTableSeed);

            builder.Entity<ItemTradeDetail>()
            .HasKey(itemTradeDetails => new { itemTradeDetails.TradeId, itemTradeDetails.ItemId });

            builder.Entity<ItemTradeDetail>()
                .HasOne(itemTradeDetails => itemTradeDetails.ItemTrade)
                .WithMany(itemTrade => itemTrade.ItemTradeDetails)
                .HasForeignKey(itemTradeDetails => itemTradeDetails.TradeId)
                .OnDelete(DeleteBehavior.Restrict);

            var itemTradeDetailsSeed = new List<ItemTradeDetail>
            {
                new ItemTradeDetail { TradeId = 1, ItemId = 7, IsSourceUserItem = true },
                new ItemTradeDetail { TradeId = 2, ItemId = 12, IsSourceUserItem = true },
                new ItemTradeDetail { TradeId = 3, ItemId = 13, IsSourceUserItem = false},
            };

            builder.Entity<ItemTradeDetail>().HasData(itemTradeDetailsSeed);

            // -- SessionDetails mapping (UserSessions) -------------------------------------
            builder.Entity<SessionDetails>(entity =>
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

            builder.Entity<Collection>()
     .ToTable(tb => tb.HasTrigger("SomeTrigger"));
            builder.Entity<OwnedGame>()
                .ToTable(tb => tb.HasTrigger("SomeTrigger"));
            // Exclude non-entity models (no corresponding tables)
            builder.Ignore<Friend>();
            builder.Ignore<Game>();
            builder.Ignore<PostDisplay>();
            builder.Ignore<AchievementWithStatus>();
            builder.Ignore<AchievementUnlockedData>();
            builder.Ignore<CommentDisplay>();

            // Configure entities here

            // -- ReviewsUser mapping ---------------------------------------------------
            builder.Entity<ReviewsUser>(entity =>
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
            builder.Entity<SoldGame>(entity =>
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
            builder.Entity<CollectionGame>(entity =>
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
            builder.Entity<Feature>(entity =>
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
            builder.Entity<FeatureUser>(entity =>
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
            builder.Entity<ForumPost>(entity =>
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
            builder.Entity<ForumComment>(entity =>
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
            builder.Entity<UserLikedPost>(entity =>
            {
                entity.ToTable("UserLikedPost");
                entity.HasKey(e => new { e.UserId, e.PostId });
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.PostId).HasColumnName("post_id");
            });

            // -- UserDislikedPost mapping ---------------------------------------------------
            builder.Entity<UserDislikedPost>(entity =>
            {
                entity.ToTable("UserDislikedPost");
                entity.HasKey(e => new { e.UserId, e.PostId });
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.PostId).HasColumnName("post_id");
            });

            // -- UserLikedComment mapping ----------------------------------------------------
            builder.Entity<UserLikedComment>(entity =>
            {
                entity.ToTable("UserLikedComment");
                entity.HasKey(e => new { e.UserId, e.CommentId });
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.CommentId).HasColumnName("comment_id");
            });

            // -- UserDislikedComment mapping -----------------------------------------------------
            builder.Entity<UserDislikedComment>(entity =>
            {
                entity.ToTable("UserDislikedComment");
                entity.HasKey(e => new { e.UserId, e.CommentId });
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.CommentId).HasColumnName("comment_id");
            });

            // -- Friend mapping ---------------------------------------------------------
            /* DELETE ONCE FRIENDS FUNCTIONALITY IS SORTED OUT */
            builder.Entity<FriendEntity>(entity =>
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
            builder.Entity<FriendRequest>(entity =>
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
            builder.Entity<Post>(entity =>
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
            builder.Entity<Comment>(entity =>
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
            builder.Entity<PostRatingType>(entity =>
            {
                entity.ToTable("NewsRatings", "dbo");
                entity.HasKey(r => new { r.PostId, r.AuthorId });
                entity.Property(r => r.PostId).HasColumnName("postId");
                entity.Property(r => r.AuthorId).HasColumnName("authorId");
                entity.Property(r => r.RatingType).HasColumnName("ratingType");
            });

            // -- PasswordResetCode mapping -----------------------------------------------
            builder.Entity<PasswordResetCode>(entity =>
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
            builder.Entity<Review>(entity =>
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
                entity.Ignore(r => r.Username);
                entity.Ignore(r => r.TitleOfGame);
                entity.Ignore(r => r.ProfilePictureBlob);
                entity.Ignore(r => r.HasVotedHelpful);
                entity.Ignore(r => r.HasVotedFunny);
            });

            // -- OwnedGame mapping ---------------------------------------------------------
            builder.Entity<OwnedGame>(entity =>
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
            builder.Entity<SessionDetails>(entity =>
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
            builder.Entity<Friendship>(entity =>
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
            builder.Entity<Achievement>(entity =>
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
            builder.Entity<UserAchievement>(entity =>
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
            builder.Entity<Collection>(entity =>
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
            builder.Entity<UserProfile>(entity =>
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
            builder.Entity<Wallet>(entity =>
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
            builder.Entity<User>(entity =>
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

            builder.Entity<ChatConversation>(entity =>
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

            builder.Entity<ChatMessage>(entity =>
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
            var usersSeed2 = new List<object>
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

            builder.Entity<User>().HasData(usersSeed2);

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

            builder.Entity<UserProfile>().HasData(userProfilesSeed);

            // ReviewsUsers seed data
            var reviewsUsersSeed = new List<object>
            {
                new { UserId = 2, Name = "Sam Carter", ProfilePicture = (byte[])null },
                new { UserId = 3, Name = "Taylor Kim", ProfilePicture = (byte[])null }
            };

            builder.Entity<ReviewsUser>().HasData(reviewsUsersSeed);

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

            builder.Entity<Feature>().HasData(featuresSeed);

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

            builder.Entity<Wallet>().HasData(walletsSeed);

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

            builder.Entity<Collection>().HasData(collectionsSeed);

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

            builder.Entity<OwnedGame>().HasData(ownedGamesSeed);

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

            builder.Entity<Achievement>().HasData(achievementsSeed);


        }
    }
}

using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.Common;
using SteamHub.ApiContract.Models.Game;
using CollectionGame = SteamHub.Api.Entities.CollectionGame;
using Game = SteamHub.Api.Entities.Game;
using OwnedGame = SteamHub.Api.Entities.OwnedGame;

namespace SteamHub.Api.Context
{
    using Game = Game;
    using OwnedGame = OwnedGame;
    using CollectionGame = CollectionGame;
    using Collection = Collection;
    using User = User;


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


        // Added From other team
        public DbSet<SessionDetails> UserSessions { get; set; }
        
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Achievement> Achievements { get; set; }
        public DbSet<ChatConversation> ChatConversations { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }

        public DbSet<UserAchievement> UserAchievements { get; set; }
        public DbSet<Collection> Collections { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<OwnedGame> OwnedGames { get; set; }
        public DbSet<Review> Reviews { get; set; }
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

            var passwords = new List<string>
            {
                "$2a$11$y9nrgXGsRSSLRuf1MYvXhOmd0lI9lc6y95ZSPlNJWAVVOBIQAUvka", // secret
                "$2a$11$L.BgAHQgfXZzzRf39MeLLeKDLkLCXbVHS/ij4uV5OoKm2OojiSDBG", // matto
                "$2a$11$PSbTI5wYN/bqNZT3TT/IzeSqNkaliV/ZeautgH07hT0JMjE5VyVYq", // cena22
                "$2a$11$m2QqrI0MQZcVa2Rs0e1Zdu/gXKwZBQ.LTGyQynQ33KbDPvRSWhYm6", // aliceJ
                "$2a$11$zsix20gCQb4OHlnY2pgKdOaZAEG4Cz9EwwtR7qoIcrSoceWEHOf3a", // secret
                "$2a$11$f6Fwypz3hHQzfxRvQKuHBO6/usICItpW2/enOPs2pEyRBU7Aakj/y", // secret
                "$2a$11$hfsZhti3nPkX8X7jhF8PR.ZuQzwF0W.L/8VqOcfzXic3PfFVbKrCu", // secret
                "$2a$11$vTuuHlSawwHhJPxOPAePquBqh.7BRqiLfsBbh4eC81dJNsz14HTWC"  // secret
            };

            var usersSeed = new List<User>
            {
                new User
                {
                    UserId = 1,
                    Email = "gabe.newell@valvestudio.com",
                    PointsBalance = 6000,
                    Username = "GabeN",
                    UserRole = UserRole.Developer,
                    WalletBalance = 500,
                    Password = passwords[0],
                    CreatedAt = new DateTime(2024, 1, 1),
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "https://i.imgur.com/vixhhkC.jpeg",
                    Bio = "Gaming enthusiast and software developer",
                    LastModified = new DateTime(2024, 1, 1)
                },
                new User
                {
                    UserId = 2,
                    Email = "mathias.new@cdprojektred.com",
                    PointsBalance = 5000,
                    Username = "MattN",
                    UserRole = UserRole.Developer,
                    WalletBalance = 420,
                    Password = passwords[1],
                    CreatedAt = new DateTime(2024, 1, 1),
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                    Bio = "Game developer and tech lover",
                    LastModified = new DateTime(2024, 1, 1)
                },
                new User
                {
                    UserId = 3,
                    Email = "john.chen@thatgamecompany.com",
                    PointsBalance = 5000,
                    Username = "JohnC",
                    UserRole = UserRole.Developer,
                    WalletBalance = 390,
                    Password = passwords[2],
                    CreatedAt = new DateTime(2024, 1, 1),
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                    Bio = "Casual gamer and streamer",
                    LastModified = new DateTime(2024, 1, 1)
                },
                new User
                {
                    UserId = 4,
                    Email = "alice.johnson@example.com",
                    PointsBalance = 6000,
                    Username = "AliceJ",
                    UserRole = UserRole.User,
                    WalletBalance = 780,
                    Password = passwords[3],
                    CreatedAt = new DateTime(2024, 1, 1),
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                    Bio = "Casual gamer and streamer",
                    LastModified = new DateTime(2024, 1, 1)
                },
                new User
                {
                    UserId = 5,
                    Email = "liam.garcia@example.com",
                    PointsBalance = 7000,
                    Username = "LiamG",
                    UserRole = UserRole.User,
                    WalletBalance = 5500,
                    Password = passwords[4],
                    CreatedAt = new DateTime(2024, 1, 1),
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                    Bio = "Casual gamer and streamer",
                    LastModified = new DateTime(2024, 1, 1)
                },
                new User
                {
                    UserId = 6,
                    Email = "sophie.williams@example.com",
                    PointsBalance = 6000,
                    Username = "SophieW",
                    UserRole = UserRole.User,
                    WalletBalance = 950,
                    Password = passwords[5],
                    CreatedAt = new DateTime(2024, 1, 1),
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                    Bio = "Casual gamer and streamer",
                    LastModified = new DateTime(2024, 1, 1)
                },
                new User
                {
                    UserId = 7,
                    Email = "noah.smith@example.com",
                    PointsBalance = 4000,
                    Username = "NoahS",
                    UserRole = UserRole.User,
                    WalletBalance = 3300,
                    Password = passwords[6],
                    CreatedAt = new DateTime(2024, 1, 1),
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                    Bio = "Casual gamer and streamer",
                    LastModified = new DateTime(2024, 1, 1)
                },
                new User
                {
                    UserId = 8,
                    Email = "emily.brown@example.com",
                    PointsBalance = 5000,
                    Username = "EmilyB",
                    UserRole = UserRole.User,
                    WalletBalance = 1100,
                    Password = passwords[7],
                    CreatedAt = new DateTime(2024, 1, 1),
                    LastLogin = new DateTime(2024, 1, 1),
                    ProfilePicture = "",
                    Bio = "Casual gamer and streamer",
                    LastModified = new DateTime(2024, 1, 1)
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
                    PurchaseDate = new DateTime(2024, 1, 1),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 4,
                    PointShopItemId = 2,
                    PurchaseDate = new DateTime(2024, 1, 1),
                    IsActive = true
                },
                new UserPointShopItemInventory
                {
                    UserId = 4,
                    PointShopItemId = 5,
                    PurchaseDate = new DateTime(2024, 1, 1),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 5,
                    PointShopItemId = 2,
                    PurchaseDate = new DateTime(2024, 1, 1),
                    IsActive = true
                },
                new UserPointShopItemInventory
                {
                    UserId = 5,
                    PointShopItemId = 6,
                    PurchaseDate = new DateTime(2024, 1, 1),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 6,
                    PointShopItemId = 3,
                    PurchaseDate = new DateTime(2024, 1, 1),
                    IsActive = false
                },
                new UserPointShopItemInventory
                {
                    UserId = 7,
                    PointShopItemId = 4,
                    PurchaseDate = new DateTime(2024, 1, 1),
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
                    Date = new DateTime(2024, 1, 1),
                    Amount = (float)14.99,
                    WithMoney = true
                },
                new StoreTransaction
                {
                    StoreTransactionId = 2,
                    UserId = 7,
                    GameId = 20,
                    Date = new DateTime(2024, 1, 1),
                    Amount = (float)34.99,
                    WithMoney = false
                },
                new StoreTransaction
                {
                    StoreTransactionId = 3,
                    UserId = 4,
                    GameId = 15,
                    Date = new DateTime(2024, 1, 1),
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
                    TradeDate = new DateTime(2024, 1, 1),
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
                    TradeDate = new DateTime(2024, 1, 1),
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
                    TradeDate = new DateTime(2024, 1, 1),
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
                    AcquiredDate = new DateTime(2024, 1, 1),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 4,
                    ItemId = 7,
                    GameId = 6,
                    AcquiredDate = new DateTime(2024, 1, 1),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 4,
                    ItemId = 9,
                    GameId = 15,
                    AcquiredDate = new DateTime(2024, 1, 1),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 4,
                    ItemId = 10,
                    GameId = 15,
                    AcquiredDate = new DateTime(2024, 1, 1),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 5,
                    ItemId = 6,
                    GameId = 5,
                    AcquiredDate = new DateTime(2024, 1, 1),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 5,
                    ItemId = 8,
                    GameId = 6,
                    AcquiredDate = new DateTime(2024, 1, 1),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 5,
                    ItemId = 12,
                    GameId = 19,
                    AcquiredDate = new DateTime(2024, 1, 1),
                    IsActive = false,
                },
                new UserInventory
                {
                    UserId = 6,
                    ItemId = 13,
                    GameId = 20,
                    AcquiredDate = new DateTime(2024, 1, 1),
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
                entity.HasKey(s => s.SessionId);
                entity.Property(s => s.SessionId);
                entity.Property(s => s.UserId)
                    .IsRequired();
                entity.Property(s => s.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
                entity.Property(s => s.ExpiresAt)
                    .IsRequired();

                entity.HasOne(s => s.User)
                    .WithMany()
                    .HasForeignKey(s => s.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure entities here

            // -- ReviewsUser mapping ---------------------------------------------------
            

            // -- SoldGame mapping --------------------------------------------------------
            builder.Entity<SoldGame>(entity =>
            {
                entity.HasKey(sg => sg.SoldGameId);
                entity.Property(sg => sg.SoldGameId)
                      .ValueGeneratedOnAdd();
                entity.Property(sg => sg.UserId)
                      .IsRequired();
                entity.Property(sg => sg.GameId);
                entity.Property(sg => sg.SoldDate);

                entity.HasOne(e => e.User)
                        .WithMany(u => u.SoldGames)
                        .HasForeignKey(e => e.UserId)
                        .OnDelete(DeleteBehavior.Cascade);
            });

            // -- CollectionGame mapping ------------------------------------------------
            builder.Entity<CollectionGame>(entity =>
            {
                entity.HasKey(cg => new { cg.CollectionId, cg.GameId });
                entity.Property(cg => cg.CollectionId);
                entity.Property(cg => cg.GameId);

                entity.HasOne(cg => cg.Collection)
                      .WithMany(c => c.CollectionGames)
                      .HasForeignKey(cg => cg.CollectionId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(cg => cg.OwnedGame)
                      .WithMany(og => og.CollectionGames)
                      .HasForeignKey(cg => cg.GameId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -- Feature mapping -------------------------------------------------------
            builder.Entity<Feature>(entity =>
            {
                entity.HasKey(f => f.FeatureId);
                entity.Property(f => f.FeatureId)
                      .ValueGeneratedOnAdd();
                entity.Property(f => f.Name)
                      .IsRequired();
                entity.Property(f => f.Value)
                      .IsRequired();
                entity.Property(f => f.Description);
                entity.Property(f => f.Type)
                      .IsRequired();
                entity.Property(f => f.Source);
                entity.Property(f => f.Equipped);

                entity.HasMany(f => f.Users)
                      .WithOne(fu => fu.Feature)
                      .HasForeignKey(fu => fu.FeatureId);
            });

            // -- FeatureUser mapping ---------------------------------------------------
            builder.Entity<FeatureUser>(entity =>
            {
                entity.HasKey(fu => new { fu.UserId, fu.FeatureId });
                entity.Property(fu => fu.UserId);
                entity.Property(fu => fu.FeatureId);
                entity.Property(fu => fu.Equipped)
                      .HasDefaultValue(false);

                entity.HasOne(fu => fu.Feature)
                    .WithMany(f => f.Users)
                    .HasForeignKey(fu => fu.FeatureId);

                entity.HasOne(fu => fu.User)
                    .WithMany()
                    .HasForeignKey(fu => fu.UserId);
            });

            // -- ForumPost mapping ----------------------------------------------------
            builder.Entity<ForumPost>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Title);
                entity.Property(e => e.Body);
                entity.Property(e => e.TimeStamp);
                entity.Property(e => e.AuthorId);
                entity.Property(e => e.Score);
                entity.Property(e => e.GameId);

                entity.HasOne(fp => fp.Author)
                      .WithMany()
                      .HasForeignKey(fp => fp.AuthorId);

                entity.HasOne(fp => fp.Game)
                      .WithMany()
                      .HasForeignKey(fp => fp.GameId);

                entity.HasMany(fp => fp.Comments)
                      .WithOne(fc => fc.Post)
                      .HasForeignKey(fc => fc.PostId);
            });

            // -- ForumComment mapping ---------------------------------------------------
            builder.Entity<ForumComment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Body);
                entity.Property(e => e.TimeStamp);
                entity.Property(e => e.AuthorId);
                entity.Property(e => e.Score);
                entity.Property(e => e.PostId);

                entity.HasOne(fc => fc.Author)
                      .WithMany(u => u.ForumComments)
                      .HasForeignKey(fc => fc.AuthorId)
                      .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(fc => fc.Post)
                      .WithMany(fp => fp.Comments)
                      .HasForeignKey(fc => fc.PostId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -- UserLikedPost mapping ------------------------------------------------------
            builder.Entity<UserLikedPost>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.PostId });
                entity.Property(e => e.UserId);
                entity.Property(e => e.PostId);

                entity.HasOne(ulp => ulp.User)
                      .WithMany()
                      .HasForeignKey(ulp => ulp.UserId);

                entity.HasOne(ulp => ulp.Post)
                      .WithMany()
                      .HasForeignKey(ulp => ulp.PostId);
            });

            // -- UserDislikedPost mapping ---------------------------------------------------
            builder.Entity<UserDislikedPost>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.PostId });
                entity.Property(e => e.UserId);
                entity.Property(e => e.PostId);

                entity.HasOne(udp => udp.User)
                      .WithMany()
                      .HasForeignKey(udp => udp.UserId);

                entity.HasOne(udp => udp.Post)
                      .WithMany()
                      .HasForeignKey(udp => udp.PostId);
            });

            // -- UserLikedComment mapping ----------------------------------------------------
            builder.Entity<UserLikedComment>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CommentId });
                entity.Property(e => e.UserId);
                entity.Property(e => e.CommentId);

                entity.HasOne(ulc => ulc.User)
                      .WithMany()
                      .HasForeignKey(ulc => ulc.UserId);

                entity.HasOne(ulc => ulc.Comment)
                      .WithMany()
                      .HasForeignKey(ulc => ulc.CommentId);
            });

            // -- UserDislikedComment mapping -----------------------------------------------------
            builder.Entity<UserDislikedComment>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.CommentId });
                entity.Property(e => e.UserId);
                entity.Property(e => e.CommentId);

                entity.HasOne(udc => udc.User)
                      .WithMany()
                      .HasForeignKey(udc => udc.UserId);

                entity.HasOne(udc => udc.Comment)
                      .WithMany()
                      .HasForeignKey(udc => udc.CommentId);
            });

            // -- FriendEntity mapping -----------------------------------------------------
            builder.Entity<FriendEntity>(entity =>
            {
                entity.HasKey(f => f.FriendshipId);
                entity.Property(f => f.User1Id)
                    .IsRequired();
                entity.Property(f => f.User2Id)
                    .IsRequired();
                entity.Property(f => f.CreatedDate)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(f => f.User1)
                      .WithMany()
                      .HasForeignKey(f => f.User1Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(f => f.User2)
                      .WithMany()
                      .HasForeignKey(f => f.User2Id)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // -- FriendRequest mapping ---------------------------------------------------
            builder.Entity<FriendRequest>(entity =>
            {
                entity.HasKey(fr => fr.RequestId);
                entity.Property(fr => fr.RequestId)
                      .ValueGeneratedOnAdd();

                entity.Property(fr => fr.SenderUserId)
                      .IsRequired();

                entity.Property(fr => fr.ReceiverUserId)
                      .IsRequired();

                entity.Property(fr => fr.RequestDate)
                      .HasDefaultValueSql("GETDATE()");

                entity.Property(fr => fr.Status)
                      .IsRequired();

                entity.HasOne(fr => fr.Sender)
                      .WithMany()
                      .HasForeignKey(fr => fr.SenderUserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(fr => fr.Receiver)
                      .WithMany()
                      .HasForeignKey(fr => fr.ReceiverUserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(fr => new { fr.SenderUserId, fr.ReceiverUserId })
                      .IsUnique();
            });

            // -- NewsPost mapping -------------------------------------------------------
            builder.Entity<Post>(entity =>
            {
                entity.HasKey(n => n.Id);
                entity.Property(n => n.Id).ValueGeneratedOnAdd();
                entity.Property(n => n.AuthorId);
                entity.Property(n => n.Content);
                entity.Property(n => n.UploadDate);
                entity.Property(n => n.NrLikes);
                entity.Property(n => n.NrDislikes);
                entity.Property(n => n.NrComments);

                entity.Ignore(n => n.ActiveUserRating);
            });

            // -- NewsComment mapping ----------------------------------------------------
            builder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.CommentId);
                entity.Property(c => c.CommentId).ValueGeneratedOnAdd();
                entity.Property(c => c.AuthorId);
                entity.Property(c => c.PostId);
                entity.Property(c => c.Content);
                entity.Property(c => c.CommentDate);

                entity.Ignore(c => c.NrLikes);
                entity.Ignore(c => c.NrDislikes);

				entity.HasOne(c => c.Post)
					.WithMany()
					.HasForeignKey(c => c.PostId)
					.OnDelete(DeleteBehavior.Cascade);
            });

            // -- NewsRating mapping -----------------------------------------------------
            builder.Entity<PostRatingType>(entity =>
            {
                entity.HasKey(r => new { r.PostId, r.AuthorId });
                entity.Property(r => r.PostId);
                entity.Property(r => r.AuthorId);
                entity.Property(r => r.RatingType);

                entity.HasOne(r => r.Post)
                    .WithMany()
                    .HasForeignKey(r => r.PostId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(r => r.Author)
                    .WithMany()
                    .HasForeignKey(r => r.AuthorId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // -- PasswordResetCode mapping -----------------------------------------------
            builder.Entity<PasswordResetCode>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.Id)
                      .ValueGeneratedOnAdd();
                entity.Property(p => p.UserId)
                      .IsRequired();
                entity.Property(p => p.ResetCode);
                entity.Property(p => p.ExpirationTime);
                entity.Property(p => p.Used);
                entity.Property(p => p.Email);
            });

            // -- Review mapping ------------------------------------------------------------
            builder.Entity<Review>(entity =>
            {
                entity.HasKey(r => r.ReviewIdentifier);
                entity.Property(r => r.ReviewIdentifier)
                    .ValueGeneratedOnAdd();
                entity.Property(r => r.ReviewTitleText)
                    .IsRequired();
                entity.Property(r => r.ReviewContentText)
                    .IsRequired();
                entity.Property(r => r.IsRecommended)
                    .HasColumnType("bit");
                entity.Property(r => r.NumericRatingGivenByUser)
                    .HasColumnType("decimal(3,1)");
                entity.Property(r => r.TotalHelpfulVotesReceived);
                entity.Property(r => r.TotalFunnyVotesReceived);
                entity.Property(r => r.TotalHoursPlayedByReviewer);
                entity.Property(r => r.DateAndTimeWhenReviewWasCreated);
                entity.Property(r => r.UserIdentifier)
                    .IsRequired();
                entity.Property(r => r.GameIdentifier)
                    .IsRequired();

                entity.HasOne(r => r.User)
                    .WithMany(u => u.Reviews)
                    .HasForeignKey(r => r.UserIdentifier)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(r => r.Game)
                    .WithMany()
                    .HasForeignKey(r => r.GameIdentifier)
                    .OnDelete(DeleteBehavior.NoAction);

            });

            // -- OwnedGame mapping ---------------------------------------------------------
            builder.Entity<OwnedGame>(entity =>
            {
                entity.HasKey(og => og.GameId);
                entity.Property(og => og.GameId)
                    .ValueGeneratedOnAdd();
                entity.Property(og => og.UserId)
                    .IsRequired();
                entity.HasIndex(og => og.UserId);
                entity.Property(og => og.GameTitle)
                    .IsRequired();
                entity.Property(og => og.Description);
                entity.Property(og => og.CoverPicture);

                entity.HasMany(og => og.CollectionGames)
                      .WithOne(cg => cg.OwnedGame)
                      .HasForeignKey(cg => cg.GameId);
            });

            // -- SessionDetails mapping -----------------------------------------------------
            builder.Entity<SessionDetails>(entity =>
            {
                entity.HasKey(s => s.SessionId);
                entity.Property(s => s.SessionId);
                entity.Property(s => s.UserId)
                    .IsRequired();
                entity.Property(s => s.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
                entity.Property(s => s.ExpiresAt)
                    .IsRequired();
            });

            // -- Friendship mapping --------------------------------------------------------
            builder.Entity<Friendship>(entity =>
            {
                entity.HasKey(f => f.FriendshipId);
                entity.Property(f => f.FriendshipId)
                    .ValueGeneratedOnAdd();
                entity.Property(f => f.UserId)
                    .IsRequired();
                entity.Property(f => f.FriendId)
                    .IsRequired();
                entity.HasIndex(f => f.UserId);
                entity.HasIndex(f => f.FriendId);
                entity.HasIndex(f => new { f.UserId, f.FriendId })
                    .IsUnique();

                entity.HasOne(f => f.User)
                    .WithMany(u => u.Friendships)
                    .HasForeignKey(f => f.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(f => f.Friend)
                    .WithMany(u => u.FriendOf)
                    .HasForeignKey(f => f.FriendId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.Ignore(f => f.FriendUsername);
                entity.Ignore(f => f.FriendProfilePicture);
            });

            // -- Achievement mapping --------------------------------------------------------
            builder.Entity<Achievement>(entity =>
            {
                entity.HasKey(a => a.AchievementId);
                entity.Property(a => a.AchievementId)
                    .ValueGeneratedOnAdd();
                entity.Property(a => a.AchievementName)
                    .IsRequired();
                entity.Property(a => a.Description);
                entity.Property(a => a.AchievementType)
                    .IsRequired();
                entity.Property(a => a.Points)
                    .IsRequired();
                entity.Property(a => a.Icon);
            });

            // -- UserAchievement mapping ----------------------------------------------------
            builder.Entity<UserAchievement>(entity =>
            {
                entity.HasKey(ua => new { ua.UserId, ua.AchievementId });
                entity.Property(ua => ua.UserId)
                      .IsRequired();
                entity.Property(ua => ua.AchievementId)
                      .IsRequired();
                entity.Property(ua => ua.UnlockedAt)
                      .HasDefaultValueSql("GETDATE()");

                entity.HasOne(ua => ua.User)
                      .WithMany(u => u.UserAchievements)
                      .HasForeignKey(ua => ua.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ua => ua.Achievement)
                      .WithMany(a => a.UserAchievements)
                      .HasForeignKey(ua => ua.AchievementId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // -- Collection mapping --------------------------------------------------------
            builder.Entity<Collection>(entity =>
            {
                entity.HasKey(c => c.CollectionId);
                entity.Property(c => c.CollectionId)
                    .ValueGeneratedOnAdd();
                entity.Property(c => c.UserId)
                    .IsRequired();
                entity.HasIndex(c => c.UserId);
                entity.Property(c => c.CollectionName)
                    .IsRequired();
                entity.Property(c => c.CoverPicture);
                entity.Property(c => c.IsPublic)
                    .HasDefaultValue(true);
                entity.Property(c => c.CreatedAt)
                    .HasColumnType("date")
                    .HasDefaultValueSql("CAST(GETDATE() AS DATE)");

                entity.HasMany(c => c.CollectionGames)
                      .WithOne(cg => cg.Collection)
                      .HasForeignKey(cg => cg.CollectionId);
            });

            // -- Wallet mapping --------------------------------------------------------
            builder.Entity<Wallet>(entity =>
            {
                entity.HasKey(w => w.WalletId);
                entity.Property(w => w.WalletId)
                    .ValueGeneratedOnAdd();
                entity.Property(w => w.UserId)
                    .IsRequired();
                entity.HasIndex(w => w.UserId)
                    .IsUnique();
                entity.Property(w => w.Points)
                    .HasDefaultValue(0);
                entity.Property(w => w.Balance)
                    .HasColumnType("decimal(10,2)")
                    .HasDefaultValue(0m);
            });

            // -- Users mapping --------------------------------------------------------------
            builder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserId)
                    .ValueGeneratedOnAdd();
                entity.Property(u => u.Username)
                    .IsRequired();
                entity.Property(u => u.Email)
                    .IsRequired();
                entity.Property(u => u.Password)
                    .IsRequired();
                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
                entity.Property(u => u.LastLogin);

                entity.HasMany(u => u.Reviews)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserIdentifier)
                    .OnDelete(DeleteBehavior.NoAction);

                // UserProfile
                entity.Property(up => up.Bio);
                entity.Property(up => up.LastModified)
                    .HasDefaultValueSql("GETDATE()");
                entity.Ignore(up => up.ProfilePicture);

                entity.HasMany(u => u.NewsPosts)
                    .WithOne(p => p.Author)
                    .HasForeignKey(p => p.AuthorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.NewsComments)
                    .WithOne(c => c.Author)
                    .HasForeignKey(c => c.AuthorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.PostRatings)
                    .WithOne(r => r.Author)
                    .HasForeignKey(r => r.AuthorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.SentFriendRequests)
                    .WithOne(fr => fr.Sender)
                    .HasForeignKey(fr => fr.SenderUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.ReceivedFriendRequests)
                    .WithOne(fr => fr.Receiver)
                    .HasForeignKey(fr => fr.ReceiverUserId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Chat-related relationships
                entity.HasMany(u => u.ConversationsAsUser1)
                    .WithOne(c => c.User1)
                    .HasForeignKey(c => c.User1Id)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.ConversationsAsUser2)
                    .WithOne(c => c.User2)
                    .HasForeignKey(c => c.User2Id)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.SentMessages)
                    .WithOne(m => m.Sender)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.NoAction);

                // Forum-related relationships
                entity.HasMany(u => u.ForumComments)
                    .WithOne(fc => fc.Author)
                    .HasForeignKey(fc => fc.AuthorId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.LikedComments)
                    .WithOne(lc => lc.User)
                    .HasForeignKey(lc => lc.UserId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasMany(u => u.DislikedComments)
                    .WithOne(dc => dc.User)
                    .HasForeignKey(dc => dc.UserId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            builder.Entity<ChatConversation>(entity =>
            {
                entity.HasKey(c => c.ConversationId);
                entity.Property(c => c.ConversationId)
                    .ValueGeneratedOnAdd();
                entity.Property(c => c.User1Id)
                    .IsRequired();
                entity.Property(c => c.User2Id)
                    .IsRequired();

                entity.HasOne(c => c.User1)
                      .WithMany()
                      .HasForeignKey(c => c.User1Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.User2)
                      .WithMany()
                      .HasForeignKey(c => c.User2Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(c => c.Messages)
                      .WithOne(m => m.Conversation)
                      .HasForeignKey(m => m.ConversationId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ChatMessage>(entity =>
            {
                entity.HasKey(m => m.MessageId);
                entity.Property(m => m.MessageId)
                    .ValueGeneratedOnAdd();
                entity.Property(m => m.ConversationId)
                    .IsRequired();
                entity.Property(m => m.SenderId)
                    .IsRequired();
                entity.Property(m => m.MessageContent)
                    .IsRequired();
                entity.Property(m => m.MessageFormat)
                    .IsRequired();
                entity.Property(m => m.Timestamp)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasOne(m => m.Conversation)
                      .WithMany(c => c.Messages)
                      .HasForeignKey(m => m.ConversationId);

                entity.HasOne(m => m.Sender)
                      .WithMany()
                      .HasForeignKey(m => m.SenderId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Wallets seed data
            var walletsSeed = new List<Wallet>
            {
                new Wallet { WalletId = 1, UserId = 1, Points = 10, Balance = 200m },
                new Wallet { WalletId = 2, UserId = 2, Points = 10, Balance = 200m },
                new Wallet { WalletId = 3, UserId = 3, Points = 10, Balance = 200m },
                new Wallet { WalletId = 4, UserId = 4, Points = 10, Balance = 200m },
                new Wallet { WalletId = 5, UserId = 5, Points = 10, Balance = 200m },
                new Wallet { WalletId = 6, UserId = 6, Points = 10, Balance = 200m },
                new Wallet { WalletId = 7, UserId = 7, Points = 10, Balance = 200m },
                new Wallet { WalletId = 8, UserId = 8, Points = 10, Balance = 200m }
            };

            builder.Entity<Wallet>().HasData(walletsSeed);

            // Features seed data
            var featuresSeed = new List<Feature>
            {
                new Feature { FeatureId = 1, Name = "Black Hat", Value = 2000, Description = "An elegant hat", Type = "hat", Source = "Assets/Features/Hats/black-hat.png", Equipped = false },
                new Feature { FeatureId = 2, Name = "Pufu", Value = 10, Description = "Cute doggo", Type = "pet", Source = "Assets/Features/Pets/dog.png", Equipped = false },
                new Feature { FeatureId = 3, Name = "Kitty", Value = 8, Description = "Cute cat", Type = "pet", Source = "Assets/Features/Pets/cat.png", Equipped = false },
                new Feature { FeatureId = 4, Name = "Frame", Value = 5, Description = "Violet frame", Type = "frame", Source = "Assets/Features/Frames/frame1.png", Equipped = false },
                new Feature { FeatureId = 5, Name = "Love Emoji", Value = 7, Description = "lalal", Type = "emoji", Source = "Assets/Features/Emojis/love.png", Equipped = false },
                new Feature { FeatureId = 6, Name = "Violet Background", Value = 7, Description = "Violet Background", Type = "background", Source = "Assets/Features/Backgrounds/violet.jpg", Equipped = false }
            };

            builder.Entity<Feature>().HasData(featuresSeed);

            // Collections seed data
            var collectionsSeed = new List<Collection>
            {
                new Collection { CollectionId = 1, UserId = 1, CollectionName = "All Owned Games", CoverPicture = "/Assets/Collections/allgames.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2022-02-21") },
                new Collection { CollectionId = 2, UserId = 1, CollectionName = "Sports", CoverPicture = "/Assets/Collections/sports.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2023-03-21") },
                new Collection { CollectionId = 3, UserId = 1, CollectionName = "Chill Games", CoverPicture = "/Assets/Collections/chill.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2024-03-21") },
                new Collection { CollectionId = 4, UserId = 1, CollectionName = "X-Mas", CoverPicture = "/Assets/Collections/xmas.jpg", IsPublic = false, CreatedAt = DateOnly.Parse("2025-02-21") },
                new Collection { CollectionId = 5, UserId = 2, CollectionName = "Shooters", CoverPicture = "/Assets/Collections/shooters.jpg", IsPublic = true, CreatedAt = DateOnly.Parse("2025-03-21") },
                new Collection { CollectionId = 6, UserId = 2, CollectionName = "Pets", CoverPicture = "/Assets/Collections/pets.jpg", IsPublic = false, CreatedAt = DateOnly.Parse("2025-01-21") }
            };

            builder.Entity<Collection>().HasData(collectionsSeed);

            // OwnedGames seed data
            var ownedGamesSeed = new List<OwnedGame>
            {
                new OwnedGame { GameId = 1, UserId = 1, GameTitle = "Call of Duty: MWIII", Description = "First-person military shooter", CoverPicture = "/Assets/Games/codmw3.png" },
                new OwnedGame { GameId = 2, UserId = 1, GameTitle = "Overwatch2", Description = "Team-based hero shooter", CoverPicture = "/Assets/Games/overwatch2.png" },
                new OwnedGame { GameId = 3, UserId = 1, GameTitle = "Counter-Strike2", Description = "Tactical shooter", CoverPicture = "/Assets/Games/cs2.png" },
                new OwnedGame { GameId = 4, UserId = 2, GameTitle = "FIFA25", Description = "Football simulation", CoverPicture = "/Assets/Games/fifa25.png" },
                new OwnedGame { GameId = 5, UserId = 2, GameTitle = "NBA2K25", Description = "Basketball simulation", CoverPicture = "/Assets/Games/nba2k25.png" },
                new OwnedGame { GameId = 6, UserId = 2, GameTitle = "Tony Hawk Pro Skater", Description = "Skateboarding sports game", CoverPicture = "/Assets/Games/thps.png" }
            };

            builder.Entity<OwnedGame>().HasData(ownedGamesSeed);

            // Achievements seed data
            var achievementsSeed = new List<Achievement>
            {
                new Achievement { AchievementId = 1, AchievementName = "FRIENDSHIP1", Description = "You made a friend, you get a point", AchievementType = "Friendships", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementId = 2, AchievementName = "FRIENDSHIP2", Description = "You made 5 friends, you get 3 points", AchievementType = "Friendships", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementId = 3, AchievementName = "FRIENDSHIP3", Description = "You made 10 friends, you get 5 points", AchievementType = "Friendships", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementId = 4, AchievementName = "FRIENDSHIP4", Description = "You made 50 friends, you get 10 points", AchievementType = "Friendships", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementId = 5, AchievementName = "FRIENDSHIP5", Description = "You made 100 friends, you get 15 points", AchievementType = "Friendships", Points = 15, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" }
            };

            builder.Entity<Achievement>().HasData(achievementsSeed);
        }
    }
}

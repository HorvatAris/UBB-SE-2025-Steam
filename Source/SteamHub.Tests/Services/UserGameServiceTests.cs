namespace SteamHub.Tests.Services
{
    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.UsersGames;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services;
    using Xunit;
    using SteamHub.ApiContract.Constants;
    using SteamHub.ApiContract.Models.Tag;
    using Windows.Security.Authentication.OnlineId;
    using SteamHub.ApiContract.Models.Common;

    public class UserGameServiceTests
    {
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IUsersGamesRepository> userGameRepositoryMock;
        private readonly Mock<IGameRepository> gameRepositoryMock;
        private readonly Mock<ITagRepository> tagRepositoryMock;
        private readonly Mock<IUserDetails> userDetailsMock;

        private readonly UserGameService userGameService;

        public UserGameServiceTests()
        {
            this.userRepositoryMock = new Mock<IUserRepository>();
            this.userGameRepositoryMock = new Mock<IUsersGamesRepository>();
            this.gameRepositoryMock = new Mock<IGameRepository>();
            this.tagRepositoryMock = new Mock<ITagRepository>();
            this.userDetailsMock = new Mock<IUserDetails>();

            this.userDetailsMock.SetupGet(u => u.UserId).Returns(1);

            this.userGameService = new UserGameService(
                this.userRepositoryMock.Object,
                this.userGameRepositoryMock.Object,
                this.gameRepositoryMock.Object,
                this.tagRepositoryMock.Object);
        }

        [Fact]
        public async Task RemoveGameFromWishlistAsync_ValidGameId_CallsRepositoryCorrectly()
        {
            var request = new UserGameRequest
            {
                UserId = 1,
                GameId = 1
            };

            await this.userGameService.RemoveGameFromWishlistAsync(request);

            this.userGameRepositoryMock.Verify(repo => repo.RemoveFromWishlistAsync(
                It.Is<UserGameRequest>(req =>
                    req.GameId == request.GameId &&
                    req.UserId == request.UserId)), Times.Once);
        }


        [Fact]
        public async Task AddGameToWishlistAsync_ValidId_CallsRepository()
        {
            var game = new Game { GameId = 2, GameTitle = "Not Owned Game" };
            var request = new UserGameRequest
            {
                UserId = 1,
                GameId = 2
            };

            this.userGameRepositoryMock
                .Setup(repo => repo.GetUserPurchasedGamesAsync(userDetailsMock.Object.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            gameRepositoryMock
    .Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
    .ReturnsAsync(new GameDetailedResponse { Identifier = 2, Name = "Test Game" });


            await this.userGameService.AddGameToWishlistAsync(request);

            this.userGameRepositoryMock.Verify(repo => repo.AddToWishlistAsync(
                It.Is<UserGameRequest>(request =>
                    request.GameId == game.GameId && request.UserId == userDetailsMock.Object.UserId)), Times.Once);
        }


        [Fact]
        public async Task GetAllGamesAsync_WhenGamesExist_ReturnsMappedGames()
        {
            int userId = 1;
            var userGamesResponse = new GetUserGamesResponse
            {
                UserGames = new List<UserGamesResponse>
        {
            new UserGamesResponse { GameId = 1 },
            new UserGamesResponse { GameId = 2 }
        }
            };

            this.userGameRepositoryMock.Setup(repo =>
                repo.GetUserGamesAsync(1)).ReturnsAsync(userGamesResponse);

            this.gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(1)).ReturnsAsync(
                new GameDetailedResponse { Identifier = 1, Name = "Game1", Price = 59.99m });

            this.gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(2)).ReturnsAsync(
                new GameDetailedResponse { Identifier = 2, Name = "Game2", Price = 39.99m });

            var result = await this.userGameService.GetAllGamesAsync(userId);

            Assert.Equal(2, result.Count);
            Assert.Contains(result, game => game.GameId == 1 && game.GameTitle == "Game1");
            Assert.Contains(result, game => game.GameId == 2 && game.GameTitle == "Game2");
        }


        [Fact]
        public async Task PurchaseGamesAsync_ValidGames_UpdatesLastEarnedPoints()
        {
            int userId = 1;
            var games = new List<Game>
    {
        new Game { GameId = 1, Price = 10.0m },
        new Game { GameId = 2, Price = 20.0m }
    };

            var purchaseRequest = new PurchaseGamesRequest
            {
                UserId = userId,
                Games = games,
                IsWalletPayment = true
            };

            var user = new UserResponse
            {
                UserId = userId,
                WalletBalance = 100f,
                PointsBalance = 50f,
                UserName = "testuser",
                Email = "test@example.com"
            };

            userRepositoryMock
                .Setup(repo => repo.GetUserByIdAsync(userId))
                .ReturnsAsync(user);

            userGameRepositoryMock
                .Setup(repo => repo.PurchaseGameAsync(It.IsAny<UserGameRequest>()))
                .Returns(Task.CompletedTask);

            userRepositoryMock
                .Setup(repo => repo.UpdateUserAsync(userId, It.IsAny<UpdateUserRequest>()))
                .Returns(Task.CompletedTask);

            var pointsAwarded = await userGameService.PurchaseGamesAsync(purchaseRequest);

            decimal totalSpent = games.Sum(g => g.Price); 
            int expectedPoints = (int)(totalSpent * 121); 

            userGameRepositoryMock.Verify(
                repo => repo.PurchaseGameAsync(It.IsAny<UserGameRequest>()),
                Times.Exactly(games.Count));

            userRepositoryMock.Verify(
                repo => repo.UpdateUserAsync(userId, It.IsAny<UpdateUserRequest>()),
                Times.Once);

            Assert.Equal(expectedPoints, pointsAwarded);
            Assert.Equal(expectedPoints, userGameService.LastEarnedPoints);
        }

        [Fact]
        public void ComputeTrendingScores_WhenMultipleGames_ShouldCalculateCorrectScores()
        {
            var games = new Collection<Game>
    {
        new Game { NumberOfRecentPurchases = 100 },
        new Game { NumberOfRecentPurchases = 50 },
        new Game { NumberOfRecentPurchases = 0 }
    };

            userGameService.ComputeTrendingScores(games);

            Assert.Equal(1.0m, games[0].TrendingScore);
            Assert.Equal(0.5m, games[1].TrendingScore);
            Assert.Equal(0.0m, games[2].TrendingScore);
        }

        [Fact]
        public async Task ComputeTagScoreForGamesAsync_WhenCalled_ShouldSetNonNegativeTagScores()
        {
            int userId = 1;
            var games = new Collection<Game>
    {
        new Game { GameTitle = "Game1", Tags = new[] { "Action", "RPG" } },
        new Game { GameTitle = "Game2", Tags = new[] { "Puzzle" } }
    };

            var allTags = new List<TagSummaryResponse>
    {
        new TagSummaryResponse { TagId = 1, TagName = "Action" },
        new TagSummaryResponse { TagId = 2, TagName = "RPG" }
    };

            tagRepositoryMock.Setup(repo => repo.GetAllTagsAsync())
                .ReturnsAsync(new GetTagsResponse { Tags = allTags });

            userGameRepositoryMock.Setup(repo => repo.GetUserGamesAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse> { new UserGamesResponse { GameId = 1 } }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Tags = new List<TagDetailedResponse>
                    {
                new TagDetailedResponse { TagName = "Action" },
                new TagDetailedResponse { TagName = "Action" },
                new TagDetailedResponse { TagName = "Puzzle" }
                    }
                });

            await userGameService.ComputeTagScoreForGamesAsync(games, userId);

            Assert.True(games.All(game => game.TagScore >= 0));
        }

        [Fact]
        public async Task GetFavoriteUserTagsAsync_WhenCalled_ShouldReturnThreeTags()
        {
            int userId = 1;
            var allTags = new List<TagSummaryResponse>
    {
        new TagSummaryResponse { TagId = 1, TagName = "Action" },
        new TagSummaryResponse { TagId = 2, TagName = "Puzzle" },
        new TagSummaryResponse { TagId = 3, TagName = "Adventure" },
        new TagSummaryResponse { TagId = 4, TagName = "RPG" }
    };

            tagRepositoryMock.Setup(repo => repo.GetAllTagsAsync())
                .ReturnsAsync(new GetTagsResponse { Tags = allTags });

            userGameRepositoryMock.Setup(repo => repo.GetUserGamesAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>
                    {
                new UserGamesResponse { GameId = 1 },
                new UserGamesResponse { GameId = 2 },
                new UserGamesResponse { GameId = 3 }
                    }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(1))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Tags = new List<TagDetailedResponse>
                    {
                new TagDetailedResponse { TagName = "Action" },
                new TagDetailedResponse { TagName = "Puzzle" }
                    }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(2))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Tags = new List<TagDetailedResponse>
                    {
                new TagDetailedResponse { TagName = "Action" },
                new TagDetailedResponse { TagName = "RPG" }
                    }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(3))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Tags = new List<TagDetailedResponse>
                    {
                new TagDetailedResponse { TagName = "Puzzle" },
                new TagDetailedResponse { TagName = "Adventure" }
                    }
                });

            var result = await userGameService.GetFavoriteUserTagsAsync(userId);

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetFavoriteUserTagsAsync_WhenCalled_ShouldIncludeMostCommonTag()
        {
            int userId = 1;
            var allTags = new List<TagSummaryResponse>
    {
        new TagSummaryResponse { TagId = 1, TagName = "Action" },
        new TagSummaryResponse { TagId = 2, TagName = "Puzzle" }
    };

            tagRepositoryMock.Setup(repo => repo.GetAllTagsAsync())
                .ReturnsAsync(new GetTagsResponse { Tags = allTags });

            userGameRepositoryMock.Setup(repo => repo.GetUserGamesAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>
                    {
                new UserGamesResponse { GameId = 1 },
                new UserGamesResponse { GameId = 2 }
                    }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(new GameDetailedResponse
                {
                    Tags = new List<TagDetailedResponse>
                    {
                new TagDetailedResponse { TagName = "Action" },
                new TagDetailedResponse { TagName = "Action" }
                    }
                });

            var result = await userGameService.GetFavoriteUserTagsAsync(userId);

            Assert.Contains(result, tag => tag.Tag_name == "Action");
        }

        [Fact]
        public async Task AddGameToWishlistAsync_WhenGameNotPurchased_ShouldCallAddToWishlist()
        {
            // Arrange
            var request = new UserGameRequest
            {
                UserId = 1,
                GameId = 7
            };

            var gameResponse = new GameDetailedResponse
            {
                Identifier = 7,
                Name = "NewGame",
                Price = 49.99m
            };

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(request.GameId))
                .ReturnsAsync(gameResponse);

            userGameRepositoryMock.Setup(repo => repo.GetUserPurchasedGamesAsync(request.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            userGameRepositoryMock.Setup(repo => repo.GetUserWishlistAsync(request.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            await userGameService.AddGameToWishlistAsync(request);

            userGameRepositoryMock.Verify(repo => repo.AddToWishlistAsync(
                It.Is<UserGameRequest>(r =>
                    r.UserId == request.UserId &&
                    r.GameId == request.GameId)),
                Times.Once);
        }


        [Fact]
        public async Task RemoveGameFromWishlistAsync_WhenCalled_ShouldCallRemoveFromWishlist()
        {
            var request = new UserGameRequest
            {
                UserId = 1,
                GameId = 5
            };

            await userGameService.RemoveGameFromWishlistAsync(request);

            userGameRepositoryMock.Verify(repo => repo.RemoveFromWishlistAsync(
                It.Is<UserGameRequest>(r =>
                    r.UserId == request.UserId &&
                    r.GameId == request.GameId)),
                Times.Once);
        }


        [Fact]
        public async Task PurchaseGamesAsync_WhenCalledWithMultipleGames_ShouldCallPurchaseForEachGame()
        {
            var gamesToBuy = new List<Game>
    {
        new Game { GameId = 10, Price = 15.0m },
        new Game { GameId = 11, Price = 25.0m }
    };

            var purchaseRequest = new PurchaseGamesRequest
            {
                UserId = 1,
                Games = gamesToBuy,
                IsWalletPayment = true
            };

            var user = new User
            {
                UserId = 1,
                Username = "TestUser",
                Email = "test@example.com",
                WalletBalance = 100.0f,
                PointsBalance = 0
            };

            userGameRepositoryMock.Setup(repo => repo.PurchaseGameAsync(It.IsAny<UserGameRequest>()))
                .Returns(Task.CompletedTask);

            userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(purchaseRequest.UserId))
    .ReturnsAsync(new UserResponse
    {
        UserId = 1,
        UserName = "TestUser",
        Email = "test@example.com",
        WalletBalance = 100f,
        PointsBalance = 0,
        UserRole = UserRole.User,
        CreatedAt = DateTime.UtcNow,
        ProfilePicture = ""
    });


            userRepositoryMock.Setup(repo => repo.UpdateUserAsync(user.UserId, It.IsAny<UpdateUserRequest>()))
                .Returns(Task.CompletedTask);


            var pointsAwarded = await userGameService.PurchaseGamesAsync(purchaseRequest);

            // Assert
            userGameRepositoryMock.Verify(repo => repo.PurchaseGameAsync(
                It.Is<UserGameRequest>(r => r.UserId == 1)),
                Times.Exactly(gamesToBuy.Count));

            Assert.Equal((int)((15.0m + 25.0m) * 121), pointsAwarded);
        }


        [Fact]
        public async Task PurchaseGamesAsync_WhenCalled_ShouldUpdateUserRepository()
        {
            var gamesToBuy = new List<Game> { new Game { GameId = 10, Price = 20m } };
            var purchaseRequest = new PurchaseGamesRequest
            {
                UserId = 1,
                Games = gamesToBuy,
                IsWalletPayment = true
            };

            var user = new User
            {
                UserId = 1,
                Username = "TestUser",
                Email = "test@example.com",
                WalletBalance = 50f,
                PointsBalance = 0
            };

            userGameRepositoryMock.Setup(repo => repo.PurchaseGameAsync(It.IsAny<UserGameRequest>()))
                .Returns(Task.CompletedTask);

            userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(purchaseRequest.UserId))
    .ReturnsAsync(new UserResponse
    {
        UserId = 1,
        UserName = "TestUser",
        Email = "test@example.com",
        WalletBalance = 50f,
        PointsBalance = 0,
        UserRole = UserRole.User,
        CreatedAt = DateTime.UtcNow,
        ProfilePicture = ""
    });


            userRepositoryMock.Setup(repo => repo.UpdateUserAsync(user.UserId, It.IsAny<UpdateUserRequest>()))
                .Returns(Task.CompletedTask);

            await userGameService.PurchaseGamesAsync(purchaseRequest);

            userRepositoryMock.Verify(repo => repo.UpdateUserAsync(
                user.UserId,
                It.IsAny<UpdateUserRequest>()),
                Times.Once);
        }

        [Fact]
        public async Task IsGamePurchasedAsync_WhenGameNotPurchased_ShouldReturnFalse()
        {
            int userId = 1;
            var game = new Game { GameId = 2 };

            userGameRepositoryMock.Setup(repo => repo.GetUserPurchasedGamesAsync(It.IsAny<int>()))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>()
                });

            var result = await userGameService.IsGamePurchasedAsync(game, userId);

            Assert.False(result);
        }

        private void SetupUserWishlistWithGames(IEnumerable<Game> games)
        {
            this.userGameRepositoryMock.Setup(repo =>
                repo.GetUserWishlistAsync(1)).ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = games.Select(g => new UserGamesResponse { GameId = g.GameId }).ToList()
                });
        }

        private void SetupGameServiceMock(IEnumerable<Game> games)
        {
            this.gameRepositoryMock.Setup(repo =>
                repo.GetGameByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((int id) =>
                {
                    var match = games.First(g => g.GameId == id);
                    return new GameDetailedResponse
                    {
                        Identifier = id,
                        Name = match.GameTitle,
                        Rating = match.Rating 
                    };
                });
        }

        }
}

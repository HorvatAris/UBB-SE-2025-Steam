namespace SteamHub.Tests.Services
{
    using System.Collections.ObjectModel;
    using System.Net;
    using System.Threading.Tasks;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UsersGames;
    using Xunit;
    using SteamHub.ApiContract.Repositories;

    public class DeveloperServiceTests
    {
        private const int TestGameId = 1;
        private const string TestGameIdText = "1";

        private const int TestGamePrice = 10;
        private const string TestGamePriceText = "10";

        private const string TestGameNameText = "Test";
        private const string TestGameDescriptionText = "Desc";
        private const string TestGameImageInfoText = "img.png";
        private const string TestGameTrailerInfoText = "trailer";
        private const string TestGameGameplayInfoText = "gameplay";
        private const string TestGameMinimumRequirementText = "min";
        private const string TestGameRecommendedRequirementText = "rec";
        private const decimal TestGameDiscount = 0;
        private const string TestGameDiscountText = "0";

        private const int TestRating = 0;
        private const int TestPublisherIdentifier = 1;

        private const string TestPendingGameStatusText = "Pending";

        private const int TestTrendingScore = 0;
        private const int TestTagScore = 0;
        private const int TestNumberOfRecentPurchases = 0;

        private const int TestTagId = 1;
        private const int TestSecondTagId = 2;

        private const int TestUserId = 1;

        private readonly DeveloperService developerService;
        private readonly Mock<IGameRepository> gameRepositoryMock;
        private readonly Mock<ITagRepository> tagRepositoryMock;
        private readonly Mock<IUsersGamesRepository> userGameRepositoryMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
        private readonly Mock<IItemRepository> itemRepositoryMock;
        private readonly Mock<IItemTradeDetailRepository> itemTradeDetailRepositoryMock;

        private readonly User testUser;

        public DeveloperServiceTests()
        {
            gameRepositoryMock = new Mock<IGameRepository>();
            tagRepositoryMock = new Mock<ITagRepository>();
            userGameRepositoryMock = new Mock<IUsersGamesRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            itemRepositoryMock = new Mock<IItemRepository>();
            itemTradeDetailRepositoryMock = new Mock<IItemTradeDetailRepository>();
            testUser = new User { UserId = 1, WalletBalance = 50f };

            developerService = new DeveloperService(gameRepositoryMock.Object, tagRepositoryMock.Object, userGameRepositoryMock.Object,
                userRepositoryMock.Object, itemRepositoryMock.Object, itemTradeDetailRepositoryMock.Object);
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenGameIdentifierEmpty_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame(string.Empty, "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5", new List<Tag> { new Tag() }));
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenGameNameEmpty_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame("1", string.Empty, "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5", new List<Tag> { new Tag() }));
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenGameIdentifierNonNumeric_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame("abc", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "5", new List<Tag> { new Tag() }));
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenPriceNegative_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame("1", "name", "-10", "desc", "img", "trailer", "gameplay", "min", "rec", "5", new List<Tag> { new Tag() }));
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenDiscountNotNumeric_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame("1", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "abc", new List<Tag> { new Tag() }));
        }

        [Fact]
        public void ValidateInputForAddingAGame_WhenDiscountTooHigh_ShouldThrow()
        {
            Assert.Throws<Exception>(() =>
                developerService.ValidateInputForAddingAGame("1", "name", "10", "desc", "img", "trailer", "gameplay", "min", "rec", "150", new List<Tag> { new Tag() }));
        }

        [Fact]
        public async Task IsGameIdInUse_WhenIdInUse_ShouldReturnTrue()
        {
            var expectedGameIdentifier = TestGameId;

            gameRepositoryMock.Setup(proxy => proxy.GetGameByIdAsync(expectedGameIdentifier)).ReturnsAsync(new GameDetailedResponse());

            var result = await developerService.IsGameIdInUseAsync(expectedGameIdentifier);

            Assert.True(result);
        }

        [Fact]
        public async Task IsGameIdInUse_WhenIdNotInUse_ShouldReturnFalse()
        {
            var expectedGameIdentifier = TestGameId;

            var exception = new HttpRequestException("Not Found", null, HttpStatusCode.NotFound);

            gameRepositoryMock.Setup(proxy => proxy.GetGameByIdAsync(expectedGameIdentifier)).ThrowsAsync(exception);

            var result = await developerService.IsGameIdInUseAsync(expectedGameIdentifier);

            Assert.False(result);
        }

        [Fact]
        public async Task GetGameOwnerCount_WhenCalledForAGameWithNonZeroOwnerCount_ShouldReturnProperCount()
        {
            var expectedGameIdentifier = TestGameId;
            var expectedGameOwnerCount = 2;
            var testUserId1 = 1;
            var testUserId2 = 2;

            userRepositoryMock.Setup(proxy => proxy.GetUsersAsync()).ReturnsAsync(new GetUsersResponse
            {
                Users = new List<UserResponse>
                {
                    new UserResponse { UserId = 1, WalletBalance = 50f },
                    new UserResponse { UserId = 2, WalletBalance = 60f }
                }
            });

            userGameRepositoryMock.Setup(proxy => proxy.GetUserGamesAsync(testUserId1))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>
                    {
                        new UserGamesResponse { GameId = expectedGameIdentifier, IsPurchased = true }
                    }
                });

            userGameRepositoryMock.Setup(proxy => proxy.GetUserGamesAsync(testUserId2))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>
                    {
                        new UserGamesResponse { GameId = expectedGameIdentifier, IsPurchased = true }
                    }
                });

            var result = await developerService.GetGameOwnerCountAsync(expectedGameIdentifier);

            Assert.Equal(expectedGameOwnerCount, result);
        }

        [Fact]
        public async Task CreateValidatedGame_WhenIdInUse_ShouldThrow()
        {
            var gameIdText = TestGameIdText;
            var name = TestGameNameText;
            var priceText = TestGamePriceText;
            var description = TestGameDescriptionText;
            var imageUrl = TestGameImageInfoText;
            var tralerUrl = TestGameTrailerInfoText;
            var gameplayUrl = TestGameGameplayInfoText;
            var minimumRequirement = TestGameMinimumRequirementText;
            var recommendedRequirement = TestGameRecommendedRequirementText;
            var dicountText = TestGameDiscountText;
            var tags = new List<Tag> { new Tag { TagId = TestTagId } };

            var expectedGameIdentifier = TestGameId;
            var userId = TestUserId;

            gameRepositoryMock.Setup(proxy => proxy.GetGameByIdAsync(expectedGameIdentifier)).ReturnsAsync(new GameDetailedResponse());

            await Assert.ThrowsAsync<NotImplementedException>(() =>
                developerService.CreateValidatedGameAsync(gameIdText, name, priceText, description, imageUrl, tralerUrl, gameplayUrl, minimumRequirement, recommendedRequirement, dicountText, tags, userId));
        }

        [Fact]
        public async Task DeleteGame_WhenDeletingValidGame_ShouldRemoveFromCollection()
        {
            var gameList = new ObservableCollection<Game> { new Game() { GameId = TestGameId } };
            var expectedIdentifier = TestGameId;

            await developerService.DeleteGameAsync(expectedIdentifier, gameList);

            Assert.Empty(gameList);
        }

        [Fact]
        public async Task UpdateGameAndRefreshList_WhenUpdatingValidGame_ShouldUpdateCorrectly()
        {
            var existing = new Game { GameId = TestGameId };
            var updated = new Game { GameId = TestGameId, GameTitle = "Updated" };
            var games = new ObservableCollection<Game> { existing };
            var userId = TestUserId;

            await developerService.UpdateGameAndRefreshListAsync(updated, games, userId);

            Assert.Contains(updated, games);
        }

        [Fact]
        public async Task RejectGameAndRemoveFromUnvalidated_WhenRemovingGameFromList_ShouldHaveListEmpty()
        {
            var games = new ObservableCollection<Game> { new Game { GameId = TestGameId } };
            var expectedIdentifier = TestGameId;

            await developerService.RejectGameAndRemoveFromUnvalidatedAsync(expectedIdentifier, games);

            Assert.Empty(games);
        }

        [Fact]
        public async Task GetMatchingTagsForGame_WhenHavingTags_ShouldReturnAsManyMatchingTags()
        {
            var allTags = new List<Tag> { new Tag() { TagId = TestTagId }, new Tag() { TagId = TestSecondTagId } };
            var expectedIdentifier = TestGameId;
            var expectedMatchingTagsCount = 1;

            gameRepositoryMock.Setup(proxy => proxy.GetGameByIdAsync(expectedIdentifier)).ReturnsAsync(new GameDetailedResponse
            {
                Identifier = expectedIdentifier,
                Tags = new List<TagDetailedResponse>
                {
                    new TagDetailedResponse
                {
                    TagId = TestTagId
                }
                }
            });
            var actualMatchingTags = await developerService.GetMatchingTagsForGameAsync(expectedIdentifier, allTags);

            Assert.Equal(expectedMatchingTagsCount, actualMatchingTags.Count);
        }
    }
}
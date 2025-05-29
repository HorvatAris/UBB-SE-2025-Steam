namespace SteamHub.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.UsersGames;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services.Interfaces;
    using Xunit;

    public class CartServiceTests
    {
        private const int TestGameIdentifier = 1;
        private const int TestSecondGameIdentifier = 2;
        private const int TestGamePrice = 10;
        private const int TestSecondGamePrice = 20;

        private readonly CartService cartService;
        private readonly Mock<IUsersGamesRepository> userGamesRepositoryMock;
        private readonly Mock<IGameRepository> gameRepositoryMock;
        private readonly Mock<IUserDetails> userDetailsMock;
        private readonly User testUser;

        public CartServiceTests()
        {
            userGamesRepositoryMock = new Mock<IUsersGamesRepository>();
            gameRepositoryMock = new Mock<IGameRepository>();
            userDetailsMock = new Mock<IUserDetails>();

            testUser = new User { UserId = 1, WalletBalance = 50f };
            userDetailsMock.Setup(u => u.UserId).Returns(testUser.UserId);
            userDetailsMock.Setup(u => u.WalletBalance).Returns(testUser.WalletBalance);

            cartService = new CartService(
                userGamesRepositoryMock.Object,
                gameRepositoryMock.Object);
        }

        [Fact]
        public async Task GetCartGamesAsync_WhenServiceThrowsException_ShouldReturnEmptyList()
        {
            int userId = 1;
            userGamesRepositoryMock.Setup(repo => repo.GetUserCartAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            var foundGames = await cartService.GetCartGamesAsync(userId);
            Assert.Empty(foundGames);
        }

        [Fact]
        public async Task GetCartGamesAsync_WhenNoGamesInCart_ShouldReturnEmptyList()
        {
            var userId = 1;
            userGamesRepositoryMock.Setup(repo => repo.GetUserCartAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse { UserGames = new List<UserGamesResponse>() });

            var foundGames = await cartService.GetCartGamesAsync(userId);
            Assert.Empty(foundGames);
        }

        [Fact]
        public async Task GetCartGamesAsync_WhenGamesExistInCart_ShouldReturnCorrectNumberOfGames()
        {
            var userId = 1;
            var testGames = new List<UserGamesResponse>
            {
                new UserGamesResponse { GameId = TestGameIdentifier },
                new UserGamesResponse { GameId = TestSecondGameIdentifier }
            };

            userGamesRepositoryMock.Setup(repo => repo.GetUserCartAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse { UserGames = testGames });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(TestGameIdentifier))
                .ReturnsAsync(new GameDetailedResponse { Identifier = TestGameIdentifier });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(TestSecondGameIdentifier))
                .ReturnsAsync(new GameDetailedResponse { Identifier = TestSecondGameIdentifier });

            var foundGames = await cartService.GetCartGamesAsync(userId);
            Assert.Equal(2, foundGames.Count);
        }

        [Fact]
        public void GetTheTotalSumOfItemsInCart_WhenMultipleGamesProvided_ShouldReturnCorrectSum()
        {
            var cartGames = new List<Game>
            {
                new Game { Price = TestGamePrice },
                new Game { Price = TestGamePrice },
                new Game { Price = TestGamePrice }
            };

            var foundTotalSum = cartService.GetTheTotalSumOfItemsInCart(cartGames);
            Assert.Equal(30f, foundTotalSum);
        }

        [Fact]
        public async Task AddGameToCartAsync_WhenGameAlreadyPurchased_ShouldThrowExceptionWithCorrectMessage()
        {
            userGamesRepositoryMock.Setup(repo => repo.GetUserPurchasedGamesAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse
                {
                    UserGames = new List<UserGamesResponse>
                    {
                new UserGamesResponse { GameId = TestGameIdentifier, IsPurchased = true }
                    }
                });

            gameRepositoryMock.Setup(repo => repo.GetGameByIdAsync(TestGameIdentifier))
                .ReturnsAsync(new GameDetailedResponse { Identifier = TestGameIdentifier });

            userGamesRepositoryMock.Setup(repo => repo.GetUserCartAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse { UserGames = new List<UserGamesResponse>() });

            var userGameRequest = new UserGameRequest
            {
                UserId = testUser.UserId,
                GameId = TestGameIdentifier
            };

            var exception = await Assert.ThrowsAsync<Exception>(() => cartService.AddGameToCartAsync(userGameRequest));
            Assert.Equal("The game is already purchased.", exception.Message);
        }


        [Fact]
        public async Task AddGameToCartAsync_WhenValidGame_ShouldCallAddToCartAsyncOnce()
        {
            userGamesRepositoryMock.Setup(repo => repo.GetUserPurchasedGamesAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse { UserGames = new List<UserGamesResponse>() });

            userGamesRepositoryMock.Setup(repo => repo.GetUserCartAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse { UserGames = new List<UserGamesResponse>() });

            var userGameRequest = new UserGameRequest
            {
                UserId = testUser.UserId,
                GameId = TestGameIdentifier
            };

            await cartService.AddGameToCartAsync(userGameRequest);

            userGamesRepositoryMock.Verify(repo => repo.AddToCartAsync(
                It.Is<UserGameRequest>(r => r.UserId == testUser.UserId && r.GameId == TestGameIdentifier)), Times.Once);
        }


        [Fact]
        public async Task GetAllPurchasedGamesAsync_WhenNoPurchasedGames_ShouldReturnEmptyList()
        {
            var userId = 1;
            userGamesRepositoryMock.Setup(repo => repo.GetUserPurchasedGamesAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse { UserGames = new List<UserGamesResponse>() });

            var result = await cartService.GetAllPurchasedGamesAsync(userId);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllCartGamesIdsAsync_WhenNoGamesInCart_ShouldReturnEmptyList()
        {
            var userId = 1;
            userGamesRepositoryMock.Setup(repo => repo.GetUserCartAsync(testUser.UserId))
                .ReturnsAsync(new GetUserGamesResponse { UserGames = new List<UserGamesResponse>() });

            var result = await cartService.GetAllCartGamesIdsAsync(userId);
            Assert.Empty(result);
        }
    }
}
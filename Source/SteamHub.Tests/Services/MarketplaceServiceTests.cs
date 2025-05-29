namespace SteamHub.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Services;
    using Moq;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.UserInventory;
    using SteamHub.ApiContract.Models.User;
    using Xunit;
    using SteamHub.ApiContract.Repositories;

    public class MarketplaceServiceTests
    {
        private readonly int testItemId = 1;
        private readonly string testItemName = "Normal Banner";
        private readonly string testItemDescription = "A Normal banner";
        private readonly float testItemPrice = 34;
        private readonly string testItemImagePath = "img";

        private readonly bool testItemListed = true;
        private readonly bool testItemNotListed = false;

        private readonly int testItemId2 = 2;
        private readonly string testItemName2 = "Cool Banner";
        private readonly string testItemDescription2 = "A Cool banner";
        private readonly float testItemPrice2 = 54;
        private readonly string testItemImagePath2 = "img2";

        private readonly int testItemId3 = 3;
        private readonly string testItemName3 = "Cold Banner";
        private readonly string testItemDescription3 = "Another cool banner, but cooler";
        private readonly float testItemPrice3 = 77;
        private readonly string testItemImagePath3 = "img3";

        private readonly MarketplaceService marketplaceService;
        private readonly Mock<IGameRepository> gameRepositoryMock;
        private readonly Mock<IUserInventoryRepository> userInventoryRepositoryMock;
        private readonly Mock<IUserRepository> userRepositoryMock;
		private readonly Mock<IItemRepository> itemRepositoryMock;
        private readonly Mock<IUsersGamesRepository> userGameRepositoryMock;

		private readonly User testUser;

        public MarketplaceServiceTests()
        {
            gameRepositoryMock = new Mock<IGameRepository>();
            userInventoryRepositoryMock = new Mock<IUserInventoryRepository>();
            userRepositoryMock = new Mock<IUserRepository>();
            itemRepositoryMock = new Mock<IItemRepository>();
            userGameRepositoryMock = new Mock<IUsersGamesRepository>();
            testUser = new User { UserId = 1, WalletBalance = 50f };
            marketplaceService = new MarketplaceService(userRepositoryMock.Object, gameRepositoryMock.Object,
                itemRepositoryMock.Object, userInventoryRepositoryMock.Object);
        }

        [Fact]
        public async Task GetListingsByGameAsync_ValidGame_ReturnsOnlyListedItemsForGame()
        {
            var game1 = new Game { GameId = 1, GameTitle = "Halo" };
            var game2 = new Game { GameId = 2, GameTitle = "Zelda" };

            var userInventory = new List<InventoryItemResponse>
            {
                new InventoryItemResponse
                {
                    ItemId = testItemId,
                    ItemName = testItemName,
                    Description = testItemDescription,
                    Price = testItemPrice,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath,
                    GameId = game1.GameId,
                    GameName = game1.GameTitle
                },
                new InventoryItemResponse
                {
                    ItemId = testItemId2,
                    ItemName = testItemName2,
                    Description = testItemDescription2,
                    Price = testItemPrice2,
                    IsListed = testItemNotListed,
                    ImagePath = testItemImagePath2,
                    GameId = game1.GameId,
                    GameName = game1.GameTitle
                },
                new InventoryItemResponse
                {
                    ItemId = testItemId3,
                    ItemName = testItemName3,
                    Description = testItemDescription3,
                    Price = testItemPrice3,
                    IsListed = testItemListed,
                    ImagePath = testItemImagePath3,
                    GameId = game2.GameId,
                    GameName = game2.GameTitle
                },
            };

            var item1 = new ItemDetailedResponse
            {
                ItemId = testItemId,
                ItemName = testItemName,
                Description = testItemDescription,
                Price = testItemPrice,
                IsListed = testItemListed,
                ImagePath = testItemImagePath,
                GameId = game1.GameId
            };

            var item2 = new ItemDetailedResponse
            {
                ItemId = testItemId2,
                ItemName = testItemName2,
                Description = testItemDescription2,
                Price = testItemPrice2,
                IsListed = testItemNotListed,
                ImagePath = testItemImagePath2,
                GameId = game1.GameId
            };

            var item3 = new ItemDetailedResponse
            {
                ItemId = testItemId3,
                ItemName = testItemName3,
                Description = testItemDescription3,
                Price = testItemPrice3,
                IsListed = testItemListed,
                ImagePath = testItemImagePath3,
                GameId = game2.GameId
            };

            var gameResponse = new GameDetailedResponse
            {
                Name = "Halo",
                Description = "Sci-fi FPS",
                Price = 60
            };

            userInventoryRepositoryMock
                .Setup(proxy => proxy.GetUserInventoryAsync(testUser.UserId))
                .ReturnsAsync(new UserInventoryResponse { Items = userInventory });

            itemRepositoryMock
                .Setup(proxy => proxy.GetItemByIdAsync(testItemId)).ReturnsAsync(item1);
            itemRepositoryMock
                .Setup(proxy => proxy.GetItemByIdAsync(testItemId2)).ReturnsAsync(item2);
            itemRepositoryMock
                .Setup(proxy => proxy.GetItemByIdAsync(testItemId3)).ReturnsAsync(item3);
            gameRepositoryMock
    .Setup(proxy => proxy.GetGameByIdAsync(game1.GameId)).ReturnsAsync(gameResponse);


            var result = await marketplaceService.GetListingsByGameAsync(game1.GameId, testUser.UserId);

            Assert.Empty(result);
            var returnedItem = result.First();
            Assert.Equal(1, returnedItem.ItemId);
            Assert.Equal("Normal Banner", returnedItem.ItemName);
            Assert.Equal("Halo", returnedItem.Game.GameTitle);
        }

        [Fact]
        public async Task BuyItemAsync_ItemNotListed_ThrowsInvalidOperationException()
        {
            var item = new Item
            {
                ItemId = testItemId,
                ItemName = testItemName,
                Description = testItemDescription,
                Price = testItemPrice,
                IsListed = testItemNotListed,
                ImagePath = testItemImagePath
            };

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                marketplaceService.BuyItemAsync(item, testUser.UserId));
        }

        [Fact]
        public async Task BuyItemAsync_ValidItem_ExecutesExpectedOperations()
        {
            var game1 = new Game { GameId = 1, GameTitle = "Halo" };

            var itemToBuy = new Item
            {
                ItemId = testItemId,
                ItemName = testItemName,
                Description = testItemDescription,
                Price = testItemPrice,
                IsListed = testItemListed,
                ImagePath = testItemImagePath,
                Game = game1
            };

            var userResponse = new UserResponse
            {
                UserId = testUser.UserId,
                Email = testUser.Email,
                UserName = testUser.Username,
                WalletBalance = testUser.WalletBalance,
                PointsBalance = testUser.PointsBalance,
                UserRole = testUser.UserRole
            };

            userRepositoryMock
                .Setup(repo => repo.GetUserByIdAsync(testUser.UserId))
                .ReturnsAsync(userResponse);

            userRepositoryMock
                .Setup(repo => repo.UpdateUserAsync(testUser.UserId, It.IsAny<UpdateUserRequest>()))
                .Returns(Task.CompletedTask);

            userInventoryRepositoryMock
                .Setup(items => items.AddItemToUserInventoryAsync(It.IsAny<ItemFromInventoryRequest>()))
                .Returns(Task.CompletedTask);

            itemRepositoryMock
                .Setup(item => item.UpdateItemAsync(testItemId, It.IsAny<UpdateItemRequest>()))
                .Returns(Task.CompletedTask);

            var result = await marketplaceService.BuyItemAsync(itemToBuy, testUser.UserId);

            Assert.True(result);

            userInventoryRepositoryMock.Verify(item =>
                item.AddItemToUserInventoryAsync(It.IsAny<ItemFromInventoryRequest>()), Times.Once);

            itemRepositoryMock.Verify(item =>
                item.UpdateItemAsync(testItemId, It.IsAny<UpdateItemRequest>()), Times.Once);

            userRepositoryMock.Verify(user =>
                user.UpdateUserAsync(testUser.UserId, It.IsAny<UpdateUserRequest>()), Times.Once);
        }


        [Fact]
        public async Task BuyItemAsync_NullItem_ThrowsArgumentNullException()
        {
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                marketplaceService.BuyItemAsync(null, testUser.UserId));

            Assert.Equal("item", exception.ParamName);
        }
    }
}

﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SteamHub.Api.Context;
using SteamHub.Api.Context.Repositories;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.UserPointShopItemInventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

using SteamHub.ApiContract.Models.Common;

namespace SteamHub.Tests.RepositoriesTests
{
    public class UserPointShopItemInventoryRepositoryTests
    {
        private readonly DataContext _context;
        private readonly UserPointShopItemInventoryRepository _repository;

        public UserPointShopItemInventoryRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string> { { "SomeSetting", "SomeValue" } })
                .Build();

            _context = new DataContext(options, configuration);
            _repository = new UserPointShopItemInventoryRepository(_context);

            SeedData();
        }

        private void SeedData()
        {
            var user = new User
            {
                UserId = 1,
                Username = "test_user",
                Email = "user@example.com",
                Password = "$2a$11$y9nrgXGsRSSLRuf1MYvXhOmd0lI9lc6y95ZSPlNJWAVVOBIQAUvka",
                ProfilePicture = "",
                WalletBalance = 100.0f,
                PointsBalance = 500.0f,
                UserRole = UserRole.User,
                UserPointShopItemsInventory = new List<UserPointShopItemInventory>(),
                StoreTransactions = new List<StoreTransaction>()
            };

            var item = new PointShopItem
            {
                PointShopItemId = 1,
                Name = "Mock Point Item",
                Description = "A mock point shop item",
                ImagePath = "/images/pointitem.jpg",
                PointPrice = 250.0,
                ItemType = "Skin",
                UserPointShopItemsInventory = new List<UserPointShopItemInventory>()
            };

            var inventory = new UserPointShopItemInventory
            {
                UserId = 1,
                PointShopItemId = 1,
                PurchaseDate = DateTime.Now,
                IsActive = true,
                User = user,
                PointShopItem = item
            };

            _context.Users.Add(user);
            _context.PointShopItems.Add(item);
            _context.UserPointShopInventories.Add(inventory);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetUserInventoryAsync_WithValidUserId_ReturnsSingleInventoryItem()
        {
            var response = await _repository.GetUserInventoryAsync(1);

            Assert.Single(response.UserPointShopItemsInventory);
        }


        [Fact]
        public async Task GetUserInventoryAsync_WithValidUserId_ReturnsActiveItem()
        {
            var response = await _repository.GetUserInventoryAsync(1);
            var item = response.UserPointShopItemsInventory.First();

            Assert.True(item.IsActive);
        }

        [Fact]
        public async Task PurchaseItemAsync_WhenCalledWithValidUserAndItem_AddsInventoryItem()
        {
            var newItem = new PointShopItem
            {
                PointShopItemId = 2,
                Name = "Mario",
                Description = "The OG game",
                ImagePath = "/images/mario.png",
                PointPrice = 100,
                ItemType = "fun",
                UserPointShopItemsInventory = new List<UserPointShopItemInventory>()
            };
            _context.PointShopItems.Add(newItem);
            _context.SaveChanges();

            var request = new PurchasePointShopItemRequest
            {
                UserId = 1,
                PointShopItemId = 2
            };

            await _repository.PurchaseItemAsync(request);

            var inventoryItem = await _context.UserPointShopInventories
                .FirstOrDefaultAsync(currentInventoryItem => currentInventoryItem.UserId == 1 && currentInventoryItem.PointShopItemId == 2);

            Assert.NotNull(inventoryItem);
            Assert.False(inventoryItem.IsActive);
        }

        [Fact]
        public async Task PurchaseItemAsync_WhenCalledWithInvalidUser_ThrowsException()
        {
            var request = new PurchasePointShopItemRequest
            {
                UserId = 999,
                PointShopItemId = 1
            };

            await Assert.ThrowsAsync<Exception>(() => _repository.PurchaseItemAsync(request));
        }

        [Fact]
        public async Task PurchaseItemAsync_WhenCalledWithInvalidItem_ThrowsException()
        {
            var request = new PurchasePointShopItemRequest
            {
                UserId = 1,
                PointShopItemId = 999
            };

            await Assert.ThrowsAsync<Exception>(() => _repository.PurchaseItemAsync(request));
        }

        [Fact]
        public async Task UpdateItemStatusAsync_WhenCalledWithValidRequest_UpdatesStatus()
        {
            var request = new UpdateUserPointShopItemInventoryRequest
            {
                UserId = 1,
                PointShopItemId = 1,
                IsActive = false
            };

            await _repository.UpdateItemStatusAsync(request);

            var item = await _context.UserPointShopInventories
                .FirstOrDefaultAsync(currentItem => currentItem.UserId == 1 && currentItem.PointShopItemId == 1);

            Assert.False(item.IsActive);
        }

        [Fact]
        public async Task UpdateItemStatusAsync_WhenCalledWithInvalidRequest_ThrowsException()
        {
            var request = new UpdateUserPointShopItemInventoryRequest
            {
                UserId = 1,
                PointShopItemId = 999,
                IsActive = true
            };

            await Assert.ThrowsAsync<Exception>(() => _repository.UpdateItemStatusAsync(request));
        }

        [Fact]
        public async Task ResetUserInventoryAsync_WhenCalled_RemovesAllItems()
        {
            await _repository.ResetUserInventoryAsync(1);

            var items = await _context.UserPointShopInventories
                .Where(currentItem => currentItem.UserId == 1)
                .ToListAsync();

            Assert.Empty(items);
        }
    }
}
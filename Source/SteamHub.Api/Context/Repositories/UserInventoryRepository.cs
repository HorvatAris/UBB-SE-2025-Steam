using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Entities;
using SteamHub.ApiContract.Models.UserInventory;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Context.Repositories
{
    public class UserInventoryRepository : IUserInventoryRepository
    {
        private readonly DataContext context;

        public UserInventoryRepository(DataContext context)
        {
            this.context = context;
        }

        public async Task AddItemToUserInventoryAsync(ItemFromInventoryRequest request)
        {
            var userId = await context.Users.FindAsync(request.UserId);
            if (userId == null)
            {
                throw new ArgumentException("User not found");
            }

            var itemId =await  context.Items.FindAsync(request.ItemId);
            if (itemId == null)
            {
                throw new ArgumentException("Item not found");
            }

            var gameId = await context.Games.FindAsync(request.GameId);
            if (gameId == null)
            {
                throw new ArgumentException("Game not found");
            }

            var userInventory = new UserInventory
            {
                UserId = request.UserId,
                ItemId = request.ItemId,
                GameId = request.GameId,
                AcquiredDate = DateTime.Now,
                IsActive = true
            };

            await context.UserInventories.AddAsync(userInventory);
            await context.SaveChangesAsync();
        }

        public async Task<InventoryItemResponse?> GetItemFromUserInventoryAsync(int userId, int itemId)
        {
            var currentUserInventory = await context.UserInventories
                .Include(userInventory => userInventory.Item)
                .Include(userInventory => userInventory.Game)
                .FirstOrDefaultAsync(userInventory => userInventory.UserId == userId && userInventory.ItemId == itemId);

            if (currentUserInventory == null) return null;

            return new InventoryItemResponse
            {
                ItemId = currentUserInventory.ItemId,
                ItemName = currentUserInventory.Item.ItemName,
                Price = currentUserInventory.Item.Price,
                Description = currentUserInventory.Item.Description,
                IsListed = currentUserInventory.Item.IsListed,
                GameName = currentUserInventory.Game.Name,
                GameId = currentUserInventory.Game.GameId,
                ImagePath = currentUserInventory.Item.ImagePath
            };
        }

        public async Task<UserInventoryResponse> GetUserInventoryAsync(int userId)
        {
            var userInventories = await context.UserInventories
                .Where(userInventory => userInventory.UserId == userId)
                .Include(userInventory => userInventory.Item)
                .Include(userInventory => userInventory.Game)
                .ToListAsync();

            var items = userInventories.Select(userInventory => new InventoryItemResponse
            {
                ItemId = userInventory.ItemId,
                ItemName = userInventory.Item.ItemName,
                Price = userInventory.Item.Price,
                Description = userInventory.Item.Description,
                IsListed = userInventory.Item.IsListed,
                GameName = userInventory.Game.Name,
                GameId = userInventory.Game.GameId,
                ImagePath = userInventory.Item.ImagePath
            }).ToList();

            return new UserInventoryResponse
            {
                UserId = userId,
                Items = items
            };
        }

        public async Task RemoveItemFromUserInventoryAsync(ItemFromInventoryRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var currentUserInventory = await context.UserInventories
                .FirstOrDefaultAsync(userInventory => userInventory.UserId == request.UserId && userInventory.ItemId == request.ItemId && userInventory.GameId == request.GameId);

            if (currentUserInventory == null) throw new ArgumentException("Item not found in user's inventory");

            context.UserInventories.Remove(currentUserInventory);
            await context.SaveChangesAsync();
        }
    }
}
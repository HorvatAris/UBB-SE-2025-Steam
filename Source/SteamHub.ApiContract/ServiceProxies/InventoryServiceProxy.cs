using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Utils;
using SteamHub.ApiContract.Models.ItemTrade;
using System.Net.Http.Json;
using SteamHub.ApiContract.Models.UserInventory;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class InventoryServiceProxy : ServiceProxy, IInventoryService
    {
        private readonly InventoryValidator inventoryValidator;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        public IUserDetails User { get; set; }
        public InventoryServiceProxy(IUserDetails user, string baseUrl = "http://172.30.245.56:8000") : base(baseUrl)
        {
            this.User = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        public async Task AddItemToInventoryAsync(Game game, Item item, int userId)
        {
            if (game == null) throw new ArgumentNullException(nameof(game));
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (userId <= 0) throw new ArgumentException("UserId must be positive.", nameof(userId));

            var request = new AddItemToInventoryRequest { Game = game, Item = item };
            await PostAsync($"/api/Inventory/AddItem/{userId}", request);
        }


        public List<Item> FilterInventoryItems(List<Item> items, Game selectedGame, string searchText)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));
            IEnumerable<Item> filtered = items.Where(item => !item.IsListed);

            bool hasGameFilter = selectedGame != null &&
                                 !string.IsNullOrWhiteSpace(selectedGame.GameTitle) &&
                                 !string.Equals(selectedGame.GameTitle, "All Games", StringComparison.OrdinalIgnoreCase);

            if (hasGameFilter)
            {
                filtered = filtered.Where(item =>
                    !string.IsNullOrWhiteSpace(item.GameName) &&
                    string.Equals(item.GameName, selectedGame.GameTitle, StringComparison.OrdinalIgnoreCase));
            }

            bool hasTextFilter = !string.IsNullOrWhiteSpace(searchText);
            if (hasTextFilter)
            {
                string trimmed = searchText.Trim();
                filtered = filtered.Where(item =>
                    (!string.IsNullOrEmpty(item.ItemName) && item.ItemName.Contains(trimmed, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(item.Description) && item.Description.Contains(trimmed, StringComparison.OrdinalIgnoreCase)));
            }

            return filtered.ToList();
        }

        public async Task<List<Item>> GetAllItemsFromInventoryAsync(int userId)
        {
            if (userId <= 0) throw new ArgumentException("UserId must be positive.", nameof(userId));
            return await GetAsync<List<Item>>($"/api/Inventory/All/{userId}");
        }

        public async Task<List<Game>> GetAvailableGamesAsync(List<Item> items, int userId)
        {
            if (userId <= 0) throw new ArgumentException("UserId must be positive.", nameof(userId));
            if (items == null) throw new ArgumentNullException(nameof(items));
            return await PostAsync<List<Game>>($"/api/Inventory/AvailableGames/{userId}", items);
        }


        public async Task<List<Item>> GetItemsFromInventoryAsync(Game game, int userId)
        {
            if (userId <= 0) throw new ArgumentException("UserId must be positive.", nameof(userId));
            if (game == null) throw new ArgumentNullException(nameof(game));
            return await GetAsync<List<Item>>($"/api/Inventory/ItemsFromGame/{userId}");
        }

        public async Task<List<Item>> GetUserFilteredInventoryAsync(int userId, Game selectedGame, string searchText)
        {
            var allItems = await this.GetUserInventoryAsync(userId);
            return this.FilterInventoryItems(allItems, selectedGame, searchText);
        }

        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            if (userId <= 0) throw new ArgumentException("UserId must be positive.", nameof(userId));
            var items = await GetAsync<List<Item>>($"/api/Inventory/UserInventory/{userId}") ?? new List<Item>();
            foreach (var item in items)
            {
                item.GameName = item.Game?.GameTitle ?? "Unknown Game";
            }
            return items;
        }


        public async Task<bool> SellItemAsync(Item item, int userId)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            if (userId <= 0) throw new ArgumentException("UserId must be positive.", nameof(userId));
            await PatchAsync<object>($"/api/Inventory/SellItem/{userId}", item);
            return true;
        }

    }
}

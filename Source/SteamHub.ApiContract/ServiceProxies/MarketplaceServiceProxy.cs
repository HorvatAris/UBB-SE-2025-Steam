using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class MarketplaceServiceProxy : ServiceProxy, IMarketplaceService
    {
        public IUserDetails User { get; set; }
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public MarketplaceServiceProxy(IUserDetails user, string baseUrl = "https://localhost:7241/") : base(baseUrl)
        {
            this.User = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        public async Task AddListingAsync(Game game, Item item)
        {
            await this.SwitchListingStatusAsync(game.GameId, item.ItemId);
        }

        public async Task<bool> BuyItemAsync(Item item, int userId)
        {
            try
            {
                await PostAsync($"api/Marketplace/BuyItem/{userId}", item);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while buying the item: {ex.Message}", ex);
            }
        }

        public async Task<List<Item>> GetAllListingsAsync()
        {
            try 
            {
                return await GetAsync<List<Item>>("api/Marketplace/Listings");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching listings: {ex.Message}", ex);
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            try
            {
                return await GetAsync<List<User>>("api/Marketplace/Users");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching users: {ex.Message}", ex);
            }
        }

        public async Task<List<Item>> GetListingsByGameAsync(int gameId, int userId)
        {
            try
            {
               return await GetAsync<List<Item>>($"api/Marketplace/Listings/{userId}/Game/{gameId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while fetching listings for game ID {gameId}: {ex.Message}", ex);
            }
        }

        public async Task RemoveListingAsync(Game game, Item item)
        {
            await this.SwitchListingStatusAsync(game.GameId, item.ItemId);
        }

        public async Task UpdateListingAsync(int gameId, int itemId)
        {
            try
            {
                await PutAsync<object>($"api/Marketplace/UpdateListing/{gameId}/{itemId}", null);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while updating the listing for game ID {gameId} and item ID {itemId}: {ex.Message}", ex);
            }
        }
        public async Task SwitchListingStatusAsync(int gameId, int itemId)
        {
            try
            {
                await PutAsync<object>($"api/Marketplace/SwitchListingStatus/{gameId}/{itemId}", null);
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while switching the listing status for game ID {gameId} and item ID {itemId}: {ex.Message}", ex);
            }
        }
    }
}

using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Models.UsersGames;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class CartServiceProxy : ServiceProxy, ICartService
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public CartServiceProxy(IUserDetails user, string baseUrl = "https://localhost:7241") : base(baseUrl)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        private const int InitialZeroSum = 0;
        private IUserDetails user;

        public async Task AddGameToCartAsync(UserGameRequest gameRequest)
        {
            try
            {
                var purchasedGames = await GetAllPurchasedGamesAsync(this.user.UserId);
                if (purchasedGames.Any(game => game.GameId == gameRequest.GameId))
                {
                    throw new InvalidOperationException("Game is already purchased and cannot be added to the cart again.");
                }

                await PostAsync("/api/Cart/AddToCart", gameRequest);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding game from cart: {exception.Message}");
                throw;
            }
        }

        public async Task<List<int>> GetAllCartGamesIdsAsync(int userId)
        {
            try
            {
                var response = await GetAsync<List<Game>>($"/api/Cart/{userId}");
                var gameIds = response
                    .Select(currentGame => currentGame.GameId)
                    .ToList();
                return gameIds ?? new List<int>();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
                return new List<int>();
            }
        }

        public async Task<List<Game>> GetAllPurchasedGamesAsync(int userId)
        {
            try
            {
                return await GetAsync<List<Game>>($"/api/Cart/Purchased/{userId}");
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching purchased games: {exception.Message}");
                return new List<Game>();
            }
        }

        public async Task<List<Game>> GetCartGamesAsync(int userId)
        {
            try
            {
                return await GetAsync<List<Game>>($"/api/Cart/{userId}");
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching user games: {exception.Message}");
                return new List<Game>();
            }
        }

        public float GetTheTotalSumOfItemsInCart(List<Game> cartGames)
        {
            float totalSum = InitialZeroSum;
            foreach (var game in cartGames)
            {
                totalSum += (float)game.Price;
            }

            return totalSum;
        }

        public async Task<decimal> GetTotalSumToBePaidAsync()
        {
            decimal totalSumToBePaid = InitialZeroSum;
            var cartGames = await this.GetCartGamesAsync(this.user.UserId);

            foreach (var game in cartGames)
            {
                totalSumToBePaid += (decimal)game.Price;
            }

            return totalSumToBePaid;
        }

        public User GetUser()
        {
            return new User(this.user);
        }

        public float GetUserFunds()
        {
            return (float)this.user.WalletBalance;
        }

        public async Task RemoveGameFromCartAsync(UserGameRequest gameRequest)
        {
            try
            {
                await PatchAsyncWithoutResponse("/api/Cart/RemoveFromCart", gameRequest);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error removing game from cart: {exception.Message}");
            }
        }

        public async Task RemoveGamesFromCartAsync(List<Game> games)
        {
            var gameRequest = new UserGameRequest
            {
                UserId = this.user.UserId
            };

            foreach (var game in games)
            {
                gameRequest.GameId = game.GameId;
                await this.RemoveGameFromCartAsync(gameRequest);
            }
        }
    }
}

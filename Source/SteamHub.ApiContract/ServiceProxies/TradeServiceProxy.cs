using SteamHub.ApiContract.Models.Item;
using SteamHub.ApiContract.Models.ItemTrade;
using SteamHub.ApiContract.Models.ItemTradeDetails;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models.Game;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class TradeServiceProxy : ServiceProxy, ITradeService
    {
        private readonly IUserDetails user;

        public TradeServiceProxy(IUserDetails user, string baseUrl = "https://localhost:7241/api/")
            : base(baseUrl)
        {
            this.user = user ?? throw new ArgumentNullException(nameof(user), "User cannot be null");
        }

        public async Task AcceptTradeAsync(ItemTrade trade, bool isSourceUser)
        {
            try
            {
                if (isSourceUser)
                {
                    trade.AcceptBySourceUser();
                }
                else
                {
                    trade.AcceptByDestinationUser();
                }

                await this.UpdateItemTradeAsync(trade);

                if (trade.AcceptedByDestinationUser)
                {
                    await this.CompleteTradeAsync(trade);
                }
            }
            catch (Exception tradeAcceptionException)
            {
                System.Diagnostics.Debug.WriteLine($"Error accepting trade: {tradeAcceptionException.Message}");
                throw;
            }
        }

        public async Task AddItemTradeAsync(ItemTrade trade)
        {
            if (trade == null)
                throw new ArgumentNullException(nameof(trade));

            if (trade.SourceUser == null || trade.DestinationUser == null)
                throw new ArgumentException("Source and destination users must be specified");
            
            if (trade.GameOfTrade == null)
                throw new ArgumentException("Game must be specified");

            if (string.IsNullOrEmpty(trade.TradeStatus))
                trade.TradeStatus = "Pending";

            await PostAsync("Trade", trade);
        }

        public async Task CompleteTradeAsync(ItemTrade trade)
        {
            try
            {
                var tradeRequest = new TransferItemTradeRequest
                {
                    SourceUserId = trade.SourceUser.UserId,
                    DestinationUserId = trade.DestinationUser.UserId,
                    GameId = trade.GameOfTrade.GameId
                };

                foreach (var item in trade.SourceUserItems)
                {
                    tradeRequest.ItemId = item.ItemId;
                    await this.TransferItemAsync(tradeRequest);
                }

                tradeRequest.SourceUserId = trade.DestinationUser.UserId;
                tradeRequest.DestinationUserId = trade.SourceUser.UserId;
                foreach (var item in trade.DestinationUserItems)
                {
                    tradeRequest.ItemId = item.ItemId;
                    await this.TransferItemAsync(tradeRequest);
                }

                trade.MarkTradeAsCompleted();
                await this.UpdateItemTradeAsync(trade);
            }
            catch (Exception tradeCompletingException)
            {
                System.Diagnostics.Debug.WriteLine($"Error completing trade: {tradeCompletingException.Message}");
                throw;
            }
        }

        public async Task CreateTradeAsync(ItemTrade trade)
        {
            try
            {
                await this.AddItemTradeAsync(trade);
            }
            catch (Exception tradeCreationException)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating trade: {tradeCreationException.Message}");
                throw;
            }
        }

        public async Task DeclineTradeRequest(ItemTrade trade)
        {
            try
            {
                await PostAsync("Trade/Decline", trade);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error declining trade: {ex.Message}", ex);
            }
        }

        public async Task<List<ItemTrade>> GetActiveTradesAsync(int userId)
        {
            try
            {
                var result = await GetAsync<List<ItemTrade>>($"Trade/Active/{userId}");
                return result ?? new List<ItemTrade>();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching active trades: {exception.Message}");
                return new List<ItemTrade>();
            }
        }

        public IUserDetails GetCurrentUser()
        {
            return this.user;
        }

        public async Task<List<ItemTrade>> GetTradeHistoryAsync(int userId)
        {
            try
            {
                var result = await GetAsync<List<ItemTrade>>($"Trade/History/{userId}");
                return result ?? new List<ItemTrade>();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching trade history: {exception.Message}");
                return new List<ItemTrade>();
            }
        }

        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            try
            {
                var result = await GetAsync<List<Item>>($"Trade/Inventory/{userId}");
                return result ?? new List<Item>();
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching user inventory: {exception.Message}");
                return new List<Item>();
            }
        }

        public async Task MarkTradeAsCompletedAsync(int tradeId)
        {
            try
            {
                await PostAsync($"Trade/{tradeId}/Complete", null);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error marking trade as completed: {ex.Message}", ex);
            }
        }

        public async Task TransferItemAsync(TransferItemTradeRequest tradeRequest)
        {
            try
            {
                await PostAsync("Trade/Transfer", tradeRequest);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error transferring item: {ex.Message}", ex);
            }
        }

        public async Task UpdateItemTradeAsync(ItemTrade trade)
        {
            try
            {
                await PutAsync<ItemTrade>($"Trade/{trade.TradeId}", trade);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating trade: {ex.Message}", ex);
            }
        }

        public async Task UpdateTradeAsync(ItemTrade trade)
        {
            try
            {
                await UpdateItemTradeAsync(trade);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating trade: {ex.Message}", ex);
            }
        }
    }
}

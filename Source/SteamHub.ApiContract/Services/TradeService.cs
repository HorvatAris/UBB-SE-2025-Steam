﻿// <copyright file="TradeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Services
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.ItemTrade;
    using SteamHub.ApiContract.Models.ItemTradeDetails;
    using SteamHub.ApiContract.Models.Tag;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UserInventory;
    using SteamHub.ApiContract.Repositories;
    using SteamHub.ApiContract.Services.Interfaces;

    public class TradeService : ITradeService
    {
        private IItemTradeRepository itemTradeRepository;
        private IItemTradeDetailRepository itemTradeDetailRepository;
        private IUserRepository userRepository;
        private IGameRepository gameRepository;
        private IItemRepository itemRepository;
        private IUserInventoryRepository userInventoryRepository;

        public TradeService(IItemTradeRepository itemTradeIItemTradeRepository, IItemTradeDetailRepository itemTradeDetailRepository, IUserRepository userRepository, IGameRepository gameRepository, IItemRepository itemRepository, IUserInventoryRepository userInventoryRepository)
        {
            this.itemTradeRepository = itemTradeIItemTradeRepository;
            this.itemTradeDetailRepository = itemTradeDetailRepository;
            this.userRepository = userRepository;
            this.gameRepository = gameRepository;
            this.itemRepository = itemRepository;
            this.userInventoryRepository = userInventoryRepository;
        }

        public async Task MarkTradeAsCompletedAsync(int tradeId)
        {
            var updateRequest = new UpdateItemTradeRequest
            {
                TradeStatus = TradeStatusEnum.Completed,
                AcceptedBySourceUser = true,
                AcceptedByDestinationUser = true,
            };
            await this.itemTradeRepository.UpdateItemTradeAsync(tradeId, updateRequest);
        }

        public async Task DeclineTradeRequest(ItemTrade trade)
        {
			trade.TradeStatus = "Declined";
			trade.AcceptedBySourceUser = false;
			trade.AcceptedByDestinationUser = false;

			await this.itemTradeRepository.UpdateItemTradeAsync(trade.TradeId, new UpdateItemTradeRequest
			{
				AcceptedByDestinationUser = trade.AcceptedByDestinationUser,
				AcceptedBySourceUser = trade.AcceptedBySourceUser,
				TradeDescription = trade.TradeDescription,
				TradeStatus = TradeStatusEnum.Declined
			});
		}

        public IUserDetails GetCurrentUser()
        {
            throw new NotImplementedException();
        }

        public async Task UpdateItemTradeAsync(ItemTrade trade)
        {
            var tradeStatus = TradeStatusEnum.Pending; // Default value
            if (Enum.TryParse(trade.TradeStatus, out TradeStatusEnum parsedStatus))
            {
                tradeStatus = parsedStatus;
            }

            // 1. Prepare the update trade request
            var updateTradeRequest = new UpdateItemTradeRequest
            {
                TradeDescription = trade.TradeDescription, 
                TradeStatus = trade.AcceptedByDestinationUser ? TradeStatusEnum.Completed : tradeStatus,
                AcceptedBySourceUser = trade.AcceptedBySourceUser,
                AcceptedByDestinationUser = trade.AcceptedByDestinationUser,
            };
            System.Diagnostics.Debug.WriteLine($"Updating trade with ID: {trade.TradeId} to status: {updateTradeRequest.TradeStatus}");

            await this.itemTradeRepository.UpdateItemTradeAsync(trade.TradeId, updateTradeRequest);
        }

        public async Task TransferItemAsync(TransferItemTradeRequest tradeRequest)
        {
            var removeRequest = new ItemFromInventoryRequest
            {
                UserId = tradeRequest.SourceUserId,
                ItemId = tradeRequest.ItemId,
                GameId = tradeRequest.GameId,
            };

            var addRequest = new ItemFromInventoryRequest
            {
                UserId = tradeRequest.DestinationUserId,
                ItemId = tradeRequest.ItemId,
                GameId = tradeRequest.GameId,
            };

            try
            {
                // Remove the item from the source user
                await this.userInventoryRepository.RemoveItemFromUserInventoryAsync(removeRequest);

                // Add the item to the destination user
                await this.userInventoryRepository.AddItemToUserInventoryAsync(addRequest);

                System.Diagnostics.Debug.WriteLine($"Successfully transferred item {tradeRequest.ItemId} from user {tradeRequest.SourceUserId} to user {tradeRequest.DestinationUserId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error transferring item {tradeRequest.ItemId}: {ex.Message}");
                throw new Exception($"Failed to transfer item {tradeRequest.ItemId} from user {tradeRequest.SourceUserId} to user {tradeRequest.DestinationUserId}", ex);
            }
        }

        public async Task AddItemTradeAsync(ItemTrade trade)
        {
            // 1. Create the trade
            var createTradeRequest = new CreateItemTradeRequest
            {
                SourceUserId = trade.SourceUser.UserId,
                DestinationUserId = trade.DestinationUser.UserId,
                GameOfTradeId = trade.GameOfTrade.GameId,
                TradeDate = trade.TradeDate,
                TradeDescription = trade.TradeDescription,
                TradeStatus = TradeStatusEnum.Pending,
                AcceptedBySourceUser = true,  
                AcceptedByDestinationUser = false,
            };
            var createTradeResponse = await this.itemTradeRepository.CreateItemTradeAsync(createTradeRequest);
            System.Diagnostics.Debug.WriteLine($"Trade created with ID: {createTradeResponse.TradeId}");
            trade.TradeId = createTradeResponse.TradeId;

            //// 2. Add source user items
            foreach (var item in trade.SourceUserItems)
            {
                System.Diagnostics.Debug.WriteLine($"Adding item {item.ItemId} to trade {trade.TradeId}");
                var detailRequest = new CreateItemTradeDetailRequest
                {
                    TradeId = trade.TradeId,
                    ItemId = item.ItemId,
                    IsSourceUserItem = true,
                };
                await this.itemTradeDetailRepository.CreateItemTradeDetailAsync(detailRequest);
            }

            // 3. Add destination user items
            foreach (var item in trade.DestinationUserItems)
            {
                var detailRequest = new CreateItemTradeDetailRequest
                {
                    TradeId = trade.TradeId,
                    ItemId = item.ItemId,
                    IsSourceUserItem = false,
                };
                await this.itemTradeDetailRepository.CreateItemTradeDetailAsync(detailRequest);
            }
        }

        public async Task<List<ItemTrade>> GetTradeHistoryAsync(int userId)
        {
            var allTrades = await this.itemTradeRepository.GetItemTradesAsync();

            // 1. Get all trades and filter
            var filteredTrades = allTrades.ItemTrades
                .Where(trade => ((trade.SourceUserId == userId || trade.DestinationUserId == userId)
                         && trade.TradeStatus == TradeStatusEnum.Completed) || trade.TradeStatus == TradeStatusEnum.Declined) // Fixed comparison to use the enum directly
                .ToList();

            var allUsersApi = (await this.userRepository.GetUsersAsync()).Users;
            var allUsers = allUsersApi
                .Select(currentUser =>
                {
                    var user = new User
                    {
                        UserId = currentUser.UserId,
                        Username = currentUser.UserName,
                        Email = currentUser.Email,
                        UserRole = currentUser.UserRole,
                        WalletBalance = currentUser.WalletBalance,
                        PointsBalance = currentUser.PointsBalance,
                    };
                    return user;
                })
                .ToList();

            // 3. Get all games
            var gamesResponse = await this.gameRepository.GetGamesAsync(new GetGamesRequest());
            var allGames = new Collection<Game>(gamesResponse.Select(GameMapper.MapToGame).ToList());

            // 4. Map to domain model
            var result = new List<ItemTrade>();
            foreach (var tradeDto in filteredTrades)
            {
                var sourceUser = allUsers.First(u => u.UserId == tradeDto.SourceUserId);
                var destinationUser = allUsers.First(u => u.UserId == tradeDto.DestinationUserId);

                var game = allGames.FirstOrDefault(g => g.GameId == tradeDto.GameOfTradeId);
                if (game == null)
                {
                    continue; // Skip if game not found
                }

                var itemTrade = new ItemTrade
                {
                    TradeId = tradeDto.TradeId,
                    SourceUser = sourceUser,
                    DestinationUser = destinationUser,
                    GameOfTrade = game,
                    TradeDescription = tradeDto.TradeDescription,
                };

                // Set trade status
                switch (tradeDto.TradeStatus)
                {
                    case TradeStatusEnum.Completed:
                        itemTrade.MarkTradeAsCompleted();
                        break;
                    case TradeStatusEnum.Declined:
                        itemTrade.DeclineTradeRequest();
                        break;
                }

                result.Add(itemTrade);
            }

            // 5. Enrich each trade with its item details
            var allTradeDetails = (await this.itemTradeDetailRepository.GetItemTradeDetailsAsync()).ItemTradeDetails;

            foreach (var trade in result)
            {
                var tradeDetailsForThisTrade = allTradeDetails
                    .Where(tradeDetail => tradeDetail.TradeId == trade.TradeId);

                foreach (var detail in tradeDetailsForThisTrade)
                {
                    var itemResponse = await this.itemTradeRepository.GetItemTradeByIdAsync(detail.TradeId);
                    var itemResponseFromItemProxy = await this.itemRepository.GetItemByIdAsync(detail.ItemId);
                    var gameResponse = await this.gameRepository.GetGameByIdAsync(itemResponse.GameOfTradeId);
                    var itemGame = GameMapper.MapToGame(gameResponse);

                    var item = new Item
                    {
                        ItemId = itemResponseFromItemProxy.ItemId,
                        ItemName = itemResponseFromItemProxy.ItemName,
                        Game = itemGame,
                        GameName = itemGame.GameTitle,
                        Price = (float)itemResponseFromItemProxy.Price,
                        IsListed = itemResponseFromItemProxy.IsListed,
                        Description = itemResponseFromItemProxy.Description,
                        ImagePath = itemResponseFromItemProxy.ImagePath,
                    };

                    if (detail.IsSourceUserItem)
                    {
                        trade.SourceUserItems.Add(item);
                    }
                    else
                    {
                        trade.DestinationUserItems.Add(item);
                    }
                }
            }

            foreach (var tradeFromHistory in result)
            {
                System.Diagnostics.Debug.WriteLine($"Trade ID: {tradeFromHistory.TradeId}, Source User: {tradeFromHistory.SourceUser.Username}, Destination User: {tradeFromHistory.DestinationUser.Username}, Game: {tradeFromHistory.GameOfTrade}");
                System.Diagnostics.Debug.WriteLine(tradeFromHistory.SourceUserItems);
                System.Diagnostics.Debug.WriteLine(tradeFromHistory.DestinationUserItems);
            }

            return result;
        }

        public async Task<List<ItemTrade>> GetActiveTradesAsync(int userId)
        {
            var allTrades = await this.itemTradeRepository.GetItemTradesAsync();

            // 1. Get all trades and filter
            var filteredTrades = allTrades.ItemTrades
                .Where(trade => (trade.SourceUserId == userId || trade.DestinationUserId == userId)
                         && trade.TradeStatus == TradeStatusEnum.Pending) // Fixed comparison to use the enum directly
                .ToList();

            var allUsersApi = (await this.userRepository.GetUsersAsync()).Users;
            var allUsers = allUsersApi
                .Select(tradeUser =>
                {
                    var user = new User
                    {
                        UserId = tradeUser.UserId,
                        Username = tradeUser.UserName,
                        Email = tradeUser.Email,
                        UserRole = tradeUser.UserRole,
                        WalletBalance = tradeUser.WalletBalance,
                        PointsBalance = tradeUser.PointsBalance,
                    };
                    return user;
                })
                .ToList();

            // 3. Get all games
            var gamesResponse = await this.gameRepository.GetGamesAsync(new GetGamesRequest());
            var allGames = new Collection<Game>(gamesResponse.Select(GameMapper.MapToGame).ToList());

            // 4. Map to domain model
            var result = new List<ItemTrade>();
            foreach (var tradeDto in filteredTrades)
            {
                var sourceUser = allUsers.First(currentUser => currentUser.UserId == tradeDto.SourceUserId);
                var destinationUser = allUsers.First(currentUser => currentUser.UserId == tradeDto.DestinationUserId);

                var game = allGames.FirstOrDefault(currentGame => currentGame.GameId == tradeDto.GameOfTradeId);
                if (game == null)
                {
                    continue; // Skip if game not found
                }

                var itemTrade = new ItemTrade
                {
                    TradeId = tradeDto.TradeId,
                    SourceUser = sourceUser,
                    DestinationUser = destinationUser,
                    GameOfTrade = game,
                    TradeDescription = tradeDto.TradeDescription,
                    AcceptedBySourceUser = tradeDto.AcceptedBySourceUser,
                    AcceptedByDestinationUser = tradeDto.AcceptedByDestinationUser,
                };

                // Set trade status
                switch (tradeDto.TradeStatus)
                {
                    case TradeStatusEnum.Completed:
                        itemTrade.MarkTradeAsCompleted();
                        break;
                    case TradeStatusEnum.Declined:
                        itemTrade.DeclineTradeRequest();
                        break;
                }

                result.Add(itemTrade);
            }

            // 5. Enrich each trade with its item details
            var allTradeDetails = (await this.itemTradeDetailRepository.GetItemTradeDetailsAsync()).ItemTradeDetails;

            foreach (var trade in result)
            {
                var tradeDetailsForThisTrade = allTradeDetails
                    .Where(tradeDetail => tradeDetail.TradeId == trade.TradeId);

                foreach (var detail in tradeDetailsForThisTrade)
                {
                    var itemResponse = await this.itemTradeRepository.GetItemTradeByIdAsync(detail.TradeId);
                    var itemResponseFromItemProxy = await this.itemRepository.GetItemByIdAsync(detail.ItemId);
                    var gameResponse = await this.gameRepository.GetGameByIdAsync(itemResponse.GameOfTradeId);
                    var itemGame = GameMapper.MapToGame(gameResponse);

                    var item = new Item
                    {
                        ItemId = itemResponseFromItemProxy.ItemId,
                        ItemName = itemResponseFromItemProxy.ItemName,
                        Game = itemGame,
                        GameName = itemGame.GameTitle,
                        Price = (float)itemResponseFromItemProxy.Price,
                        IsListed = itemResponseFromItemProxy.IsListed,
                        Description = itemResponseFromItemProxy.Description,
                        ImagePath = itemResponseFromItemProxy.ImagePath,
                    };

                    if (detail.IsSourceUserItem)
                    {
                        trade.SourceUserItems.Add(item);
                    }
                    else
                    {
                        trade.DestinationUserItems.Add(item);
                    }
                }
            }

            foreach (var activeTrade in result)
            {
                System.Diagnostics.Debug.WriteLine($"Trade ID: {activeTrade.TradeId}, Source User: {activeTrade.SourceUser.Username}, Destination User: {activeTrade.DestinationUser.Username}, Game: {activeTrade.GameOfTrade}");
                System.Diagnostics.Debug.WriteLine(activeTrade.SourceUserItems);
                System.Diagnostics.Debug.WriteLine(activeTrade.DestinationUserItems);
            }

            return result;
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

        public async Task UpdateTradeAsync(ItemTrade trade)
        {
            try
            {
                await this.UpdateItemTradeAsync(trade);
            }
            catch (Exception tradeUpdateException)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating trade: {tradeUpdateException.Message}");
                throw;
            }
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

                // If both users have accepted, complete the trade
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

        public async Task CompleteTradeAsync(ItemTrade trade)
        {
            try
            {
                // Transfer source user items to destination user
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


                // Transfer destination user items to source user
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

        public async Task<List<Item>> GetUserInventoryAsync(int userId)
        {
            var inventoryResponse = await this.userInventoryRepository.GetUserInventoryAsync(userId);
            var allGamesResponse = await this.gameRepository.GetGamesAsync(new GetGamesRequest());
            var result = new List<Item>();
            var allGames = allGamesResponse.Select(GameMapper.MapToGame).ToList();
            foreach (var inventoryItem in inventoryResponse.Items)
            {
                var matchingGame = allGames
                    .FirstOrDefault(game => string.Equals(game.GameTitle, inventoryItem.GameName, StringComparison.OrdinalIgnoreCase));
                
                if(matchingGame != null)
                {
                    var item = new Item
                    {
                        ItemId = inventoryItem.ItemId,
                        ItemName = inventoryItem.ItemName,
                        Game = matchingGame,
                        GameName = matchingGame.GameTitle,
                        Price = inventoryItem.Price,
                        IsListed = inventoryItem.IsListed,
                        Description = inventoryItem.Description,
                        ImagePath = inventoryItem.ImagePath,
                    };
                    result.Add(item);
                }
            }

            return result;
        }
    }
}

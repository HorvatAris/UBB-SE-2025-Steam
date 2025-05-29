﻿// <copyright file="ITradeService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Item;
    using SteamHub.ApiContract.Models.ItemTrade;
    using SteamHub.ApiContract.Models.User;

    public interface ITradeService
    {
        Task<List<ItemTrade>> GetActiveTradesAsync(int userId);

        Task AddItemTradeAsync(ItemTrade trade);

        Task MarkTradeAsCompletedAsync(int tradeId);

        Task DeclineTradeRequest(ItemTrade trade);

        Task UpdateItemTradeAsync(ItemTrade trade);

        Task TransferItemAsync(TransferItemTradeRequest tradeRequest);

        Task<List<ItemTrade>> GetTradeHistoryAsync(int userId);

        Task CreateTradeAsync(ItemTrade trade);

        Task UpdateTradeAsync(ItemTrade trade);

        Task AcceptTradeAsync(ItemTrade trade, bool isSourceUser);

        Task CompleteTradeAsync(ItemTrade trade);

        Task<List<Item>> GetUserInventoryAsync(int userId);
    }
}

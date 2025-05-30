﻿// <copyright file="IUserGameService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ApiContract.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Models.UsersGames;

    public interface IUserGameService
    {
        int LastEarnedPoints { get; }

        IUserDetails GetUser();

        Task RemoveGameFromWishlistAsync(UserGameRequest gameRequest);

        Task AddGameToWishlistAsync(UserGameRequest gameRequest);

        Task<int> PurchaseGamesAsync(PurchaseGamesRequest request);

        Task ComputeNoOfUserGamesForEachTagAsync(Collection<Tag> all_tags, int userId);

        Task<Collection<Tag>> GetFavoriteUserTagsAsync(int userId);

        Task ComputeTagScoreForGamesAsync(Collection<Game> games, int userId);

        void ComputeTrendingScores(Collection<Game> games);

        Task<Collection<Game>> GetRecommendedGamesAsync(int userId);

        Task<Collection<Game>> GetWishListGamesAsync(int userId);

        Task<Collection<Game>> GetAllGamesAsync(int userId);

        Task<Collection<Game>> SearchWishListByNameAsync(string searchText);

        Task<Collection<Game>> FilterWishListGamesAsync(string criteria);

        Task<bool> IsGamePurchasedAsync(Game game, int userId);

        Task<Collection<Game>> SortWishListGamesAsync(string criteria, bool ascending);

        Task<Collection<Game>> GetPurchasedGamesAsync(int userId);
    }
}

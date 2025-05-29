﻿// <copyright file="UserGameService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SteamHub.ApiContract.Constants;
using SteamHub.ApiContract.Models.Game;
using SteamHub.ApiContract.Models.Tag;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Services.Interfaces;

public class UserGameService : IUserGameService
{
    private const int InitialValueForLastEarnedPoints = 0;
    private const int ResetValueForNumberOfUserGamesWithTag = 0;
    private const int NumberOfFavouriteTagsToTake = 3;
    private const int StartingIndexValue = 0;
    private const int InitialTagScore = 0;
    private const int TagScoreMultiplierNumerator = 1;
    private const decimal TagScoreMultiplierDenominator = 3m;
    private const decimal WeightedScoreMultiplier = 0.5m;
    private const int NumberOfSortegGamesShown = 10;
    private const decimal MinimumValueForOverwhelminglyPositive = 4.5m;
    private const decimal MinimumValueForVeryPositive = 4m;
    private const decimal MinimumValueForMixed = 2m;
    private const int ValueToDecrementPositionWith = 1;
    private const int ValueToIncrementPositionWith = 1;


    public UserGameService(IUserRepository userRepository, IUsersGamesRepository userGameRepository, IGameRepository gameRepository, ITagRepository tagRepository, IWalletRepository walletRepository)
    {
        this.UserRepository = userRepository;
        this.UserGameRepository = userGameRepository;
        this.GameRepository = gameRepository;
        this.TagRepository = tagRepository;
        this.WalletRepository = walletRepository;
    }

    // Property to track points earned in the last purchase

    public int LastEarnedPoints { get; private set; }

    private IUserRepository UserRepository { get; set; }

    private IUsersGamesRepository UserGameRepository { get; set; }

    private IGameRepository GameRepository { get; set; }

    private ITagRepository TagRepository { get; set; }

    private IWalletRepository WalletRepository { get; set; }

    public async Task RemoveGameFromWishlistAsync(UserGameRequest gameRequest)
    {
        //var request = new UserGameRequest
        //{
        //    UserId = this.user.UserId,
        //    GameId = game.GameId,
        //};
        await this.UserGameRepository.RemoveFromWishlistAsync(gameRequest);
    }

    public async Task AddGameToWishlistAsync(UserGameRequest gameRequest)
    {
        try
        {
            var gameResponse = await GameRepository.GetGameByIdAsync(gameRequest.GameId);
            if (gameResponse == null)
            {
                throw new Exception($"Game with ID {gameRequest.GameId} not found.");
            }

            var game = GameMapper.MapToGame(gameResponse);

            // Check if game is already purchased
            if (await this.IsGamePurchasedAsync(game, gameRequest.UserId))
            {
                throw new Exception(string.Format(ExceptionMessages.GameAlreadyOwned, game.GameTitle));
            }

            var wishlistGames = await this.GetWishListGamesAsync(gameRequest.UserId);
            foreach (var wishlistGame in wishlistGames)
            {
                if (game.GameId == wishlistGame.GameId)
                {
                    throw new Exception(string.Format(ExceptionMessages.GameAlreadyInWishlist, game.GameTitle));
                }
            }
            await this.UserGameRepository.AddToWishlistAsync(gameRequest);
        }
        catch (Exception exception)
        {
            throw;
        }
    }

    public async Task<Collection<Game>> GetAllGamesAsync(int userId)
    {
        try
        {
            var response = await this.UserGameRepository.GetUserGamesAsync(userId);
            var userGamesResponses = response.UserGames;
            System.Diagnostics.Debug.WriteLine($"UserGamesResponses: {userGamesResponses.Count}");
            var gameIds = userGamesResponses
                .Select(game => game.GameId)
                .ToList();
            if (gameIds.Count == 0)
            {
                return new Collection<Game>();
            }

            var games = new Collection<Game>();
            foreach (var gameId in gameIds)
            {
                System.Diagnostics.Debug.WriteLine($"GameId: {gameId}");
                var game = GameMapper.MapToGame(await this.GameRepository.GetGameByIdAsync(gameId));
                games.Add(game);
            }

            return games;
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching user games: {exception.Message}");
            return new Collection<Game>();
        }
    }

    public async Task<int> PurchaseGamesAsync(PurchaseGamesRequest purchaseRequest)
    {
        // Reset points counter
        this.LastEarnedPoints = InitialValueForLastEarnedPoints;

        // Track user's points before purchase
        var user = await this.UserRepository.GetUserByIdAsync(purchaseRequest.UserId);
        var pointsBalanceBefore = await this.WalletRepository.GetPointsFromWallet(purchaseRequest.UserId);

        // Purchase games
        foreach (var game in purchaseRequest.Games)
        {
            var request = new UserGameRequest
            {
                UserId = user.UserId,
                GameId = game.GameId,
            };
            if (purchaseRequest.IsWalletPayment)
            {
                await this.WalletRepository.BuyWithMoney(game.Price, user.UserId);
            }
            await this.UserGameRepository.PurchaseGameAsync(request);
        }

        decimal totalSpent = purchaseRequest.Games.Sum(g => g.Price);
        int pointsToAward = (int)(totalSpent * 121);

        await this.WalletRepository.AddPointsToWallet(pointsToAward, user.UserId);

        // Calculate earned points by comparing balances
        float pointsBalanceAfter = await this.WalletRepository.GetPointsFromWallet(user.UserId);
        this.LastEarnedPoints = (int)(pointsBalanceAfter - pointsBalanceBefore);

        var updateUserRequest = new UpdateUserRequest
        {
            UserName = user.Username,
            Email = user.Email,
            WalletBalance = user.WalletBalance - (decimal)totalSpent,
            PointsBalance = user.PointsBalance + pointsToAward,
            UserRole = user.UserRole,
        };

        await this.UserRepository.UpdateUserAsync(user.UserId, updateUserRequest);

        return pointsToAward;
    }

    public async Task ComputeNoOfUserGamesForEachTagAsync(Collection<Tag> all_tags, int userId)
    {
        var user_games = await this.GetAllGamesAsync(userId);

        // Manually build the dictionary instead of using ToDictionary
        Dictionary<string, Tag> tagsDictionary = new Dictionary<string, Tag>();
        foreach (var tag in all_tags)
        {
            if (!tagsDictionary.ContainsKey(tag.Tag_name))
            {
                tagsDictionary.Add(tag.Tag_name, tag);
            }
        }

        foreach (var tag in tagsDictionary.Values)
        {
            tag.NumberOfUserGamesWithTag = ResetValueForNumberOfUserGamesWithTag;
        }

        foreach (var user_game in user_games)
        {
            foreach (string tag_name in user_game.Tags)
            {
                if (tagsDictionary.ContainsKey(tag_name))
                {
                    tagsDictionary[tag_name].NumberOfUserGamesWithTag++;
                }
            }
        }
    }

    public async Task<Collection<Tag>> GetFavoriteUserTagsAsync(int userId)
    {
        var tagsResponse = await this.TagRepository.GetAllTagsAsync();
        var allTags = new Collection<Tag>(tagsResponse.Tags.Select(TagMapper.MapToTag).ToList());
        await this.ComputeNoOfUserGamesForEachTagAsync(allTags, userId);

        List<Tag> sortedTags = new List<Tag>(allTags);

        for (int currentIndex = StartingIndexValue; currentIndex < sortedTags.Count - ValueToDecrementPositionWith; currentIndex++)
        {
            for (int comparisonIndex = currentIndex + ValueToIncrementPositionWith; comparisonIndex < sortedTags.Count; comparisonIndex++)
            {
                if (sortedTags[comparisonIndex].NumberOfUserGamesWithTag > sortedTags[currentIndex].NumberOfUserGamesWithTag)
                {
                    var tagToSwap = sortedTags[currentIndex];
                    sortedTags[currentIndex] = sortedTags[comparisonIndex];
                    sortedTags[comparisonIndex] = tagToSwap;
                }
            }
        }

        List<Tag> topTags = new List<Tag>();
        for (int tagIndex = StartingIndexValue; tagIndex < sortedTags.Count && tagIndex < NumberOfFavouriteTagsToTake; tagIndex++)
        {
            topTags.Add(sortedTags[tagIndex]);
        }

        return new Collection<Tag>(topTags);
    }

    public async Task ComputeTagScoreForGamesAsync(Collection<Game> games, int userId)
    {
        var favorite_tags = await this.GetFavoriteUserTagsAsync(userId);
        foreach (var game in games)
        {
            game.TagScore = InitialTagScore;
            foreach (var tag in favorite_tags)
            {
                if (game.Tags.Contains(tag.Tag_name))
                {
                    game.TagScore += tag.NumberOfUserGamesWithTag;
                }
            }

            game.TagScore = game.TagScore * (TagScoreMultiplierNumerator / TagScoreMultiplierDenominator);
        }
    }

    public void ComputeTrendingScores(Collection<Game> games)
    {
        var maxRecentSales = games.Max(game => game.NumberOfRecentPurchases);
        foreach (var game in games)
        {
            game.TrendingScore = Convert.ToDecimal(game.NumberOfRecentPurchases) / maxRecentSales;
        }
    }

    public async Task<Collection<Game>> GetRecommendedGamesAsync(int userId)
    {
        var games = await this.GameRepository.GetGamesAsync(
            new GetGamesRequest());
        var allGames = new Collection<Game>(games.Select(GameMapper.MapToGame).ToList());
        var approvedGames = new Collection<Game>();
        foreach (var game in allGames)
        {
            if (game.Status == "Approved")
            {
                approvedGames.Add(game);
            }
        }

        this.ComputeTrendingScores(approvedGames);
        await this.ComputeTagScoreForGamesAsync(approvedGames, userId);

        List<Game> sortedGames = new List<Game>(approvedGames);

        // Manual sorting based on weighted score
        for (int currentIndex = StartingIndexValue; currentIndex < sortedGames.Count - 1; currentIndex++)
        {
            for (int comparisonIndex = currentIndex + 1; comparisonIndex < sortedGames.Count; comparisonIndex++)
            {
                decimal currentScore = (sortedGames[currentIndex].TagScore * WeightedScoreMultiplier) + (sortedGames[currentIndex].TrendingScore * WeightedScoreMultiplier);
                decimal comparisonScore = (sortedGames[comparisonIndex].TagScore * WeightedScoreMultiplier) + (sortedGames[comparisonIndex].TrendingScore * WeightedScoreMultiplier);

                if (comparisonScore > currentScore)
                {
                    Game gameToSwap = sortedGames[currentIndex];
                    sortedGames[currentIndex] = sortedGames[comparisonIndex];
                    sortedGames[comparisonIndex] = gameToSwap;
                }
            }
        }

        // Take top games
        List<Game> recommendedGames = new List<Game>();
        int limit = sortedGames.Count < NumberOfSortegGamesShown ? sortedGames.Count : NumberOfSortegGamesShown;
        for (int gameIndex = StartingIndexValue; gameIndex < limit; gameIndex++)
        {
            recommendedGames.Add(sortedGames[gameIndex]);
        }

        return new Collection<Game>(recommendedGames);
    }

    public async Task<Collection<Game>> GetWishListGamesAsync(int userId)
    {
        try
        {
            var response = await this.UserGameRepository.GetUserWishlistAsync(userId);
            var userGamesResponses = response.UserGames;
            System.Diagnostics.Debug.WriteLine($"UserGamesResponses: {userGamesResponses.Count}");
            var gameIds = userGamesResponses
                .Select(game => game.GameId)
                .ToList();
            if (gameIds.Count == 0)
            {
                return new Collection<Game>();
            }

            var games = new Collection<Game>();
            foreach (var gameId in gameIds)
            {
                System.Diagnostics.Debug.WriteLine($"GameId: {gameId}");
                var game = GameMapper.MapToGame(await this.GameRepository.GetGameByIdAsync(gameId));
                games.Add(game);
            }

            return games;
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching user games: {exception.Message}");
            return new Collection<Game>();
        }
    }

    public async Task<Collection<Game>> SearchWishListByNameAsync(string searchText)
    {
        List<Game> allWishListGames = (await this.GetWishListGamesAsync(this.GetUser().UserId)).ToList();
        List<Game> matchingGames = new List<Game>();

        foreach (Game game in allWishListGames)
        {
            if (game.GameTitle != null && game.GameTitle.ToLower().Contains(searchText.ToLower()))
            {
                matchingGames.Add(game);
            }
        }

        return new Collection<Game>(matchingGames);
    }

    public async Task<Collection<Game>> FilterWishListGamesAsync(string criteria)
    {
        Collection<Game> games = await this.GetWishListGamesAsync(this.GetUser().UserId);
        Collection<Game> filteredGames = new Collection<Game>();

        bool isKnownCriteria = criteria == FilterCriteria.OVERWHELMINGLYPOSITIVE ||
                               criteria == FilterCriteria.VERYPOSITIVE ||
                               criteria == FilterCriteria.MIXED ||
                               criteria == FilterCriteria.NEGATIVE;

        if (!isKnownCriteria)
        {
            // If the criteria is not recognized, return the full list
            return games;
        }

        foreach (Game game in games)
        {
            if (criteria == FilterCriteria.OVERWHELMINGLYPOSITIVE && game.Rating >= MinimumValueForOverwhelminglyPositive)
            {
                filteredGames.Add(game);
            }
            else if (criteria == FilterCriteria.VERYPOSITIVE &&
                     game.Rating >= MinimumValueForVeryPositive &&
                     game.Rating < MinimumValueForOverwhelminglyPositive)
            {
                filteredGames.Add(game);
            }
            else if (criteria == FilterCriteria.MIXED &&
                     game.Rating >= MinimumValueForMixed &&
                     game.Rating < MinimumValueForVeryPositive)
            {
                filteredGames.Add(game);
            }
            else if (criteria == FilterCriteria.NEGATIVE &&
                     game.Rating < MinimumValueForMixed)
            {
                filteredGames.Add(game);
            }
        }

        return filteredGames;
    }

    public async Task<Collection<Game>> GetPurchasedGamesAsync(int userId)
    {
        try
        {
            var response = await this.UserGameRepository.GetUserPurchasedGamesAsync(userId);
            var userGamesResponses = response.UserGames;
            System.Diagnostics.Debug.WriteLine($"UserGamesResponses: {userGamesResponses.Count}");
            var gameIds = userGamesResponses
                .Select(game => game.GameId)
                .ToList();
            if (gameIds.Count == 0)
            {
                return new Collection<Game>();
            }

            var games = new Collection<Game>();
            foreach (var gameId in gameIds)
            {
                System.Diagnostics.Debug.WriteLine($"GameId: {gameId}");
                var game = GameMapper.MapToGame(await this.GameRepository.GetGameByIdAsync(gameId));
                games.Add(game);
            }

            return games;
        }
        catch (Exception exception)
        {
            System.Diagnostics.Debug.WriteLine($"Error fetching user games: {exception.Message}");
            return new Collection<Game>();
        }
    }

    public async Task<bool> IsGamePurchasedAsync(Game game, int userId)
    {
        var purchasedGameList = await this.GetPurchasedGamesAsync(userId);
        return purchasedGameList.Any(currentGame => currentGame.GameId == game.GameId);
    }

    public async Task<Collection<Game>> SortWishListGamesAsync(string criteria, bool ascending)
    {
        Collection<Game> gamesCollection = await this.GetWishListGamesAsync(this.GetUser().UserId);
        List<Game> games = new List<Game>();

        foreach (var game in gamesCollection)
        {
            games.Add(game);
        }

        if (criteria == FilterCriteria.PRICE)
        {
            if (ascending)
            {
                games.Sort(this.CompareByPriceAscending);
            }
            else
            {
                games.Sort(this.CompareByPriceDescending);
            }
        }
        else if (criteria == FilterCriteria.RATING)
        {
            if (ascending)
            {
                games.Sort(this.CompareByRatingAscending);
            }
            else
            {
                games.Sort(this.CompareByRatingDescending);
            }
        }
        else if (criteria == FilterCriteria.DISCOUNT)
        {
            if (ascending)
            {
                games.Sort(this.CompareByDiscountAscending);
            }
            else
            {
                games.Sort(this.CompareByDiscountDescending);
            }
        }
        else
        {
            if (ascending)
            {
                games.Sort(this.CompareByNameAscending);
            }
            else
            {
                games.Sort(this.CompareByNameDescending);
            }
        }

        return new Collection<Game>(games);
    }

    private int CompareByPriceAscending(Game firstGame, Game secondGame)
    {
        return firstGame.Price.CompareTo(secondGame.Price);
    }

    private int CompareByPriceDescending(Game firstGame, Game secondGame)
    {
        return secondGame.Price.CompareTo(firstGame.Price);
    }

    private int CompareByRatingAscending(Game firstGame, Game secondGame)
    {
        return firstGame.Rating.CompareTo(secondGame.Rating);
    }

    private int CompareByRatingDescending(Game firstGame, Game secondGame)
    {
        return secondGame.Rating.CompareTo(firstGame.Rating);
    }

    private int CompareByDiscountAscending(Game firstGame, Game secondGame)
    {
        return firstGame.Discount.CompareTo(secondGame.Discount);
    }

    private int CompareByDiscountDescending(Game firstGame, Game secondGame)
    {
        return secondGame.Discount.CompareTo(firstGame.Discount);
    }

    private int CompareByNameAscending(Game firstGame, Game secondGame)
    {
        return string.Compare(firstGame.GameTitle, secondGame.GameTitle, StringComparison.OrdinalIgnoreCase);
    }

    private int CompareByNameDescending(Game firstGame, Game secondGame)
    {
        return string.Compare(secondGame.GameTitle, firstGame.GameTitle, StringComparison.OrdinalIgnoreCase);
    }

    public IUserDetails GetUser()
    {
        throw new NotImplementedException();
    }
}
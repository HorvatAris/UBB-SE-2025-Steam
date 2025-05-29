// <copyright file="GamePageViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.Game;
using SteamHub.Pages;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.UsersGames;
using SteamHub.ApiContract.Models.User;

public class GamePageViewModel : INotifyPropertyChanged
{
    private const int MaxSimilarGamesToDisplay = 3;
    private const string CurrencySymbol = "$";
    private const string PriceFormat = "F2";

    private readonly ICartService cartService;
    private readonly IUserGameService userGameService;
    private readonly IGameService gameService;
    private readonly IReviewService reviewService;
    private IUserDetails user;

    private Game game;
    private ObservableCollection<Game> similarGames;
    private bool isOwned = false;
    private ObservableCollection<string> gameTags;
    private ObservableCollection<string> mediaLinks;

    public GamePageViewModel(IGameService gameService, ICartService cartService, IUserGameService userGameService, IReviewService reviewService)
    {
        this.cartService = cartService;
        this.userGameService = userGameService;
        this.gameService = gameService;
        this.reviewService = reviewService;
        this.user = this.cartService.GetUser();
        this.SimilarGames = new ObservableCollection<Game>();
        this.GameTags = new ObservableCollection<string>();
        this.MediaLinks = new ObservableCollection<string>();
        this.reviewService = reviewService;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public string GameTitle => this.Game?.GameTitle ?? string.Empty;

    public string GameDescription => this.Game?.GameDescription ?? string.Empty;

    public string Developer => this.Game != null ? $"Developer: {this.Game.GameTitle}" : string.Empty;

    public string MinimumRequirements => this.Game?.MinimumRequirements ?? string.Empty;

    public string RecommendedRequirements => this.Game?.RecommendedRequirements ?? string.Empty;

    public string ImagePath => this.Game?.ImagePath ?? string.Empty;

    public double Rating => Convert.ToDouble(this.Game?.Rating ?? 5);

    public string OwnedStatus => this.IsOwned ? "Owned" : "Not Owned";

    public Game Game
    {
        get => this.game;
        set
        {
            this.game = value;
            this.OnPropertyChanged();
            this.UpdateMediaLinks();
            _ = this.UpdateIsOwnedStatusAsync();
            _ = this.UpdateGameTagsAsync();
        }
    }

    // public string FormattedPrice => this.Game != null ? $"${this.Game.Price:F2}" : string.Empty;
    public string FormattedPrice => this.Game != null ? $"{CurrencySymbol}{this.Game.Price.ToString(PriceFormat)}" : string.Empty;

    public ObservableCollection<string> GameTags
    {
        get => this.gameTags;
        private set
        {
            this.gameTags = value;
            this.OnPropertyChanged();
        }
    }

    public bool IsOwned
    {
        get => this.isOwned;
        private set
        {
            if (this.isOwned != value)
            {
                this.isOwned = value;
                this.OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<Game> SimilarGames
    {
        get => this.similarGames;
        set
        {
            this.similarGames = value;
            this.OnPropertyChanged();
        }
    }

    public ObservableCollection<string> MediaLinks
    {
        get => this.mediaLinks;
        private set
        {
            this.mediaLinks = value;
            this.OnPropertyChanged();
        }
    }

    public async Task LoadGame(Game game)
    {
        this.Game = game;
        this.OnPropertyChanged(nameof(this.Game));
        this.OnPropertyChanged(nameof(this.Rating));
        await this.LoadSimilarGamesAsync();
    }

    public async Task LoadGameById(int gameId)
    {
        if (this.gameService == null)
        {
            return;
        }

        this.Game = await this.gameService.GetGameByIdAsync(gameId);
        if (this.Game != null)
        {
            this.OnPropertyChanged(nameof(this.Game)); // Let UI know the Game has changed

            await this.LoadSimilarGamesAsync();
        }
    }

    // Add game to cart - safely handle null CartService
    public async Task AddToCartAsync()
    {
        if (this.Game != null && this.cartService != null)
        {
            try
            {
                var gameRequest = new UserGameRequest
                {
                    UserId = this.user.UserId,
                    GameId = this.Game.GameId
                };
                await this.cartService.AddGameToCartAsync(gameRequest);
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding game to cart: {exception.Message}");
                throw new Exception(exception.Message);
            }
        }
    }

    // Add game to wishlist - this will be implemented later
    public async Task AddToWishlistAsync()
    {
        if (this.Game != null && this.userGameService != null)
        {
            var gameRequest = new UserGameRequest
            {
                UserId = this.user.UserId,
                GameId = this.Game.GameId
            };
            try
            {
                await this.userGameService.AddGameToWishlistAsync(gameRequest);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
        }
    }

    public void GetSimilarGames(Game game, Frame frame)
    {
        if (frame != null)
        {
            var gamePage = new GamePage(this.gameService, this.cartService, this.userGameService, this.reviewService, game);

            frame.Content = gamePage;
        }
        else
        {
            frame.Navigate(typeof(GamePage), game);
        }
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private async Task UpdateIsOwnedStatusAsync()
    {
        if (this.Game == null || this.userGameService == null)
        {
            this.IsOwned = false;
            return;
        }

        try
        {
            this.IsOwned = await this.userGameService.IsGamePurchasedAsync(this.Game, this.user.UserId);
        }
        catch (Exception)
        {
            this.IsOwned = false;
        }
    }

    private async Task UpdateGameTagsAsync()
    {
        if (this.Game == null || this.gameService == null)
        {
            this.GameTags.Clear();
            return;
        }

        try
        {
            var allTags = await this.gameService.GetAllGameTagsAsync(this.Game);
            this.GameTags.Clear();
            foreach (var tag in allTags)
            {
                this.GameTags.Add(tag.Tag_name);
            }
        }
        catch (Exception)
        {
            this.GameTags.Clear();
        }
    }

    private void UpdateMediaLinks()
    {
        this.MediaLinks.Clear();
        if (!string.IsNullOrEmpty(this.Game?.TrailerPath))
        {
            this.MediaLinks.Add(this.Game.TrailerPath);
        }

        if (!string.IsNullOrEmpty(this.Game?.GameplayPath))
        {
            this.MediaLinks.Add(this.Game.GameplayPath);
        }
    }

    // Load similar games based on current game
    private async Task LoadSimilarGamesAsync()
    {
        if (this.Game == null || this.gameService == null)
        {
            return;
        }

        var similarGames = await this.gameService.GetSimilarGamesAsync(this.Game.GameId);
        this.SimilarGames = new ObservableCollection<Game>(similarGames.Take(MaxSimilarGamesToDisplay));
    }

    public void NavigateToReviewsPage(Frame parentFrame, int gameId)
    {
        if (parentFrame != null)
        {
            var reviewsPage = new ReviewsPage(this.reviewService);
            parentFrame.Navigate(typeof(ReviewsPage), gameId);
        }
    }

}
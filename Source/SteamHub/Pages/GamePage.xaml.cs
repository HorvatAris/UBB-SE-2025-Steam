﻿// <copyright file="GamePage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Media.Imaging;
    using Microsoft.UI.Xaml.Navigation;
    using SteamHub.ApiContract.Constants;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Services.Interfaces;
    using Windows.Foundation;
    using Windows.Foundation.Collections;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GamePage : Page
    {
        private readonly IReviewService _reviewService;
        public GamePage(
            IGameService gameService,
            ICartService cartService,
            IUserGameService userGameService,
            IReviewService reviewService,
            Game game = null)
        {
            this.InitializeComponent();
            this._reviewService = reviewService;

            this.ViewModel = new GamePageViewModel(gameService, cartService, userGameService, reviewService);

            this.DataContext = this.ViewModel;

            this.Loaded += async (_, __) =>
            {
                if (game != null)
                {
                    await this.ViewModel.LoadGame(game);
                }
            };
        }

        private GamePageViewModel ViewModel { get; }

        protected override async void OnNavigatedTo(NavigationEventArgs navigationEventArguments)
        {
            base.OnNavigatedTo(navigationEventArguments);

            if (navigationEventArguments.Parameter is Game selectedGame)
            {
                await this.ViewModel.LoadGame(selectedGame);
            }
            else if (navigationEventArguments.Parameter is int gameId)
            {
                await this.ViewModel.LoadGameById(gameId);
            }
        }

        private async void BuyButton_Click(object buyButtonSender, RoutedEventArgs buyClickEventArgument)
        {
            try
            {
                await this.ViewModel.AddToCartAsync();
                this.ShowNotification(
                    NotificationStrings.AddToCartSuccessTitle,
                    string.Format(NotificationStrings.AddToCartSuccessMessage, this.ViewModel.Game.GameTitle));
            }
            catch (Exception exception)
            {
                this.ShowNotification(
                    NotificationStrings.AddToCartErrorTitle,
                    string.Format(NotificationStrings.AddToCartErrorMessage, this.ViewModel.Game.GameTitle) + " " + exception.Message);
            }
        }

        private async void WishlistButton_Click(object wishListButtonSender, RoutedEventArgs wishListClickEventArgument)
        {
            try
            {
                await this.ViewModel.AddToWishlistAsync();
                this.ShowNotification(
                    NotificationStrings.AddToWishlistSuccessTitle,
                    string.Format(NotificationStrings.AddToWishlistSuccessMessage, this.ViewModel.Game.GameTitle));
            }
            catch (Exception exception)
            {
                var message = exception.Message.Contains(ErrorStrings.SqlNonQUeryFailure)
                    ? string.Format(ErrorStrings.AddToWishlistAlreadyExistsError, this.ViewModel.Game.GameTitle)
                    : exception.Message;

                this.ShowNotification(NotificationStrings.AddToWishlistErrorTitle, message);
            }
        }

        private void SimilarGame_Click(object similarGameButton, RoutedEventArgs similarGamesClickEventArgument)
        {
            if (similarGameButton is Button button && button.Tag is Game game)
            {
                Frame frame = this.Parent as Frame;
                this.ViewModel.GetSimilarGames(game, frame);
            }
        }

        private void ViewReviewsButton_Click(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent;

            while (parent != null && !(parent is Frame) && !(parent is ContentControl))
            {
                parent = (parent as FrameworkElement)?.Parent;
            }

            if (parent is Frame parentFrame && this._reviewService != null && this.ViewModel.Game != null)
            {
                var reviewsPage = new ReviewsPage(this._reviewService);
                parentFrame.Content = reviewsPage;

                reviewsPage.NavigateToGameReviews(this.ViewModel.Game.GameId);
            }
            else if (parent is ContentControl parentContent && this._reviewService != null && this.ViewModel.Game != null)
            {
                var reviewsPage = new ReviewsPage(this._reviewService);
                parentContent.Content = reviewsPage;

                reviewsPage.NavigateToGameReviews(this.ViewModel.Game.GameId);
            }
            else
            {
                Debug.WriteLine("Cannot show ReviewsPage: Parent, ReviewService, or Game is null.");
            }
        }


        private void ShowNotification(string title, string subtitle)
        {
            this.NotificationTip.Title = title;
            this.NotificationTip.Subtitle = subtitle;
            this.NotificationTip.IsOpen = true;
        }
    }
}
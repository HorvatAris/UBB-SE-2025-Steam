// <copyright file="WishListView.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Pages
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;
    using SteamHub;
    using SteamHub.ApiContract.Models;
    using SteamHub.Pages;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Services.Interfaces;
    using SteamHub.ViewModels;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WishListView : Page
    {
        private WishListViewModel viewModel;

        public WishListView(IUserGameService userGameService, IGameService gameService, ICartService cartService, IReviewService reviewService)
        {
            this.InitializeComponent();
            this.viewModel = new WishListViewModel(userGameService, gameService, cartService, reviewService);
            this.DataContext = this.viewModel;
        }

        private void ViewDetails_Click(object viewDetailsButton, RoutedEventArgs viewDetailsEventArgument)
        {
            if (viewDetailsButton is Button button && button.DataContext is Game game)
            {
                if (this.Parent is Frame frame)
                {
                    this.viewModel.ViewGameDetails(frame, game);
                }
            }
        }

        private void BackButton_Click(object backButton, RoutedEventArgs backButtonClickEventArgument)
        {
            if (this.Parent is Frame frame)
            {
                this.viewModel.BackToHomePage(frame);
            }
        }

        private async void SearchBox_TextChanged(object searchTextBox, TextChangedEventArgs textChangedEventArguments)
        {
            if (this.viewModel != null)
            {
                await this.viewModel.HandleSearchWishListGames();
            }
        }
    }
}
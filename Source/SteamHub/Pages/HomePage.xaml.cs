// <copyright file="HomePage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Pages
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Navigation;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Models.Game;
    using SteamHub.ApiContract.Models.Tag;
    using SteamHub.ApiContract.Services.Interfaces;
    using Windows.Foundation;
    using Windows.Foundation.Collections;

    public sealed partial class HomePage : Page
    {
        private const int MINIMUMPRICEFILTERVALUE = 0;
        private const int MAXIMUMPRICEFILTERVALUE = 200;
        private const int RATINGFILTERVALUE = 0;

        public HomePage(
            IGameService gameService,
            ICartService cartService,
            IUserGameService userGameService,
            IReviewService reviewService)
        {
            this.InitializeComponent();
            this.HomePageViewModel = new HomePageViewModel(gameService, userGameService, cartService, reviewService);
            this.DataContext = this.HomePageViewModel;
            this.Loaded += async (_, __) => await this.HomePageViewModel.InitAsync();
        }

        private HomePageViewModel HomePageViewModel { get; set; }

        private async void SearchBox_TextChanged(object searchBox, TextChangedEventArgs textChangedEventArgument)
        {
            await this.HomePageViewModel.SearchGames(this.SearchBox.Text);
        }

        private void FilterButton_Click(object filterButton, RoutedEventArgs filterClickEventArgument)
        {
            this.FilterPopup.IsOpen = true;
        }

        private async void ApplyFilters_Click(object applyFiltersButton, RoutedEventArgs applyFiltersArgument)
        {
            // You can access the filter values from PopupRatingSlider, MinPriceSlider, MaxPriceSlider here.
            this.HomePageViewModel.RatingFilter = (int)this.PopupRatingSlider.Value;
            this.HomePageViewModel.MinPrice = (int)this.MinPriceSlider.Value;
            this.HomePageViewModel.MaxPrice = (int)this.MaxPriceSlider.Value;

            this.HomePageViewModel.SelectedTags.Clear();
            foreach (var item in this.TagListView.SelectedItems)
            {
                if (item is Tag tag)
                {
                    this.HomePageViewModel.SelectedTags.Add(tag.Tag_name);
                }
            }

            await this.HomePageViewModel.ApplyFilters();

            // Close the popup
            this.FilterPopup.IsOpen = false;
        }

        private async void ResetFilters_Click(object resetFiltersButton, RoutedEventArgs resetFiltersClickEventArgument)
        {
            this.HomePageViewModel.ResetFilters();
            this.PopupRatingSlider.Value = 0;
            this.MinPriceSlider.Value = 0;
            this.MaxPriceSlider.Value = 200;
            this.TagListView.SelectedItems.Clear();

            if (this.DataContext is HomePageViewModel viewModel)
            {
                await viewModel.LoadAllGames();
            }

            this.FilterPopup.IsOpen = false;
        }

        // Navigation to GamePage
        private void ListView_SelectionChanged(object gameListView, SelectionChangedEventArgs selectionChangedEventArgument)
        {
            if (gameListView is ListView listView && listView.SelectedItem is Game selectedGame)
            {
                this.HomePageViewModel.NavigateToGamePage(this.Parent as Frame, selectedGame);
                listView.SelectedItem = null;
            }
        }
    }
}

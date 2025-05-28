// <copyright file="FeaturesPage.xaml.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.Pages
{
    using System;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Input;
    using Microsoft.UI.Xaml.Media;
    using Microsoft.UI.Xaml.Navigation;
    using SteamHub.ViewModels;
    using SteamHub.ApiContract.Services;
    using SteamHub.ApiContract.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public sealed partial class FeaturesPage : Page
    {
        private readonly FeaturesViewModel featuresViewModel;
        private int userIdentifier;
        private bool showingMyFeatures = false;
        private List<FeatureDisplay> allFrames;
        private List<FeatureDisplay> allEmojis;
        private List<FeatureDisplay> allBackgrounds;
        private List<FeatureDisplay> allPets;
        private List<FeatureDisplay> allHats;

        public FeaturesPage(IFeaturesService featuresService, IUserService userService)
        {
            this.InitializeComponent();
            this.featuresViewModel = new FeaturesViewModel(
                featuresService,
                userService);
            this.DataContext = this.featuresViewModel;
            this.Loaded += this.FeaturesPage_Loaded;
            SetUserIdentifierAsync(userService);
        }

        private async void SetUserIdentifierAsync(IUserService userService)
        {
            var user = await userService.GetCurrentUserAsync();
            if (user != null)
                userIdentifier = user.UserId;
        }

        private void FeaturesPage_Loaded(object sender, RoutedEventArgs routedEventArguments)
        {
            this.featuresViewModel.Initialize(this.XamlRoot);
        }

        private async void Grid_Tapped(object sender, TappedRoutedEventArgs tappedRoutedEventArguments)
        {
            if (sender is FrameworkElement element && element.DataContext is FeatureDisplay feature)
            {
                var user = await featuresViewModel.GetCurrentUserAsync();
                if (user == null)
                {
                    featuresViewModel.StatusMessage = "No user is currently logged in.";
                    featuresViewModel.StatusColor = new Microsoft.UI.Xaml.Media.SolidColorBrush(Microsoft.UI.Colors.Red);
                    return;
                }
                var previewData = await featuresViewModel.GetFeaturePreviewDataAsync(user.UserId, feature.FeatureId);

                var dialog = new ContentDialog
                {
                    XamlRoot = this.XamlRoot,
                    Title = "Preview",
                    Content = new TextBlock { Text = "Test dialog" }, // Replace with AdaptiveProfileControl if desired
                    PrimaryButtonText = "Close"
                };
                await dialog.ShowAsync();
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArguments)
        {
            var image = sender as Image;
            if (image != null)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load image. Error: {exceptionRoutedEventArguments.ErrorMessage}");
            }
        }

        private void BackToProfileButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            // Get the current user ID from the service
            int userId = userIdentifier;

            // If we can navigate back (in case we came from Profile)
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                // Navigate directly to ProfilePage with the user ID
               this.Frame.Navigate(typeof(ProfilePage), userId);
            }
        }

        private void MyFeaturesButton_Click(object sender, RoutedEventArgs e)
        {
            if (!showingMyFeatures)
            {
                // Save all features
                allFrames = featuresViewModel.Frames.ToList();
                allEmojis = featuresViewModel.Emojis.ToList();
                allBackgrounds = featuresViewModel.Backgrounds.ToList();
                allPets = featuresViewModel.Pets.ToList();
                allHats = featuresViewModel.Hats.ToList();

                // Filter to only purchased
                featuresViewModel.Frames.Clear();
                foreach (var f in allFrames.Where(f => f.IsPurchased)) featuresViewModel.Frames.Add(f);
                featuresViewModel.Emojis.Clear();
                foreach (var f in allEmojis.Where(f => f.IsPurchased)) featuresViewModel.Emojis.Add(f);
                featuresViewModel.Backgrounds.Clear();
                foreach (var f in allBackgrounds.Where(f => f.IsPurchased)) featuresViewModel.Backgrounds.Add(f);
                featuresViewModel.Pets.Clear();
                foreach (var f in allPets.Where(f => f.IsPurchased)) featuresViewModel.Pets.Add(f);
                featuresViewModel.Hats.Clear();
                foreach (var f in allHats.Where(f => f.IsPurchased)) featuresViewModel.Hats.Add(f);
                showingMyFeatures = true;
            }
            else
            {
                // Restore all features
                featuresViewModel.Frames.Clear();
                foreach (var f in allFrames) featuresViewModel.Frames.Add(f);
                featuresViewModel.Emojis.Clear();
                foreach (var f in allEmojis) featuresViewModel.Emojis.Add(f);
                featuresViewModel.Backgrounds.Clear();
                foreach (var f in allBackgrounds) featuresViewModel.Backgrounds.Add(f);
                featuresViewModel.Pets.Clear();
                foreach (var f in allPets) featuresViewModel.Pets.Add(f);
                featuresViewModel.Hats.Clear();
                foreach (var f in allHats) featuresViewModel.Hats.Add(f);
                showingMyFeatures = false;
            }
        }

        // Add this method to allow explicit reloading of features with error handling
        public async void ReloadFeaturesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await featuresViewModel.LoadFeaturesAsyncPublic();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                // Optionally show a dialog or status message
                if (featuresViewModel != null)
                {
                    featuresViewModel.StatusMessage = $"Error: {ex.Message}";
                }
            }
        }
    }
} 
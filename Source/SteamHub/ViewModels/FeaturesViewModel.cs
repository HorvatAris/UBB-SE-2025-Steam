// <copyright file="FeaturesViewModel.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace SteamHub.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Threading.Tasks;
    using Microsoft.UI;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;
    using SteamHub.ApiContract.Models;
    using SteamHub.ApiContract.Services;
    using SteamHub.ApiContract.Services.Interfaces;

    public class FeaturesViewModel : INotifyPropertyChanged
    {
        private readonly IFeaturesService featuresService;
        private readonly IUserService userService;
        private XamlRoot xamlRoot;
        private string statusMessage = string.Empty;
        private const string PathStart = "ms-appx:///";
        private SolidColorBrush statusColor = new(Colors.Black);
        private FeatureDisplay selectedFeature;
        private XamlRoot featuresXamlRoot;

        public FeaturesViewModel(IFeaturesService featuresService, IUserService userService)
        {
            this.featuresService = featuresService;
            this.userService = userService;
            _ = this.LoadFeaturesAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string StatusMessage
        {
            get => this.statusMessage;
            set
            {
                if (this.statusMessage != value)
                {
                    this.statusMessage = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public SolidColorBrush StatusColor
        {
            get => this.statusColor;
            set
            {
                if (this.statusColor != value)
                {
                    this.statusColor = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public FeatureDisplay SelectedFeature
        {
            get => this.selectedFeature;
            set
            {
                if (this.selectedFeature != value)
                {
                    this.selectedFeature = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<FeatureDisplay> Frames { get; } = new();
        public ObservableCollection<FeatureDisplay> Emojis { get; } = new();
        public ObservableCollection<FeatureDisplay> Backgrounds { get; } = new();
        public ObservableCollection<FeatureDisplay> Pets { get; } = new();
        public ObservableCollection<FeatureDisplay> Hats { get; } = new();

        public void SetXamlRoot(XamlRoot xamlRoot)
        {
            this.xamlRoot = xamlRoot;
        }

        public void Initialize(XamlRoot xamlRoot)
        {
            this.featuresXamlRoot = xamlRoot;
        }

        private async Task LoadFeaturesAsync()
        {
            try
            {
                const string frameString = "frame";
                const string emojiString = "emoji";
                const string backgroundString = "background";
                const string petString = "pet";
                const string hatString = "hat";

                var currentUser = this.userService.GetCurrentUser();
                var features = await this.featuresService.GetFeaturesByCategoriesAsync(currentUser.UserId);

                await this.UpdateCollectionAsync(this.Frames, features.GetValueOrDefault(frameString, new()));
                await this.UpdateCollectionAsync(this.Emojis, features.GetValueOrDefault(emojiString, new()));
                await this.UpdateCollectionAsync(this.Backgrounds, features.GetValueOrDefault(backgroundString, new()));
                await this.UpdateCollectionAsync(this.Pets, features.GetValueOrDefault(petString, new()));
                await this.UpdateCollectionAsync(this.Hats, features.GetValueOrDefault(hatString, new()));

                this.StatusMessage = string.Empty;
            }
            catch (Exception ex)
            {
                this.StatusMessage = $"Error: {ex.Message}";
                this.StatusColor = new SolidColorBrush(Colors.Red);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private async Task UpdateCollectionAsync(ObservableCollection<FeatureDisplay> collection, List<Feature> features)
        {
            collection.Clear();
            var currentUser = this.userService.GetCurrentUser();
            foreach (var feature in features)
            {
                bool isPurchased = await this.featuresService.IsFeaturePurchasedAsync(currentUser.UserId, feature.FeatureId);
                collection.Add(new FeatureDisplay(feature, isPurchased));
            }
        }

        public async Task ShowOptionsAsync(FeatureDisplay feature)
        {
            try
            {
                if (feature == null || this.xamlRoot == null)
                {
                    return;
                }

                this.SelectedFeature = feature;
                var currentUser = this.userService.GetCurrentUser();
                var dialog = new ContentDialog
                {
                    XamlRoot = this.xamlRoot,
                    Title = feature.Name
                };

                var buttons = new StackPanel { Spacing = 10 };

                if (feature.IsPurchased)
                {
                    if (feature.Equipped)
                    {
                        buttons.Children.Add(new Button
                        {
                            Content = "Unequip",
                            Command = new RelayCommand<object>(async _ => await this.SafeUnequipFeatureAsync(currentUser.UserId, feature))
                        });
                    }
                    else
                    {
                        buttons.Children.Add(new Button
                        {
                            Content = "Equip",
                            Command = new RelayCommand<object>(async _ => await this.SafeEquipFeatureAsync(feature.FeatureId))
                        });
                    }
                }
                else
                {
                    buttons.Children.Add(new Button
                    {
                        Content = "Purchase",
                        Command = new RelayCommand<object>(async _ => await this.SafePurchaseFeatureAsync(currentUser.UserId, feature))
                    });
                }

                dialog.Content = buttons;
                await dialog.ShowAsync();
            }
            catch (Exception ex)
            {
                this.StatusMessage = $"Error: {ex.Message}";
                this.StatusColor = new SolidColorBrush(Colors.Red);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private async Task SafeEquipFeatureAsync(int featureId)
        {
            try { await this.EquipFeatureAsync(featureId); }
            catch (Exception ex)
            {
                this.StatusMessage = $"Error: {ex.Message}";
                this.StatusColor = new SolidColorBrush(Colors.Red);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        private async Task SafeUnequipFeatureAsync(int userId, FeatureDisplay feature)
        {
            try { await this.UnequipFeatureAsync(userId, feature); }
            catch (Exception ex)
            {
                this.StatusMessage = $"Error: {ex.Message}";
                this.StatusColor = new SolidColorBrush(Colors.Red);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }
        private async Task SafePurchaseFeatureAsync(int userId, FeatureDisplay feature)
        {
            try { await this.PurchaseFeatureAsync(userId, feature); }
            catch (Exception ex)
            {
                this.StatusMessage = $"Error: {ex.Message}";
                this.StatusColor = new SolidColorBrush(Colors.Red);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        public static event EventHandler<int> FeatureEquipStatusChanged;

        public async Task<bool> EquipFeatureAsync(int featureId)
        {
            try
            {
                bool success = await this.featuresService.EquipFeatureAsync(
                    this.userService.GetCurrentUser().UserId,
                    featureId);

                if (success)
                {
                    FeatureEquipStatusChanged?.Invoke(this, this.userService.GetCurrentUser().UserId);

                    this.StatusMessage = "Feature equipped successfully";
                    this.StatusColor = new SolidColorBrush(Colors.Green);

                    await this.LoadFeaturesAsync();
                }
                else
                {
                    this.StatusMessage = "Failed to equip feature";
                    this.StatusColor = new SolidColorBrush(Colors.Red);
                }

                return success;
            }
            catch (Exception exception)
            {
                this.StatusMessage = $"Error: {exception.Message}";
                this.StatusColor = new SolidColorBrush(Colors.Red);
                System.Diagnostics.Debug.WriteLine(exception.ToString());
                return false;
            }
        }

        public async Task<bool> UnequipFeatureAsync(int userId, FeatureDisplay feature)
        {
            try
            {
                bool success = await this.featuresService.UnequipFeatureAsync(userId, feature.FeatureId);
                this.StatusMessage = success ? "Feature unequipped successfully" : "Failed to unequip feature";
                this.StatusColor = new SolidColorBrush(success ? Colors.Green : Colors.Red);
                if (success)
                {
                    await this.LoadFeaturesAsync();
                }
                return success;
            }
            catch (Exception ex)
            {
                this.StatusMessage = $"Error: {ex.Message}";
                this.StatusColor = new SolidColorBrush(Colors.Red);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return false;
            }
        }

        public async void ShowPreview(FeatureDisplay feature)
        {
            await this.ShowPreviewDialog(feature);
        }

        private async Task PurchaseFeatureAsync(int userId, FeatureDisplay feature)
        {
            try
            {
                bool success = await this.featuresService.AddUserFeatureAsync(userId, feature.FeatureId);
                this.StatusMessage = success ? "Feature purchased successfully" : "Failed to purchase feature";
                this.StatusColor = new SolidColorBrush(success ? Colors.Green : Colors.Red);

                if (success)
                {
                    await this.LoadFeaturesAsync();
                }
            }
            catch (Exception ex)
            {
                this.StatusMessage = $"Error: {ex.Message}";
                this.StatusColor = new SolidColorBrush(Colors.Red);
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        private async Task ShowPreviewDialog(FeatureDisplay featureDisplay)
        {
            var user = this.userService.GetCurrentUser();
            var previewData = await this.featuresService.GetFeaturePreviewDataAsync(user.UserId, featureDisplay.FeatureId);

            //var profileControl = new AdaptiveProfileControl();

            string profilePicturePath = previewData.profilePicturePath;
            string bioText = previewData.bioText;
            var userFeatures = previewData.equippedFeatures;

            string hatPath = null;
            string petPath = null;
            string emojiPath = null;
            string framePath = null;
            string backgroundPath = null;

            foreach (var feature in userFeatures)
            {
                switch (feature.Type.ToLower())
                {
                    case "hat":
                        hatPath = feature.Source;
                        break;
                    case "pet":
                        petPath = feature.Source;
                        break;
                    case "emoji":
                        emojiPath = feature.Source;
                        break;
                    case "frame":
                        framePath = feature.Source;
                        break;
                    case "background":
                        backgroundPath = feature.Source;
                        break;
                }
            }

            /* profileControl.UpdateProfilePreview(
                profilePicturePath,
                bioText,
                hatPath,
                petPath,
                emojiPath,
                framePath,
                backgroundPath);

            var dialog = new ContentDialog
            {
                XamlRoot = this.featuresXamlRoot,
                Title = "Preview",
                Content = profileControl,
                PrimaryButtonText = "Close"
            };*/

            //await dialog.ShowAsync();
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FeatureDisplay : INotifyPropertyChanged
    {
        private readonly Feature feature;
        private readonly bool isPurchased;
        private bool isEquipped;

        public event PropertyChangedEventHandler PropertyChanged;

        public FeatureDisplay(Feature feature, bool isPurchased)
        {
            this.feature = feature;
            this.isPurchased = isPurchased;
        }

        public int FeatureId => this.feature.FeatureId;
        public string Name => this.feature.Name;
        public string Description => this.feature.Description;
        public string Type => this.feature.Type;
        public int Value => this.feature.Value;
        public bool IsPurchased => this.isPurchased;

        public bool Equipped
        {
            get => this.isEquipped;
            set
            {
                if (this.isEquipped != value)
                {
                    this.isEquipped = value;
                    this.OnPropertyChanged();
                }
            }
        }

        public string Source => this.feature.Source;

        public SolidColorBrush BorderBrush => this.IsPurchased ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Gray);
        public double Opacity => this.IsPurchased ? 1.0 : 0.5;
        public Visibility LockIconVisibility => this.IsPurchased ? Visibility.Collapsed : Visibility.Visible;
        public string DisplayValue => $"{this.Value} points";

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
} 
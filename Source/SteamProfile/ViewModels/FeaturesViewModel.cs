using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using SteamProfile.Views;

namespace SteamProfile.ViewModels
{
    public partial class FeaturesViewModel : ObservableObject
    {
        private readonly IFeaturesService featuresService;
        private readonly IUserService userService;
        private XamlRoot xamlRoot;
        private string statusMessage = string.Empty;
        private const string PathStart = "ms-appx:///";
        private readonly IUserProfilesRepository userProfilesRepository;
        private SolidColorBrush statusColor = new(Colors.Black);
        private FeatureDisplay selectedFeature;
        private XamlRoot featuresXamlRoot;

        public string StatusMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        public SolidColorBrush StatusColor
        {
            get => statusColor;
            set => SetProperty(ref statusColor, value);
        }

        public FeatureDisplay SelectedFeature
        {
            get => selectedFeature;
            set => SetProperty(ref selectedFeature, value);
        }

        public ObservableCollection<FeatureDisplay> Frames { get; } = new();
        public ObservableCollection<FeatureDisplay> Emojis { get; } = new();
        public ObservableCollection<FeatureDisplay> Backgrounds { get; } = new();
        public ObservableCollection<FeatureDisplay> Pets { get; } = new();
        public ObservableCollection<FeatureDisplay> Hats { get; } = new();

        public FeaturesViewModel(IFeaturesService featuresService, IUserService userService)
        {
            this.featuresService = featuresService;
            this.userService = userService;
            LoadFeatures();
        }

        public void SetXamlRoot(XamlRoot xamlRoot)
        {
            this.xamlRoot = xamlRoot;
        }
        public void Initialize(XamlRoot xamlRoot)
        {
            featuresXamlRoot = xamlRoot;
        }
        private void LoadFeatures()
        {
            try
            {
                const string frameString = "frame";
                const string emojiString = "emoji";
                const string backgroundString = "background";
                const string petString = "pet";
                const string hatString = "hat";

                var features = featuresService.GetFeaturesByCategories();

                UpdateCollection(Frames, features.GetValueOrDefault(frameString, new()));
                UpdateCollection(Emojis, features.GetValueOrDefault(emojiString, new()));
                UpdateCollection(Backgrounds, features.GetValueOrDefault(backgroundString, new()));
                UpdateCollection(Pets, features.GetValueOrDefault(petString, new()));
                UpdateCollection(Hats, features.GetValueOrDefault(hatString, new()));

                StatusMessage = string.Empty;
            }
            catch (Exception exception)
            {
                StatusMessage = "Failed to load features. Please try again later.";
                StatusColor = new SolidColorBrush(Colors.Red);
            }
        }

        private void UpdateCollection(ObservableCollection<FeatureDisplay> collection, List<Feature> features)
        {
            collection.Clear();
            var currentUser = userService.GetCurrentUser();
            foreach (var feature in features)
            {
                bool isPurchased = featuresService.IsFeaturePurchased(currentUser.UserId, feature.FeatureId);
                collection.Add(new FeatureDisplay(feature, isPurchased));
            }
        }

        [RelayCommand]
        private async Task ShowOptionsAsync(FeatureDisplay feature)
        {
            if (feature == null || xamlRoot == null)
            {
                return;
            }

            SelectedFeature = feature;
            var currentUser = userService.GetCurrentUser();
            var dialog = new ContentDialog
            {
                XamlRoot = xamlRoot,
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
                        Command = new RelayCommand(() => UnequipFeature(currentUser.UserId, feature))
                    });
                }
                else
                {
                    buttons.Children.Add(new Button
                    {
                        Content = "Equip",
                        Command = new RelayCommand(() => EquipFeature(feature.FeatureId))
                    });
                }
            }
            else
            {
                buttons.Children.Add(new Button
                {
                    Content = "Purchase",
                    Command = new RelayCommand(() => PurchaseFeature(currentUser.UserId, feature))
                });
            }

            dialog.Content = buttons;
            await dialog.ShowAsync();
        }

        public static event EventHandler<int> FeatureEquipStatusChanged;
        public bool EquipFeature(int featureId)
        {
            try
            {
                bool success = featuresService.EquipFeature(
                    userService.GetCurrentUser().UserId,
                    featureId);

                if (success)
                {
                    FeatureEquipStatusChanged?.Invoke(this, userService.GetCurrentUser().UserId);

                    StatusMessage = "Feature equipped successfully";
                    StatusColor = new SolidColorBrush(Colors.Green);

                    LoadFeatures();
                }
                else
                {
                    StatusMessage = "Failed to equip feature";
                    StatusColor = new SolidColorBrush(Colors.Red);
                }

                return success;
            }
            catch (Exception exception)
            {
                StatusMessage = $"Error: {exception.Message}";
                StatusColor = new SolidColorBrush(Colors.Red);
                return false;
            }
        }

        public bool UnequipFeature(int userId, FeatureDisplay feature)
        {
            var result = featuresService.UnequipFeature(userId, feature.FeatureId);
            StatusMessage = result.Item2;
            StatusColor = new SolidColorBrush(result.Item1 ? Colors.Green : Colors.Red);
            if (result.Item1)
            {
                LoadFeatures();
            }
            return result.Item1;
        }

        public async void ShowPreview(FeatureDisplay feature)
        {
            await ShowPreviewDialog(feature);
        }

        private void PurchaseFeature(int userId, FeatureDisplay feature)
        {
            var result = featuresService.PurchaseFeature(userId, feature.FeatureId);

            StatusMessage = result.message;
            StatusColor = new SolidColorBrush(result.success ? Colors.Green : Colors.Red);

            if (result.success)
            {
                LoadFeatures();
            }
        }

        private async Task ShowPreviewDialog(FeatureDisplay featureDisplay)
        {
            // Get current user
            var user = userService.GetCurrentUser();

            // Use service to get preview data
            var previewData = featuresService.GetFeaturePreviewData(user.UserId, featureDisplay.FeatureId);

            var profileControl = new AdaptiveProfileControl();

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
                if (!feature.Equipped)
                {
                    continue;
                }

                string path = feature.Source;
                if (!path.StartsWith(PathStart))
                {
                    path = $"ms-appx:///{path}";
                }

                switch (feature.Type.ToLower())
                {
                    case "hat": hatPath = path; break;
                    case "pet": petPath = path; break;
                    case "emoji": emojiPath = path; break;
                    case "frame": framePath = path; break;
                    case "background": backgroundPath = path; break;
                }
            }

            string previewPath = featureDisplay.Source;
            switch (featureDisplay.Type.ToLower())
            {
                case "hat": hatPath = previewPath; break;
                case "pet": petPath = previewPath; break;
                case "emoji": emojiPath = previewPath; break;
                case "frame": framePath = previewPath; break;
                case "background": backgroundPath = previewPath; break;
            }

            profileControl.UpdateProfile(
                user.Username,
                bioText,
                profilePicturePath,
                hatPath,
                petPath,
                emojiPath,
                framePath,
                backgroundPath);

            profileControl.Width = 350;
            profileControl.Height = 500;

            var previewDialog = new ContentDialog
            {
                XamlRoot = featuresXamlRoot,
                Title = "Profile Preview",
                Content = profileControl,
                CloseButtonText = "Close"
            };

            await previewDialog.ShowAsync();
        }
    }

    public class FeatureDisplay : ObservableObject
    {
        private readonly Feature feature;
        private readonly bool isPurchased;
        private bool isEquipped;

        public FeatureDisplay(Feature feature, bool isPurchased)
        {
            this.feature = feature;
            this.isPurchased = isPurchased;
            this.isEquipped = feature.Equipped;
        }

        public int FeatureId => feature.FeatureId;
        public string Name => feature.Name;
        public string Description => feature.Description;
        public string Type => feature.Type;
        public int Value => feature.Value;
        public bool IsPurchased => isPurchased;
        public bool Equipped
        {
            get => isEquipped;
            set => SetProperty(ref isEquipped, value);
        }
        public string Source
        {
            get
            {
                if (feature == null)
                {
                    return string.Empty;
                }

                return $"ms-appx:///{feature.Source}";
            }
        }
    }
}

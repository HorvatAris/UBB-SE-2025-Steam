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
    using SteamHub.ApiContract.Models.User;
    using SteamHub.ApiContract.Services;
    using SteamHub.ApiContract.Services.Interfaces;
    using SteamHub.Pages;
    using System.Windows.Input;
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;

    public partial class FeaturesViewModel : BaseViewModel
    {
        private readonly IFeaturesService featuresService;
        private readonly IUserService _userService;
        private Dictionary<string, List<Feature>> _featuresByCategories;
        private bool _isLoading;
        private string _errorMessage;
        private User _currentUser;
        private bool _isShowingMyFeatures;

        [ObservableProperty]
        private ObservableCollection<FeatureDisplay> allFeatures = new();

        [ObservableProperty]
        private ObservableCollection<FeatureDisplay> userFeatures = new();

        [ObservableProperty]
        private ObservableCollection<FeatureDisplay> equippedFeatures = new();

        [ObservableProperty]
        private FeatureDisplay selectedFeature;

        [ObservableProperty]
        private string previewBioText;

        [ObservableProperty]
        private ObservableCollection<Feature> previewEquippedFeatures;

        public Dictionary<string, List<FeatureDisplay>> FeaturesByCategories { get; private set; } = new();

        public bool IsShowingMyFeatures
        {
            get => _isShowingMyFeatures;
            set => SetProperty(ref _isShowingMyFeatures, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public int FeaturesCount => AllFeatures?.Count ?? 0;

        public string FeaturesCountLabel => $"Features count: {FeaturesCount}";
        public Frame frame;

        public FeaturesViewModel(IFeaturesService featuresService, IUserService userService, User currentUser, Frame frame)
            : base(userService, currentUser)
        {
            this.featuresService = featuresService;
            this._userService = userService;
            this._currentUser = currentUser;
            this.previewEquippedFeatures = new ObservableCollection<Feature>();
            this.frame = frame;
            // Hardcoded features for testing
            AllFeatures = new ObservableCollection<FeatureDisplay>
            {
                new FeatureDisplay { Feature = new Feature { FeatureId = 1, Name = "Black Hat", Value = 2000, Description = "An elegant hat", Type = "hat", Source = "Assets/Features/Hats/black-hat.png", Equipped = false } },
                new FeatureDisplay { Feature = new Feature { FeatureId = 2, Name = "Pufu", Value = 10, Description = "Cute doggo", Type = "pet", Source = "Assets/Features/Pets/dog.png", Equipped = false } },
                new FeatureDisplay { Feature = new Feature { FeatureId = 3, Name = "Kitty", Value = 8, Description = "Cute cat", Type = "pet", Source = "Assets/Features/Pets/cat.png", Equipped = false } },
                new FeatureDisplay { Feature = new Feature { FeatureId = 4, Name = "Frame", Value = 5, Description = "Violet frame", Type = "frame", Source = "Assets/Features/Frames/frame1.png", Equipped = false } },
                new FeatureDisplay { Feature = new Feature { FeatureId = 5, Name = "Love Emoji", Value = 7, Description = "lalal", Type = "emoji", Source = "Assets/Features/Emojis/love.png", Equipped = false } },
                new FeatureDisplay { Feature = new Feature { FeatureId = 6, Name = "Violet Background", Value = 7, Description = "Violet Background", Type = "background", Source = "Assets/Features/Backgrounds/violet.jpg", Equipped = false } }
            };
        }

        [RelayCommand]
        public async Task LoadFeaturesAsync()
        {
            if (featuresService == null || _userService == null || _currentUser == null)
            {
                System.Diagnostics.Debug.WriteLine("Skipping service calls: services or user are null.");
                return;
            }
            System.Diagnostics.Debug.WriteLine("LoadFeaturesAsync called");
            try
            {
                _isLoading = true;
                ErrorMessage = string.Empty;

                var features = await featuresService.GetAllFeaturesAsync(_currentUser.UserId);
                System.Diagnostics.Debug.WriteLine($"Features count: {features.Count}");
                foreach (var f in features)
                    System.Diagnostics.Debug.WriteLine($"Feature: {f.Name}");
                var userFeatures = await featuresService.GetUserFeaturesAsync(_currentUser.UserId);
                var equippedFeatures = await featuresService.GetEquippedFeaturesAsync(_currentUser.UserId);

                // Group features by category
                var groupedFeatures = features.GroupBy(f => f.Type)
                                            .ToDictionary(g => g.Key, g => g.ToList());

                // Convert to FeatureDisplay objects and populate collections
                foreach (var group in groupedFeatures)
                {
                    var featureDisplays = group.Value.Select(f => new FeatureDisplay
                    {
                        Feature = f,
                        IsPurchased = userFeatures.Any(uf => uf.FeatureId == f.FeatureId),
                        IsEquipped = equippedFeatures.Any(ef => ef.FeatureId == f.FeatureId)
                    }).ToList();

                    FeaturesByCategories[group.Key] = featureDisplays;
                }

                // Update other collections
                AllFeatures = new ObservableCollection<FeatureDisplay>(
                    features.Select(f => new FeatureDisplay
                    {
                        Feature = f
                    })
                );
                OnPropertyChanged(nameof(FeaturesCount));
                OnPropertyChanged(nameof(FeaturesCountLabel));
                System.Diagnostics.Debug.WriteLine($"AllFeatures count: {AllFeatures.Count}");

                UserFeatures = new ObservableCollection<FeatureDisplay>(
                    userFeatures.Select(uf => new FeatureDisplay
                    {
                        Feature = features.First(f => f.FeatureId == uf.FeatureId),
                        IsPurchased = true,
                        IsEquipped = equippedFeatures.Any(ef => ef.FeatureId == uf.FeatureId)
                    })
                );

                EquippedFeatures = new ObservableCollection<FeatureDisplay>(
                    equippedFeatures.Select(ef => new FeatureDisplay
                    {
                        Feature = features.First(f => f.FeatureId == ef.FeatureId),
                        IsPurchased = true,
                        IsEquipped = true
                    })
                );
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to load features. Please try again later.";
                System.Diagnostics.Debug.WriteLine($"Error loading features: {ex}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        [RelayCommand]
        public void ToggleShowMyFeatures()
        {
            IsShowingMyFeatures = !IsShowingMyFeatures;
        }

        [RelayCommand]
        public async Task PurchaseFeatureAsync(FeatureDisplay feature)
        {
            try
            {
                await featuresService.AddUserFeatureAsync(_currentUser.UserId, feature.Feature.FeatureId);
                await LoadFeaturesAsync(); // Reload features after purchase
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to purchase feature. Please try again later.";
                System.Diagnostics.Debug.WriteLine($"Error purchasing feature: {ex}");
            }
        }

        [RelayCommand]
        public async Task EquipFeatureAsync(FeatureDisplay feature)
        {
            try
            {
                await featuresService.EquipFeatureAsync(_currentUser.UserId, feature.Feature.FeatureId);
                await LoadFeaturesAsync(); // Reload features after equipping
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to equip feature. Please try again later.";
                System.Diagnostics.Debug.WriteLine($"Error equipping feature: {ex}");
            }
        }

        [RelayCommand]
        public async Task UnequipFeatureAsync(FeatureDisplay feature)
        {
            try
            {
                await featuresService.UnequipFeatureAsync(_currentUser.UserId, feature.Feature.FeatureId);
                await LoadFeaturesAsync(); // Reload features after unequipping
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to unequip feature. Please try again later.";
                System.Diagnostics.Debug.WriteLine($"Error unequipping feature: {ex}");
            }
        }
        /*
        [RelayCommand]
        public async Task PreviewFeatureAsync(FeatureDisplay feature)
        {
            try
            {
                SelectedFeature = feature;
                await UpdatePreviewAsync();
                
                // Navigate to preview page
                if (frame != null)
                {
                    frame.Content = new FeaturePreviewPage(userService,featuresService, frame, feature);
                }
                else
                {
                    ErrorMessage = "Navigation frame not found.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to preview feature. Please try again later.";
                System.Diagnostics.Debug.WriteLine($"Error previewing feature: {ex}");
            }
        }*/

        public async Task UpdatePreviewAsync()
        {
            try
            {
                if (SelectedFeature == null) return;

                // Get current bio
                var user = await _userService.GetUserByIdentifierAsync(_currentUser.UserId);
                PreviewBioText = user.Bio;

                // Get currently equipped features
                var equippedFeatures = await featuresService.GetEquippedFeaturesAsync(_currentUser.UserId);
                PreviewEquippedFeatures.Clear();
                foreach (var feature in equippedFeatures)
                {
                    PreviewEquippedFeatures.Add(feature);
                }

                // Add the preview feature if not already equipped
                if (!PreviewEquippedFeatures.Any(f => f.FeatureId == SelectedFeature.Feature.FeatureId))
                {
                    PreviewEquippedFeatures.Add(SelectedFeature.Feature);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Failed to update preview. Please try again later.";
                System.Diagnostics.Debug.WriteLine($"Error updating preview: {ex}");
            }
        }

        [RelayCommand]
        public void PreviewFeature(FeatureDisplay featureDisplay)
        {
            if (frame != null && featureDisplay != null)
            {
                frame.Content = new SteamHub.Pages.FeaturePreviewPage(this._userService, this.featuresService, frame, featureDisplay, _currentUser);
            }
        }
    }

    public class FeatureDisplay : ObservableObject
    {
        private Feature _feature;
        private bool _isPurchased;
        private bool _isEquipped;

        public Feature Feature
        {
            get => _feature;
            set => SetProperty(ref _feature, value);
        }

        public bool IsPurchased
        {
            get => _isPurchased;
            set => SetProperty(ref _isPurchased, value);
        }

        public bool IsEquipped
        {
            get => _isEquipped;
            set => SetProperty(ref _isEquipped, value);
        }

        public string Name => Feature?.Name ?? string.Empty;
        public string Value => Feature?.Value.ToString() ?? string.Empty;
        public string Source => Feature?.Source ?? string.Empty;
        public double Opacity => IsPurchased ? 1.0 : 0.5;
    }
} 
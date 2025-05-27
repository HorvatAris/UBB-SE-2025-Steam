using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Services;
using SteamHub.ApiContract.Models;
using SteamHub.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Models.Collections;
using Collection = SteamHub.ApiContract.Models.Collections.Collection;



namespace SteamHub.ViewModels
{
    public partial class ProfileViewModel : ObservableObject
    {
        private static ProfileViewModel profileViewModelInstance;
        private User user;
        // private readonly FriendRequestViewModel friendRequestViewModel;
        private readonly IUserService userService;
        private readonly IFriendsService friendsService;
        private readonly DispatcherQueue dispatcherQueue;
        private readonly IFeaturesService featuresService;
        private readonly IAchievementsService achievementsService;

        public ProfileViewModel()
        {
            // Initialize userProfile to prevent null reference exceptions
            user = new User();

            // Get the FriendRequestViewModel from the service container
            try
            {
                // this.friendRequestViewModel = friendRequestViewModel;
            }
            catch
            {
                // Fall back to sample data if service container isn't set up yet
                // friendRequestViewModel = null;
            }
        }

        public string Username
        {
            get => user?.Username ?? string.Empty;
            set
            {
                if (user == null)
                {
                    user = new User();
                }

                if (user.Username != value)
                {
                    user.Username = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Email
        {
            get => user?.Email ?? string.Empty;
            set
            {
                if (user == null)
                {
                    user = new User();
                }

                if (user.Email != value)
                {
                    user.Email = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ProfilePhotoPath
        {
            get => user?.ProfilePicture ?? string.Empty;
            set
            {
                if (user == null)
                {
                    user = new User();
                }

                if (user.ProfilePicture != value)
                {
                    user.ProfilePicture = value;
                    OnPropertyChanged();
                }
            }
        }

        [ObservableProperty]
        private string biography = string.Empty;

        [ObservableProperty]
        private int friendCount;

        [ObservableProperty]
        private decimal moneyBalance;

        [ObservableProperty]
        private int pointsBalance;

        [ObservableProperty]
        private string profilePicture = string.Empty;

        [ObservableProperty]
        private string coverPhotography = string.Empty;

        [ObservableProperty]
        private ObservableCollection<Collection> gameCollections = new();

        [ObservableProperty]
        private bool isLoading = false;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isProfileOwner = true;

        [ObservableProperty]
        private int userIdentifier;

        [ObservableProperty]
        private bool hasGameplayAchievement;

        [ObservableProperty]
        private bool hasCollectionAchievement;

        [ObservableProperty]
        private bool hasSocialAchievement;

        [ObservableProperty]
        private bool hasMarketAchievement;

        [ObservableProperty]
        private bool hasCustomizationAchievement;

        [ObservableProperty]
        private bool hasCommunityAchievement;

        [ObservableProperty]
        private bool hasEventAchievement;

        [ObservableProperty]
        private bool hasSpecialAchievement;

        [ObservableProperty]
        private string equippedFrameSource = string.Empty;

        [ObservableProperty]
        private string equippedHatSource = string.Empty;

        [ObservableProperty]
        private string equippedPetSource = string.Empty;

        [ObservableProperty]
        private string equippedEmojiSource = string.Empty;

        [ObservableProperty]
        private string equippedBackgroundSource = string.Empty;

        [ObservableProperty]
        private bool hasEquippedFrame;

        [ObservableProperty]
        private bool hasEquippedHat;

        [ObservableProperty]
        private bool hasEquippedPet;

        [ObservableProperty]
        private bool hasEquippedEmoji;

        [ObservableProperty]
        private bool hasEquippedBackground;

        [ObservableProperty]
        private BitmapImage profilePhoto;

        private static ICollectionsService gameCollectionsService;

        [ObservableProperty]
        private bool isFriend = false;

        [ObservableProperty]
        private string friendButtonText = "Add Friend";

        [ObservableProperty]
        private string friendButtonStyle = "AccentButtonStyle";

        public static bool IsInitialized => profileViewModelInstance != null;

        [ObservableProperty]
        private AchievementWithStatus friendshipsAchievement;

        [ObservableProperty]
        private AchievementWithStatus ownedGamesAchievement;

        [ObservableProperty]
        private AchievementWithStatus soldGamesAchievement;

        [ObservableProperty]
        private AchievementWithStatus numberOfReviewsAchievement;

        [ObservableProperty]
        private bool isDeveloper;

        [ObservableProperty]
        private AchievementWithStatus numberOfReviewsReceived;
        [ObservableProperty]
        private AchievementWithStatus developerAchievement;

        [ObservableProperty]
        private AchievementWithStatus yearsOfActivity;
        [ObservableProperty]
        private AchievementWithStatus numberOfPostsGetTopAchievement;

        public static ProfileViewModel Instance
        {
            get
            {
                if (profileViewModelInstance == null)
                {
                    throw new InvalidOperationException("ProfileViewModel must be initialized with Initialize() first");
                }
                return profileViewModelInstance;
            }
        }

        public static void Initialize(
            IUserService userService,
            IFriendsService friendsService,
            DispatcherQueue dispatcherQueue,
            ICollectionsRepository gameCollectionsRepository,
            IFeaturesService featuresService,
            IAchievementsService achievementsService)
        {
            if (profileViewModelInstance != null)
            {
                throw new InvalidOperationException("ProfileViewModel is already initialized");
            }

            profileViewModelInstance = new ProfileViewModel(userService, friendsService, dispatcherQueue, gameCollectionsRepository, featuresService, achievementsService);
        }

        public ProfileViewModel(
            IUserService userService,
            IFriendsService friendsService,
            DispatcherQueue dispatcherQueue,
            ICollectionsRepository gameCollectionsRepository,
            IFeaturesService featuresService,
            IAchievementsService achievementsService)
        {
            // Initialize userProfile to prevent null reference exceptions
            user = new User();

            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.friendsService = friendsService ?? throw new ArgumentNullException(nameof(friendsService));
            this.dispatcherQueue = dispatcherQueue ?? throw new ArgumentNullException(nameof(dispatcherQueue));
            ProfileViewModel.gameCollectionsService = (ICollectionsService)(gameCollectionsService ?? throw new ArgumentNullException(nameof(gameCollectionsService)));
            this.featuresService = featuresService ?? throw new ArgumentNullException(nameof(featuresService));
            this.achievementsService = achievementsService ?? throw new ArgumentNullException(nameof(achievementsService));

            // Register for feature equipped/unequipped events
            FeaturesViewModel.FeatureEquipStatusChanged += async (sender, userId) =>
            {
                // Only refresh if it's the current user's profile being displayed
                if (userId == userIdentifier)
                {
                    await RefreshEquippedFeaturesAsync();
                }
            };
        }

        public async Task LoadProfileAsync(int user_id)
        {
            try
            {
                await dispatcherQueue.EnqueueAsync(() => IsLoading = true);
                await dispatcherQueue.EnqueueAsync(() => ErrorMessage = string.Empty);
                Debug.WriteLine($"Loading profile for user {user_id}");
                if (user_id <= 0)
                {
                    Debug.WriteLine($"Invalid user ID: {user_id}");
                    await dispatcherQueue.EnqueueAsync(() =>
                    {
                        ErrorMessage = "Invalid user ID provided.";
                        IsLoading = false;
                    });
                    return;
                }

                // Load user first, with careful error handling
                User currentUser = null;
                try
                {
                    // Instead of using Task.Run, try direct call to reduce complexity
                    currentUser = userService.GetUserByIdentifier(user_id);

                    if (currentUser == null)
                    {
                        Debug.WriteLine($"User with ID {user_id} not found");
                        await dispatcherQueue.EnqueueAsync(() =>
                        {
                            ErrorMessage = "User not found.";
                            IsLoading = false;
                        });
                        return;
                    }

                    Debug.WriteLine($"Retrieved user: {currentUser.Username}");
                }
                catch (Exception exception)
                {
                    Debug.WriteLine($"Error getting user: {exception.Message}");
                    if (exception.InnerException != null)
                    {
                        Debug.WriteLine($"Inner exceptionception: {exception.InnerException.Message}");
                    }
                    Debug.WriteLine($"Stack trace: {exception.StackTrace}");

                    await dispatcherQueue.EnqueueAsync(() =>
                    {
                        ErrorMessage = "Failed to load user data.";
                        IsLoading = false;
                    });
                    return;
                }

                // Continue with rest of the method only if we successfully got a user
                try
                {
                    var currentUserId = userService.GetCurrentUser().UserId;
                    var isFriend = await Task.Run(() => friendsService.AreUsersFriends(currentUserId, user_id));

                    // Get equipped features (safer direct call instead of Task.Run)
                    List<Feature> equippedFeatures = new List<Feature>();
                    try
                    {
                        equippedFeatures = featuresService.GetUserEquippedFeatures(currentUser.UserId);
                        Debug.WriteLine($"Retrieved {equippedFeatures.Count} equipped features");
                    }
                    catch (Exception exception)
                    {
                        Debug.WriteLine($"Error getting equipped features: {exception.Message}");

                        // Continue with empty features list
                    }

                    await dispatcherQueue.EnqueueAsync(async () =>
                    {
                        if (currentUser != null)
                        {
                            isProfileOwner = user_id == currentUserId;
                            // Basic user info from Users table
                            userIdentifier = currentUser.UserId;
                            Username = currentUser.Username ?? string.Empty;
                            Debug.WriteLine($"Current user {Username}; isProfileOwner = {isProfileOwner}");
                            IsDeveloper = currentUser.IsDeveloper;
                            // Update friend status
                            IsFriend = isFriend;
                            FriendButtonText = isFriend ? "Unfriend" : "Add Friend";
                            FriendButtonStyle = "AccentButtonStyle";

                            Debug.WriteLine($"Current user {Username} ; isProfileOwner = {isProfileOwner} ; isFriend = {IsFriend}");
                            // Profile info from UserProfiles table
                            if (user != null)
                            {
                                biography = user.Bio ?? string.Empty;
                                // Set the string property for backward compatibility
                                // Handle different types of image sources
                                if (user.ProfilePicture != null)
                                {
                                    if (user.ProfilePicture.StartsWith("http://") || user.ProfilePicture.StartsWith("https://"))
                                    {
                                        // Web URL - use as-is for BitmapImage
                                        ProfilePicture = user.ProfilePicture;
                                    }
                                    else if (user.ProfilePicture.StartsWith("ms-appx:///"))
                                    {
                                        // Already has ms-appx prefix
                                        ProfilePicture = user.ProfilePicture;
                                    }
                                    else
                                    {
                                        // Local asset - add ms-appx prefix
                                        ProfilePicture = $"ms-appx:///{user.ProfilePicture}";
                                    }
                                }
                                else
                                {
                                    ProfilePicture = "ms-appx:///Assets/default-profile.png";
                                }

                                // Load as BitmapImage
                                await LoadProfilePhotoAsync(ProfilePicture);
                            }

                            // Process equipped features
                            ProcessEquippedFeatures(equippedFeatures);

                            // Load friend count
                            try
                            {
                                FriendCount = friendsService.GetFriendshipCount(currentUser.UserId);
                            }
                            catch (Exception exception)
                            {
                                Debug.WriteLine($"Error getting friend count: {exception.Message}");
                                FriendCount = 0;
                            }

                            // Set achievement values
                            // First unlock any achievements the user has earned
                            achievementsService.UnlockAchievementForUser(currentUser.UserId);

                            // Then load achievements
                            FriendshipsAchievement = GetTopAchievement(currentUser.UserId, "Friendships");
                            OwnedGamesAchievement = GetTopAchievement(currentUser.UserId, "Owned Games");
                            SoldGamesAchievement = GetTopAchievement(currentUser.UserId, "Sold Games");
                            NumberOfReviewsAchievement = GetTopAchievement(currentUser.UserId, "Number of Reviews Given");
                            NumberOfReviewsReceived = GetTopAchievement(currentUser.UserId, "Number of Reviews Received");
                            DeveloperAchievement = GetTopAchievement(currentUser.UserId, "Developer");
                            YearsOfActivity = GetTopAchievement(currentUser.UserId, "Years of Activity");
                            NumberOfPostsGetTopAchievement = GetTopAchievement(currentUser.UserId, "Number of Posts");

                            Debug.WriteLine($"Loaded achievements for user {currentUser.UserId}:");
                            Debug.WriteLine($"Friendships: {FriendshipsAchievement?.Achievement?.AchievementName}, Unlocked: {FriendshipsAchievement?.IsUnlocked}");
                            Debug.WriteLine($"Owned Games: {OwnedGamesAchievement?.Achievement?.AchievementName}, Unlocked: {OwnedGamesAchievement?.IsUnlocked}");
                            Debug.WriteLine($"Sold Games: {SoldGamesAchievement?.Achievement?.AchievementName}, Unlocked: {SoldGamesAchievement?.IsUnlocked}");
                            Debug.WriteLine($"Reviews: {NumberOfReviewsAchievement?.Achievement?.AchievementName}, Unlocked: {NumberOfReviewsAchievement?.IsUnlocked}");

                            moneyBalance = 0;
                            pointsBalance = 0;
                            coverPhotography = "default_cover.png";

                            try
                            {
                                var lastThreeCollections = gameCollectionsService.GetLastThreeCollectionsForUser(user_id);
                                gameCollections.Clear();
                                foreach (var collection in lastThreeCollections)
                                {
                                    gameCollections.Add(collection);
                                }
                            }
                            catch (Exception exception)
                            {
                                Debug.WriteLine($"Error loading collections: {exception.Message}");
                            }
                        }

                        IsLoading = false;
                    });
                }
                catch (Exception exception)
                {
                    Debug.WriteLine($"Error in profile loading process: {exception.Message}");
                    if (exception.InnerException != null)
                    {
                        Debug.WriteLine($"Inner exception: {exception.InnerException.Message}");
                    }
                    Debug.WriteLine($"Stack trace: {exception.StackTrace}");

                    await dispatcherQueue.EnqueueAsync(() =>
                    {
                        ErrorMessage = "Failed to load profile data. Please try again later.";
                        IsLoading = false;
                    });
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Critical error in LoadProfileAsync: {exception.Message}");
                if (exception.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {exception.InnerException.Message}");
                }

                Debug.WriteLine($"Stack trace: {exception.StackTrace}");
            }
        }

        private void ProcessEquippedFeatures(List<Feature> equippedFeatures)
        {
            try
            {
                // Use a known-good image path that definitely exists in the project
                const string DEFAULT_IMAGE = "ms-appx:///Assets/default-profile.png";

                // Reset all equipped features to a valid empty image
                EquippedFrameSource = DEFAULT_IMAGE;
                EquippedHatSource = DEFAULT_IMAGE;
                EquippedPetSource = DEFAULT_IMAGE;
                EquippedEmojiSource = DEFAULT_IMAGE;
                EquippedBackgroundSource = DEFAULT_IMAGE;

                // Reset visibility flags
                HasEquippedFrame = false;
                HasEquippedHat = false;
                HasEquippedPet = false;
                HasEquippedEmoji = false;
                HasEquippedBackground = false;

                Debug.WriteLine($"Processing {equippedFeatures?.Count ?? 0} equipped features");

                // Process each equipped feature with better error handling
                if (equippedFeatures != null)
                {
                    foreach (var feature in equippedFeatures)
                    {
                        if (feature == null)
                        {
                            Debug.WriteLine("Skipping null feature");
                            continue;
                        }

                        Debug.WriteLine($"Processing feature: ID={feature.FeatureId}, Type={feature.Type}, Source={feature.Source}, Equipped={feature.Equipped}");

                        if (feature.Equipped)
                        {
                            try
                            {
                                // Use ms-appx path format for images
                                string source = feature.Source;
                                if (string.IsNullOrEmpty(source))
                                {
                                    Debug.WriteLine($"Skipping feature {feature.FeatureId} with empty source");
                                    continue;
                                }

                                if (!source.StartsWith("ms-appx:///"))
                                {
                                    source = $"ms-appx:///{source}";
                                }

                                if (string.IsNullOrEmpty(feature.Type))
                                {
                                    Debug.WriteLine($"Skipping feature {feature.FeatureId} with empty type");
                                    continue;
                                }

                                switch (feature.Type.ToLower())
                                {
                                    case "frame":
                                        EquippedFrameSource = source;
                                        HasEquippedFrame = true;
                                        Debug.WriteLine($"Set frame: {source}");
                                        break;
                                    case "hat":
                                        EquippedHatSource = source;
                                        HasEquippedHat = true;
                                        Debug.WriteLine($"Set hat: {source}");
                                        break;
                                    case "pet":
                                        EquippedPetSource = source;
                                        HasEquippedPet = true;
                                        Debug.WriteLine($"Set pet: {source}");
                                        break;
                                    case "emoji":
                                        EquippedEmojiSource = source;
                                        HasEquippedEmoji = true;
                                        Debug.WriteLine($"Set emoji: {source}");
                                        break;
                                    case "background":
                                        EquippedBackgroundSource = source;
                                        HasEquippedBackground = true;
                                        Debug.WriteLine($"Set background: {source}");
                                        break;
                                    default:
                                        Debug.WriteLine($"Unknown feature type: {feature.Type}");
                                        break;
                                }
                            }
                            catch (Exception exception)
                            {
                                Debug.WriteLine($"Error processing feature {feature.FeatureId}: {exception.Message}");
                            }
                        }
                    }
                }

                Debug.WriteLine($"Feature visibility - Frame: {HasEquippedFrame}, Hat: {HasEquippedHat}, Pet: {HasEquippedPet}, Emoji: {HasEquippedEmoji}, Background: {HasEquippedBackground}");
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error in ProcessEquippedFeatures: {exception.Message}");
                if (exception.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {exception.InnerException.Message}");
                }
                Debug.WriteLine($"Stack trace: {exception.StackTrace}");
                // In case of error, ensure we have valid image sources
                const string DEFAULT_IMAGE = "ms-appx:///Assets/default-profile.png";
                EquippedFrameSource = DEFAULT_IMAGE;
                EquippedHatSource = DEFAULT_IMAGE;
                EquippedPetSource = DEFAULT_IMAGE;
                EquippedEmojiSource = DEFAULT_IMAGE;
                EquippedBackgroundSource = DEFAULT_IMAGE;
            }
        }

        [RelayCommand]
        private async Task ToggleFriendship()
        {
            try
            {
                if (IsFriend)
                {
                    // Remove friend
                    var friendshipId = await Task.Run(() => friendsService.GetFriendshipIdentifier(userService.GetCurrentUser().UserId, userIdentifier));
                    if (friendshipId.HasValue)
                    {
                        await Task.Run(() => friendsService.RemoveFriend(friendshipId.Value));
                        IsFriend = false;
                        FriendButtonText = "Add Friend";
                        FriendCount = friendsService.GetFriendshipCount(userIdentifier);
                    }
                }
                else
                {
                    // Add friend
                    await Task.Run(() => friendsService.AddFriend(userService.GetCurrentUser().UserId, userIdentifier));
                    IsFriend = true;
                    FriendButtonText = "Unfriend";
                    FriendCount = friendsService.GetFriendshipCount(userIdentifier);
                }
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error toggling friendship: {exception.Message}");
                ErrorMessage = "Failed to update friendship status. Please try again later.";
            }
        }

        private async Task LoadProfilePhotoAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                ProfilePhoto = null;
                return;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.UriSource = new Uri(imageUrl);
                ProfilePhoto = bitmap;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to load profile image: {ex.Message}");
                ProfilePhoto = null;
            }
        }

        private AchievementWithStatus GetTopAchievement(int userId, string category)
        {
            try
            {
                // Get all achievements for this category
                var achievements = achievementsService.GetAchievementsWithStatusForUser(userId)
                    .Where(achievementWithStatus => achievementWithStatus.Achievement.AchievementType == category)
                    .ToList();

                // First try to get the highest-points unlocked achievement
                var topUnlockedAchievement = achievements
                    .Where(achievement => achievement.IsUnlocked)
                    .OrderByDescending(achievement => achievement.Achievement.Points)
                    .FirstOrDefault();

                // If we found an unlocked achievement, return it
                if (topUnlockedAchievement != null)
                {
                    Debug.WriteLine($"Found top unlocked {category} achievement: {topUnlockedAchievement.Achievement.AchievementName}");
                    return topUnlockedAchievement;
                }

                // If no unlocked achievements, get the lowest-points locked achievement
                var lowestLockedAchievement = achievements
                    .Where(achievement => !achievement.IsUnlocked)
                    .OrderBy(achievement => achievement.Achievement.Points)
                    .FirstOrDefault();

                // If we found a locked achievement, return it
                if (lowestLockedAchievement != null)
                {
                    Debug.WriteLine($"Found lowest locked {category} achievement: {lowestLockedAchievement.Achievement.AchievementName}");
                    return lowestLockedAchievement;
                }

                // If no achievements found at all, return an empty achievement
                Debug.WriteLine($"No achievements found for {category}, returning empty achievement");
                return new AchievementWithStatus
                {
                    Achievement = new Achievement
                    {
                        AchievementName = $"No {category} Achievement",
                        Description = "Complete tasks to unlock this achievement",
                        AchievementType = category,
                        Points = 0,
                        Icon = "ms-appx:///Assets/empty_achievement.png" // Use a grayscale or empty icon
                    },
                    IsUnlocked = false
                };
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error getting top achievement for {category}: {exception.Message}");
                return new AchievementWithStatus
                {
                    Achievement = new Achievement
                    {
                        AchievementName = $"No {category} Achievement",
                        Description = "Complete tasks to unlock this achievement",
                        AchievementType = category,
                        Points = 0,
                        Icon = "ms-appx:///Assets/empty_achievement.png" // Use a grayscale or empty icon
                    },
                    IsUnlocked = false
                };
            }
        }



        [RelayCommand]
        private void Configuration()
        {
            NavigationService.Instance.Navigate(typeof(Pages.ConfigurationsPage));
        }

        [RelayCommand]
        private void BackToProfile()
        {
            // Get the current user's ID from the UserService
            int currentUserId = userService.GetCurrentUser().UserId;

            // Navigate back to the Profile page with the current user ID
            NavigationService.Instance.Navigate(typeof(ProfilePage), currentUserId);
        }

        public async Task RefreshEquippedFeaturesAsync()
        {
            try
            {
                Debug.WriteLine($"Refreshing equipped features for user {userIdentifier}");

                // Get the updated equipped features
                var equippedFeatures = featuresService.GetUserEquippedFeatures(userIdentifier);

                // Process and update the UI
                await dispatcherQueue.EnqueueAsync(() =>
                {
                    ProcessEquippedFeatures(equippedFeatures);
                });

                Debug.WriteLine("Equipped features refreshed successfully");
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error refreshing equipped features: {exception.Message}");
            }
        }
    }

    public static partial class DispatcherQueueExtensions
    {
        public static Task EnqueueAsync(this DispatcherQueue dispatcher, Action action)
        {
            var taskCompletionSource = new TaskCompletionSource();
            if (!dispatcher.TryEnqueue(() =>
            {
                try
                {
                    action();
                    taskCompletionSource.SetResult();
                }
                catch (Exception exception)
                {
                    taskCompletionSource.SetException(exception);
                }
            }))
            {
                taskCompletionSource.SetException(new InvalidOperationException("Failed to enqueue task to dispatcher"));
            }

            return taskCompletionSource.Task;
        }
    }
}

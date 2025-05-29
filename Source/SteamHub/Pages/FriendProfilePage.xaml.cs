using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data.SqlClient;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using BusinessLayer.Repositories;
using SteamHub.Pages;
using Microsoft.UI;
using Microsoft.UI.Dispatching;
using SteamHub;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Pages;
using SteamHub.ViewModels;

namespace SteamHub.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FriendProfilePage : Page
    {
        public ProfileViewModel ViewModel { get; set; }
        private int userIdentifier;
        private bool isNavigatingAway = false;
        private readonly IUserService userService;
        private readonly IFriendsService friendsService;
        private readonly ICollectionsService gameCollectionsService;
        private readonly IFeaturesService featuresService;
        private readonly IAchievementsService achievementsService;

        public FriendProfilePage(IUserService userService,
            IFriendsService friendsService,
            ICollectionsService gameCollectionsService,
            IFeaturesService featuresService,
            IAchievementsService achievementsService)
        {
            try
            {
                InitializeComponent();
                Debug.WriteLine("FriendProfilePage initialized.");

                // Initialize services
                this.userService = userService ?? throw new ArgumentNullException(nameof(userService), "UserService cannot be null");
                this.friendsService = friendsService ?? throw new ArgumentNullException(nameof(friendsService), "FriendsService cannot be null");
                this.gameCollectionsService = gameCollectionsService ?? throw new ArgumentNullException(nameof(gameCollectionsService), "GameCollectionsService cannot be null");
                this.featuresService = featuresService ?? throw new ArgumentNullException(nameof(featuresService), "FeaturesService cannot be null");
                this.achievementsService = achievementsService ?? throw new ArgumentNullException(nameof(achievementsService), "AchievementsService cannot be null");

                // Only initialize the ViewModel if it hasn't been initialized yet
                if (ProfileViewModel.IsInitialized)
                {
                    Debug.WriteLine("Using existing ProfileViewModel instance.");
                    ViewModel = ProfileViewModel.Instance;
                }
                else
                {
                    // Initialize the ViewModel with the UI thread's dispatcher
                    Debug.WriteLine("DataLink instance obtained.");

                    Debug.WriteLine("FriendshipsRepository and FriendsService initialized.");

                    // Add the UserProfileRepository parameter
                    ProfileViewModel.Initialize(
                        userService, friendsService, Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread(),
                        gameCollectionsService, featuresService, achievementsService);

                    Debug.WriteLine("ProfileViewModel initialized with services.");
                    ViewModel = ProfileViewModel.Instance;
                }

                DataContext = ViewModel; // Ensure this is set correctly
                Debug.WriteLine("Profile data loading initiated.");
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error initializing FriendProfilePage: {exception.Message}");
                if (exception.InnerException != null)
                {
                    Debug.WriteLine($"Inner exception: {exception.InnerException.Message}");
                }

                // Show error dialog to user
                ShowErrorDialog("Failed to initialize profile. Please try again later.");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // Unregister from property changed events to prevent memory leaks
            if (ViewModel != null)
            {
                ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }

            Debug.WriteLine($"Navigated away from FriendProfilePage to {e.SourcePageType.Name}");

            base.OnNavigatedFrom(e);
            isNavigatingAway = true;
            Debug.WriteLine("Navigated away from FriendProfilePage");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Debug.WriteLine("Navigated to FriendProfilePage.");

            // Ensure ProfileViewModel is initialized
            if (!ProfileViewModel.IsInitialized)
            {
                Debug.WriteLine("Initializing ProfileViewModel...");

                Debug.WriteLine("FriendsService obtained.");

                // Initialize ProfileViewModel with all required services
                ProfileViewModel.Initialize(
                    userService,
                    friendsService,
                    Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread(),
                    gameCollectionsService,
                    featuresService,
                    achievementsService);
                Debug.WriteLine("ProfileViewModel initialized with services.");
            }

            // Get the ViewModel instance
            ViewModel = ProfileViewModel.Instance;
            DataContext = ViewModel;

            // Register for property changed events
            ViewModel.PropertyChanged += ViewModel_PropertyChanged;

            // If we receive a parameter, it indicates whose profile we're viewing
            if (e.Parameter != null)
            {
                // Use the parameter as the user ID
                userIdentifier = (int)e.Parameter;
                Debug.WriteLine($"Using user ID from navigation parameter: {userIdentifier}");

                // Load the profile data
                _ = ViewModel.LoadProfileAsync(userIdentifier);
            }

            // If no parameter but we're returning to the page and ViewModel has a user ID
            else if (ViewModel.UserIdentifier > 0)
            {
                // Use the user ID stored in the ViewModel
                userIdentifier = ViewModel.UserIdentifier;
                Debug.WriteLine($"Using stored user ID from ViewModel: {userIdentifier}");

                // Load the profile data
                _ = LoadAndUpdateProfile(userIdentifier);  /// VERY IMPORTANT
            }
            else
            {
                Debug.WriteLine("No user ID available - cannot load profile");
            }

            UpdateProfileControl();
        }

        private async Task LoadAndUpdateProfile(int userId)
        {
            await ViewModel.LoadProfileAsync(userId);

            // Important: Call UpdateProfileControl explicitly after loading profile
            UpdateProfileControl();
            Debug.WriteLine($"Profile control updated after loading profile. Profile picture path: {ViewModel.ProfilePicture}");
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // Update the AdaptiveProfileControl when relevant properties change
            if (e.PropertyName == nameof(ViewModel.Username) ||
                e.PropertyName == nameof(ViewModel.Biography) ||
                e.PropertyName == nameof(ViewModel.ProfilePicture) ||
                e.PropertyName == nameof(ViewModel.EquippedHatSource) ||
                e.PropertyName == nameof(ViewModel.EquippedPetSource) ||
                e.PropertyName == nameof(ViewModel.EquippedEmojiSource) ||
                e.PropertyName == nameof(ViewModel.EquippedFrameSource) ||
                e.PropertyName == nameof(ViewModel.EquippedBackgroundSource) ||
                e.PropertyName == nameof(ViewModel.HasEquippedHat) ||
                e.PropertyName == nameof(ViewModel.HasEquippedPet) ||
                e.PropertyName == nameof(ViewModel.HasEquippedEmoji) ||
                e.PropertyName == nameof(ViewModel.HasEquippedFrame) ||
                e.PropertyName == nameof(ViewModel.HasEquippedBackground))
            {
                UpdateProfileControl();
            }
        }

        private void UpdateProfileControl()
        {
            // Only update if the control exists
            if (ProfileControl == null)
            {
                return;
            }

            // Always pass the profile picture as long as it's not null or empty
            string profilePicture = !string.IsNullOrEmpty(ViewModel.ProfilePicture)
                ? ViewModel.ProfilePicture
                : "ms-appx:///Assets/default-profile.png";

            // Get paths for all equipped items, but only if they are equipped (check visibility flags)
            string hatPath = ViewModel.HasEquippedHat ? ViewModel.EquippedHatSource : null;
            string petPath = ViewModel.HasEquippedPet ? ViewModel.EquippedPetSource : null;
            string emojiPath = ViewModel.HasEquippedEmoji ? ViewModel.EquippedEmojiSource : null;
            string framePath = ViewModel.HasEquippedFrame ? ViewModel.EquippedFrameSource : null;
            string backgroundPath = ViewModel.HasEquippedBackground ? ViewModel.EquippedBackgroundSource : null;

            // Update the AdaptiveProfileControl
            ProfileControl.UpdateProfile(
                ViewModel.Username,
                string.Empty,
                profilePicture,
                hatPath,
                petPath,
                emojiPath,
                framePath,
                backgroundPath);
        }

        private async void ShowErrorDialog(string message)
        {
            try
            {
                // Create a new dialog instance each time
                ContentDialog errorDialog = new ContentDialog
                {
                    Title = "Error",
                    Content = message,
                    CloseButtonText = "OK",
                    XamlRoot = this.XamlRoot,
                };

                await errorDialog.ShowAsync();
                Debug.WriteLine("Error dialog shown.");
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error showing dialog: {exception.Message}");
            }
        }

        private async void UnfriendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ContentDialog confirmDialog = new ContentDialog
                {
                    Title = "Confirm Unfriend",
                    Content = "Are you sure you want to unfriend this user?",
                    PrimaryButtonText = "Yes",
                    CloseButtonText = "No",
                    DefaultButton = ContentDialogButton.Close,
                    XamlRoot = this.XamlRoot,
                };

                var result = await confirmDialog.ShowAsync();
                if (result == ContentDialogResult.Primary)
                {
                    // Get the friendship ID for the current user and friend
                    var currentUser = await App.UserService.GetCurrentUserAsync();
                    var friendships = await App.FriendsService.GetAllFriendshipsAsync(currentUser.UserId);
                    var friendship = friendships.FirstOrDefault(currentFriendship =>
                        (currentFriendship.UserId ==currentUser.UserId && currentFriendship.FriendId == userIdentifier) ||
                        (currentFriendship.UserId == userIdentifier && currentFriendship.FriendId == currentUser.UserId));

                    if (friendship != null)
                    {
                        await App.FriendsService.RemoveFriendAsync(friendship.FriendshipId);
                        Frame.Navigate(typeof(FriendProfilePage), currentUser.UserId);
                    }
                    else
                    {
                        ShowErrorDialog("Could not find friendship to remove.");
                    }
                }
            }
            catch (Exception exception)
            {
                ShowErrorDialog($"Error unfriending user: {exception.Message}");
            }
        }

        private void ViewCollections_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(CollectionsPage));
        }
        private void AchievementsButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AchievementsPage));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // If we have a valid user ID, refresh the profile
            if (userIdentifier > 0)
            {
                _ = ViewModel.LoadProfileAsync(userIdentifier);
            }
            // Initial update of the profile control
            UpdateProfileControl();
        }

        private async Task BackToProfileClickAsync(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(ProfilePage), (await App.UserService.GetCurrentUserAsync()).UserId);
        }
    }
}
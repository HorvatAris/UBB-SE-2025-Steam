using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels
{
    public partial class AchievementsViewModel : ObservableObject
    {
        private static AchievementsViewModel achievementsViewModelInstance;
        private readonly IAchievementsService achievementsService;
        private readonly IUserService userService;
        private User currentUser;
       
        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> allAchievements = new();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> friendshipsAchievements = new();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> ownedGamesAchievements = new();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> soldGamesAchievements = new();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> yearsOfActivityAchievements = new();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> numberOfPostsAchievements = new();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> numberOfReviewsGivenAchievements = new();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> numberOfReviewsReceivedAchievements = new();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> developerAchievements = new();

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        public AchievementsViewModel(IAchievementsService achievementsService, IUserService userService)
        {
            this.achievementsService = achievementsService ?? throw new ArgumentNullException(nameof(achievementsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
           // BackToProfileCommand = new RelayCommand(BackToProfile);
            _ = InitializeAsync();
        }

        private async Task InitializeAsync()
        {
            try
            {
                isLoading = true;
                errorMessage = string.Empty;
                currentUser = await userService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    await LoadAchievementsAsync();
                }
                else
                {
                    errorMessage = "Failed to get current user";
                    System.Diagnostics.Debug.WriteLine("Failed to get current user during initialization");
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error initializing achievements";
                System.Diagnostics.Debug.WriteLine($"Error in InitializeAsync: {ex.Message}");
            }
            finally
            {
                isLoading = false;
            }
        }

        [RelayCommand]
        public async Task LoadAchievementsAsync()
        {
            try
            {
                if (currentUser == null)
                {
                    currentUser = await userService.GetCurrentUserAsync();
                    if (currentUser == null)
                    {
                        errorMessage = "Failed to get current user";
                        return;
                    }
                }

                isLoading = true;
                errorMessage = string.Empty;
                System.Diagnostics.Debug.WriteLine("Starting LoadAchievementsAsync in ViewModel");
                var userId = currentUser.UserId;
                System.Diagnostics.Debug.WriteLine($"Current UserId: {userId}");

                // Get grouped achievements (no logic in ViewModel)
                var groupedAchievements = await achievementsService.GetGroupedAchievementsForUser(userId);
             
                try
                {
                    // Assign to ObservableCollections
                    AllAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.AllAchievements);
                    System.Diagnostics.Debug.WriteLine("AllAchievements assigned");
                    
                    FriendshipsAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.Friendships);
                    System.Diagnostics.Debug.WriteLine("FriendshipsAchievements assigned");
                    
                    OwnedGamesAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.OwnedGames);
                    System.Diagnostics.Debug.WriteLine("OwnedGamesAchievements assigned");
                    
                    SoldGamesAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.SoldGames);
                    System.Diagnostics.Debug.WriteLine("SoldGamesAchievements assigned");
                    
                    YearsOfActivityAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.YearsOfActivity);
                    System.Diagnostics.Debug.WriteLine("YearsOfActivityAchievements assigned");
                    
                    NumberOfPostsAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.NumberOfPosts);
                    System.Diagnostics.Debug.WriteLine("NumberOfPostsAchievements assigned");
                    
                    NumberOfReviewsGivenAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.NumberOfReviewsGiven);
                    System.Diagnostics.Debug.WriteLine("NumberOfReviewsGivenAchievements assigned");
                    
                    NumberOfReviewsReceivedAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.NumberOfReviewsReceived);
                    System.Diagnostics.Debug.WriteLine("NumberOfReviewsReceivedAchievements assigned");
                    
                    DeveloperAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.Developer);
                    System.Diagnostics.Debug.WriteLine("DeveloperAchievements assigned");

                    System.Diagnostics.Debug.WriteLine("Successfully assigned achievements to ObservableCollections");
                }
                catch (Exception ex)
                {
                    errorMessage = "Error assigning achievements to collections";
                    System.Diagnostics.Debug.WriteLine($"Error assigning achievements to collections: {ex}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error loading achievements";
                System.Diagnostics.Debug.WriteLine($"Error in LoadAchievementsAsync: {ex}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                isLoading = false;
            }
        }

        //public IRelayCommand BackToProfileCommand { get; private set; }

        private void BackToProfile()
        {
            if (currentUser != null)
            {
                int currentUserId = currentUser.UserId;
                //NavigationService.Instance.Navigate(typeof(ProfilePage), currentUserId);
            }
        }
    }
}

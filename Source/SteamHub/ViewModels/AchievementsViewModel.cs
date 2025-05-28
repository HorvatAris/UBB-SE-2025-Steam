using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels
{
    public partial class AchievementsViewModel : BaseViewModel
    {
        private readonly IAchievementsService achievementsService;
       
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

        public AchievementsViewModel(IAchievementsService achievementsService, IUserService userService, User currentUser)
            : base(userService, currentUser)
        {
            this.achievementsService = achievementsService ?? throw new ArgumentNullException(nameof(achievementsService));
            Debug.WriteLine($"AchievementsViewModel initialized for user: {currentUser.Username}");
            _ = LoadAchievementsAsync();
        }

        protected override void OnUserChanged()
        {
            base.OnUserChanged();
            Debug.WriteLine($"User changed in AchievementsViewModel - refreshing achievements for user: {CurrentUser.Username}");
            _ = LoadAchievementsAsync();
        }

        [RelayCommand]
        public async Task LoadAchievementsAsync()
        {
            try
            {
                isLoading = true;
                errorMessage = string.Empty;

                Debug.WriteLine($"Loading achievements for user: {CurrentUser.Username} (ID: {CurrentUser.UserId})");
                
                // Get grouped achievements
                var groupedAchievements = await achievementsService.GetGroupedAchievementsForUser(CurrentUser.UserId);
             
                try
                {
                    // Assign to ObservableCollections
                    AllAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.AllAchievements);
                    FriendshipsAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.Friendships);
                    OwnedGamesAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.OwnedGames);
                    SoldGamesAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.SoldGames);
                    YearsOfActivityAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.YearsOfActivity);
                    NumberOfPostsAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.NumberOfPosts);
                    NumberOfReviewsGivenAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.NumberOfReviewsGiven);
                    NumberOfReviewsReceivedAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.NumberOfReviewsReceived);
                    DeveloperAchievements = new ObservableCollection<AchievementWithStatus>(groupedAchievements.Developer);

                    Debug.WriteLine($"Successfully loaded achievements for user: {CurrentUser.Username}");
                }
                catch (Exception ex)
                {
                    errorMessage = "Error assigning achievements to collections";
                    Debug.WriteLine($"Error assigning achievements to collections: {ex}");
                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Error loading achievements";
                Debug.WriteLine($"Error in LoadAchievementsAsync: {ex}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                isLoading = false;
            }
        }

        [RelayCommand]
        private void BackToProfile()
        {
            if (CurrentUser != null)
            {
                Debug.WriteLine($"Navigating back to profile for user: {CurrentUser.Username}");
                //NavigationService.Instance.Navigate(typeof(ProfilePage), CurrentUser.UserId);
            }
        }
    }
}

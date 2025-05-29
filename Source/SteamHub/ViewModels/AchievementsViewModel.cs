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
    public partial class AchievementsViewModel : ObservableObject
    {
        private readonly IAchievementsService achievementsService;
        private readonly IUserService userService;

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
            _ = LoadAchievementsAsync();
        }


        [RelayCommand]
        public async Task LoadAchievementsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var currentUser = await this.userService.GetCurrentUserAsync();

                Debug.WriteLine($"Loading achievements for user: {currentUser.Username} (ID: {currentUser.UserId})");


                // Get grouped achievements
                var groupedAchievements = await achievementsService.GetGroupedAchievementsForUser(currentUser.UserId);
             
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

                    Debug.WriteLine($"Successfully loaded achievements for user: {currentUser.Username}");
                }
                catch (Exception ex)
                {
                    ErrorMessage = "Error assigning achievements to collections";
                    Debug.WriteLine($"Error assigning achievements to collections: {ex}");
                    Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading achievements";
                Debug.WriteLine($"Error in LoadAchievementsAsync: {ex}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void BackToProfile()
        {
            //if (CurrentUser != null)
            //{
            //    Debug.WriteLine($"Navigating back to profile for user: {CurrentUser.Username}");
            //    //NavigationService.Instance.Navigate(typeof(ProfilePage), CurrentUser.UserId);
            //}
        }
    }
}

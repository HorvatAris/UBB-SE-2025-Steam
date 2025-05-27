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
        private readonly IUserDetails currentUser;
       
        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> allAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> friendshipsAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> ownedGamesAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> soldGamesAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> yearsOfActivityAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> numberOfPostsAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> numberOfReviewsGivenAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> numberOfReviewsReceivedAchievements = new ObservableCollection<AchievementWithStatus>();

        [ObservableProperty]
        private ObservableCollection<AchievementWithStatus> developerAchievements = new ObservableCollection<AchievementWithStatus>();

        //public static AchievementsViewModel Instance
        //{
        //    get
        //    {
        //        if (achievementsViewModelInstance == null)
        //        {
        //            //achievementsViewModelInstance = new AchievementsViewModel(App.AchievementsService, App.UserService);
        //            var aService = CallConvThiscall.
        //            achievementsViewModelInstance = new AchievementsViewModel();
                        
        //        }
        //        return achievementsViewModelInstance;
        //    }
        //}

        public AchievementsViewModel(IAchievementsService achievementsService, IUserService userService)
        {
            this.achievementsService = achievementsService ?? throw new ArgumentNullException(nameof(achievementsService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.currentUser = this.userService.GetCurrentUser();
            BackToProfileCommand = new RelayCommand(BackToProfile);
        }
        [RelayCommand]
        public async Task LoadAchievementsAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Starting LoadAchievementsAsync in ViewModel");
                var userId = this.currentUser.UserId;
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
                    System.Diagnostics.Debug.WriteLine($"Error assigning achievements to collections: {ex}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    throw;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in LoadAchievementsAsync: {ex}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw; // Re-throw to be handled by the page
            }
        }
        public IRelayCommand BackToProfileCommand { get; }

        private void BackToProfile()
        {
            int currentUserId = this.currentUser.UserId;
            //NavigationService.Instance.Navigate(typeof(ProfilePage), currentUserId);
        }
    }
}

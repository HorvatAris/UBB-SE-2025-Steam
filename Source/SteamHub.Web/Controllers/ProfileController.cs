using Microsoft.AspNetCore.Mvc;
using SteamHub.Web.ViewModels;
using SteamHub.ApiContract.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Diagnostics;
using SteamHub.ApiContract.Models;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService userService;
        private readonly IFriendsService friendsService;
        private readonly ICollectionsService collectionsService;
        private readonly IFeaturesService featuresService;
        private readonly IAchievementsService achievementsService;

        public ProfileController(
            IUserService userService,
            IFriendsService friendsService,
            ICollectionsService collectionsService,
            IFeaturesService featuresService,
            IAchievementsService achievementsService)
        {
            this.userService = userService;
            this.friendsService = friendsService;
            this.collectionsService = collectionsService;
            this.featuresService = featuresService;
            this.achievementsService = achievementsService;
        }

        public IActionResult Index()
        {
            string userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = userService.GetUserByIdentifierAsync(userId).Result;
            // FIX: CurrentUser is null, these are 100% Problems with web login
            // var currentUser = userService.GetCurrentUserAsync().Result;
            var currentUser = user;
            if (user == null || currentUser == null)
                return NotFound();

            var collections = collectionsService.GetLastThreeCollectionsForUser(userId).Result;

            var vm = new ProfileViewModel
            {
                UserIdentifier = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsProfileOwner = user.UserId == currentUser.UserId,
                IsDeveloper = user.IsDeveloper,
                ProfilePhotoPath = user?.ProfilePicture ?? "/images/default-profile.png",
                Biography = user?.Bio ?? "",
                FriendCount = friendsService.GetFriendshipCountAsync(userId).Result,
                GameCollections = collections,
                FriendshipsAchievement = GetTopAchievementAsync(currentUser.UserId, "Friendships").Result,
                OwnedGamesAchievement = GetTopAchievementAsync(currentUser.UserId, "Owned Games").Result,
                SoldGamesAchievement = GetTopAchievementAsync(currentUser.UserId, "Sold Games").Result,
                NumberOfReviewsAchievement = GetTopAchievementAsync(currentUser.UserId, "Number of Reviews Given").Result,
                NumberOfReviewsReceived = GetTopAchievementAsync(currentUser.UserId, "Number of Reviews Received").Result,
                DeveloperAchievement = GetTopAchievementAsync(currentUser.UserId, "Developer").Result,
                YearsOfActivity = GetTopAchievementAsync(currentUser.UserId, "Years of Activity").Result,
                NumberOfPostsGetTopAchievement = GetTopAchievementAsync(currentUser.UserId, "Number of Posts").Result
            };


            return View(vm);
        }
            private async Task<AchievementWithStatus> GetTopAchievementAsync(int userId, string category)
        {
            try
            {
                var achievements = await achievementsService.GetAchievementsWithStatusForUser(userId);
                var categoryAchievements = achievements
                    .Where(achievementWithStatus => achievementWithStatus.Achievement.AchievementType == category)
                    .ToList();

                var topUnlockedAchievement = categoryAchievements
                    .Where(achievement => achievement.IsUnlocked)
                    .OrderByDescending(achievement => achievement.Achievement.Points)
                    .FirstOrDefault();

                if (topUnlockedAchievement != null)
                {
                    Debug.WriteLine($"Found top unlocked {category} achievement: {topUnlockedAchievement.Achievement.AchievementName}");
                    return topUnlockedAchievement;
                }

                var lowestLockedAchievement = categoryAchievements
                    .Where(achievement => !achievement.IsUnlocked)
                    .OrderBy(achievement => achievement.Achievement.Points)
                    .FirstOrDefault();

                if (lowestLockedAchievement != null)
                {
                    Debug.WriteLine($"Found lowest locked {category} achievement: {lowestLockedAchievement.Achievement.AchievementName}");
                    return lowestLockedAchievement;
                }

                Debug.WriteLine($"No achievements found for {category}, returning empty achievement");
                return new AchievementWithStatus
                {
                    Achievement = new Achievement
                    {
                        AchievementName = $"No {category} Achievement",
                        Description = "Complete tasks to unlock this achievement",
                        AchievementType = category,
                        Points = 0,
                        Icon = "ms-appx:///Assets/empty_achievement.png"
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
                        Icon = "ms-appx:///Assets/empty_achievement.png"
                    },
                    IsUnlocked = false
                };
            }
        }
        
    }
}
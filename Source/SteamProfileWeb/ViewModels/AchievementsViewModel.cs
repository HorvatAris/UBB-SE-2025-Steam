using BusinessLayer.Models;

namespace SteamProfileWeb.ViewModels
{
    public class AchievementsViewModel
    {
        public List<AchievementWithStatus> FriendshipsAchievements { get; set; } = new();
        public List<AchievementWithStatus> OwnedGamesAchievements { get; set; } = new();
        public List<AchievementWithStatus> SoldGamesAchievements { get; set; } = new();
        public List<AchievementWithStatus> NumberOfPostsAchievements { get; set; } = new();
        public List<AchievementWithStatus> NumberOfReviewsGivenAchievements { get; set; } = new();
        public List<AchievementWithStatus> NumberOfReviewsReceivedAchievements { get; set; } = new();
        public List<AchievementWithStatus> YearsOfActivityAchievements { get; set; } = new();
        public List<AchievementWithStatus> DeveloperAchievements { get; set; } = new();
    }

}

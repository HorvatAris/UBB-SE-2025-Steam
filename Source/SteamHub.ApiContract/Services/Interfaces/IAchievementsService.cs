using BusinessLayer.Models;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface IAchievementsService
    {
        void InitializeAchievements();
        // GroupedAchievementsResult GetGroupedAchievementsForUser(int userIdentifier);
        List<Achievement> GetAchievementsForUser(int userIdentifier);
        void UnlockAchievementForUser(int userIdentifier);
        List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userIdentifier);
        int GetPointsForUnlockedAchievement(int userIdentifier, int achievementIdentifier);
    }
}

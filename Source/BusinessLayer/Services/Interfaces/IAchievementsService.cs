using BusinessLayer.Models;
using static BusinessLayer.Services.AchievementsService;

namespace BusinessLayer.Services.Interfaces
{
    public interface IAchievementsService
    {
        void InitializeAchievements();
        GroupedAchievementsResult GetGroupedAchievementsForUser(int userIdentifier);
        List<Achievement> GetAchievementsForUser(int userIdentifier);
        void UnlockAchievementForUser(int userIdentifier);
        List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userIdentifier);
        int GetPointsForUnlockedAchievement(int userIdentifier, int achievementIdentifier);
    }
}

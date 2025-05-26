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
        void RemoveAchievement(int userIdentifier, int achievementIdentifier);
        List<Achievement> GetUnlockedAchievementsForUser(int userIdentifier);
        List<Achievement> GetAllAchievements();
        AchievementUnlockedData GetUnlockedDataForAchievement(int userIdentifier, int achievementIdentifier);
        List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userIdentifier);
        int GetPointsForUnlockedAchievement(int userIdentifier, int achievementIdentifier);
    }
}

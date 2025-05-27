
using SteamHub.ApiContract.Models;
using static SteamHub.ApiContract.Services.AchievementsService;


namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface IAchievementsService
    {
        void InitializeAchievements();
        Task<GroupedAchievementsResult> GetGroupedAchievementsForUser(int userIdentifier);
        List<Achievement> GetAchievementsForUser(int userIdentifier);
        void UnlockAchievementForUser(int userIdentifier);
        void RemoveAchievement(int userIdentifier, int achievementIdentifier);
        List<Achievement> GetUnlockedAchievementsForUser(int userIdentifier);
        List<Achievement> GetAllAchievements();
        AchievementUnlockedData GetUnlockedDataForAchievement(int userIdentifier, int achievementIdentifier);
        Task<List<AchievementWithStatus>> GetAchievementsWithStatusForUser(int userIdentifier);
        int GetPointsForUnlockedAchievement(int userIdentifier, int achievementIdentifier);
    }
}

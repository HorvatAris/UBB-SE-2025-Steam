
using SteamHub.ApiContract.Models;
using System.Threading.Tasks;
using static SteamHub.ApiContract.Services.AchievementsService;


namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface IAchievementsService
    {
        Task InitializeAchievements();
        Task<GroupedAchievementsResult> GetGroupedAchievementsForUser(int userIdentifier);
        Task<List<Achievement>> GetAchievementsForUser(int userIdentifier);
        Task UnlockAchievementForUser(int userIdentifier);
        Task<List<Achievement>> GetAllAchievements();
        Task<List<AchievementWithStatus>> GetAchievementsWithStatusForUser(int userIdentifier);
        Task<int> GetPointsForUnlockedAchievement(int userIdentifier, int achievementIdentifier);
    }
}

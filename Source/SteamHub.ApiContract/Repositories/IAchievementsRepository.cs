using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Repositories
{
    public interface IAchievementsRepository
    {
        void InsertAchievements();
        Task<bool> IsAchievementsTableEmpty();
        void UpdateAchievementIconUrl(int points, string iconUrl);
        Task<List<Achievement>> GetAllAchievements();
        List<Achievement> GetUnlockedAchievementsForUser(int userId);
        Task UnlockAchievement(int userId, int achievementId);
        void RemoveAchievement(int userId, int achievementId);
        AchievementUnlockedData GetUnlockedDataForAchievement(int userId, int achievementId);
        Task<bool> IsAchievementUnlocked(int userId, int achievementId);
        Task<List<AchievementWithStatus>> GetAchievementsWithStatusForUser(int userId);
        Task<int> GetNumberOfSoldGames(int userId);
        Task<int> GetFriendshipCount(int userId);
        Task<int> GetNumberOfOwnedGames(int userId);
        Task<int> GetNumberOfReviewsGiven(int userId);
        Task<int> GetNumberOfReviewsReceived(int userId);
        Task<int> GetNumberOfPosts(int userId);
        Task<int> GetYearsOfAcftivity(int userId); // Note: Typo preserved to match original code
        Task<int?> GetAchievementIdByName(string achievementName);
        Task <bool> IsUserDeveloper(int userId);
    }
}

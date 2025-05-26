using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IAchievementsRepository
    {
        void InsertAchievements();
        bool IsAchievementsTableEmpty();
        void UpdateAchievementIconUrl(int points, string iconUrl);
        List<Achievement> GetAllAchievements();
        List<Achievement> GetUnlockedAchievementsForUser(int userId);
        void UnlockAchievement(int userId, int achievementId);
        void RemoveAchievement(int userId, int achievementId);
        AchievementUnlockedData GetUnlockedDataForAchievement(int userId, int achievementId);
        bool IsAchievementUnlocked(int userId, int achievementId);
        List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userId);
        int GetNumberOfSoldGames(int userId);
        int GetFriendshipCount(int userId);
        int GetNumberOfOwnedGames(int userId);
        int GetNumberOfReviewsGiven(int userId);
        int GetNumberOfReviewsReceived(int userId);
        int GetNumberOfPosts(int userId);
        int GetYearsOfAcftivity(int userId); // Note: Typo preserved to match original code
        int? GetAchievementIdByName(string achievementName);
        bool IsUserDeveloper(int userId);
    }
}

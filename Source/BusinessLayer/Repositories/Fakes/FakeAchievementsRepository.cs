using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Exceptions;

public class FakeAchievementsRepository : IAchievementsRepository
{
    public bool ThrowOnInsertAchievements { get; set; }
    public bool ThrowOnGetAllAchievements { get; set; }
    public bool ThrowOnGetAchievementsWithStatus { get; set; } = false;
    public bool ThrowOnUpdateIconUrl { get; set; } = false;
    public bool ThrowOnGetNumberOfSoldGames { get; set; } = false;
    public bool ThrowOnRemoveAchievement { get; set; } = false;
    public bool ThrowOnGetUnlockedAchievements { get; set; } = false;
    public bool ThrowOnGetUnlockedData { get; set; }
    public bool ThrowOnIsUnlocked { get; set; }
    public bool InsertAchievementsCalled { get; private set; }
    public List<(int points, string url)> UpdatedIcons { get; } = new();
    public List<AchievementWithStatus> AchievementsWithStatusToReturn { get; set; } = new();
    public List<Achievement> AllAchievementsToReturn { get; set; } = new();
    public AchievementUnlockedData UnlockedDataToReturn { get; set; }

    public bool AchievementsTableIsEmpty { get; set; } = true;
    public int NumberOfOwnedGames { get; set; }
    public int NumberOfSoldGames { get; set; }
    public int NumberOfReviewsGiven { get; set; }
    public int NumberOfReviewsReceived { get; set; }
    public int NumberOfFriends { get; set; }
    public int NumberOfPosts { get; set; }
    public Dictionary<string, Dictionary<int, int?>> AchievementIds { get; set; } = new();
    public List<(int userId, int achievementId)> UnlockedAchievements { get; } = new();
    public List<Achievement> UnlockedAchievementsToReturn { get; set; } = new();
    public void SetAchievementId(string category, int count, int? identifier)
    {
        if (!AchievementIds.ContainsKey(category))
        {
            AchievementIds[category] = new Dictionary<int, int?>();
        }
        AchievementIds[category][count] = identifier;
    }
    public void InsertAchievements()
    {
        if (ThrowOnInsertAchievements)
        {
            throw new RepositoryException("Insert fail");
        }

        InsertAchievementsCalled = true;
    }

    public bool IsAchievementsTableEmpty() => AchievementsTableIsEmpty;
    public void UpdateAchievementIconUrl(int points, string iconUrl)
    {
        if (ThrowOnUpdateIconUrl)
        {
            throw new RepositoryException("Fake update icon failure");
        }

        UpdatedIcons.Add((points, iconUrl));
    }

    public List<Achievement> GetAllAchievements()
    {
        if (ThrowOnGetAllAchievements)
        {
            throw new RepositoryException("GetAll failed");
        }

        return AllAchievementsToReturn;
    }

    public List<Achievement> GetUnlockedAchievementsForUser(int userIdentifier)
    {
        if (ThrowOnGetUnlockedAchievements)
        {
            throw new RepositoryException("Simulated failure");
        }

        return UnlockedAchievementsToReturn;
    }

    public void UnlockAchievement(int userIdentifier, int achievementIdentifier)
    {
        UnlockedAchievements.Add((userIdentifier, achievementIdentifier));
    }

    public void RemoveAchievement(int userId, int achievementId)
    {
        if (ThrowOnRemoveAchievement)
        {
            throw new RepositoryException("Fake remove fail");
        }
    }
    public AchievementUnlockedData GetUnlockedDataForAchievement(int userId, int achievementIdentifier)
    {
        if (ThrowOnGetUnlockedData)
        {
            throw new RepositoryException("Fake exception");
        }

        return UnlockedDataToReturn;
    }
    public bool IsAchievementUnlocked(int userIdentifier, int achievementIdentifier)
    {
        if (ThrowOnIsUnlocked)
        {
            throw new RepositoryException("Simulated DB error");
        }

        return UnlockedAchievements.Any(achievement => achievement.userId == userIdentifier && achievement.achievementId == achievementIdentifier);
    }

    public List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userIdentifier)
    {
        if (ThrowOnGetAchievementsWithStatus)
        {
            throw new RepositoryException("Simulated repo failure");
        }

        return AchievementsWithStatusToReturn;
    }

    public int GetNumberOfSoldGames(int userIdentifier)
    {
        if (ThrowOnGetNumberOfSoldGames)
        {
            throw new RepositoryException("Simulated failure");
        }

        return NumberOfSoldGames;
    }
    public int GetFriendshipCount(int userIdentifier) => NumberOfFriends;
    public int GetNumberOfOwnedGames(int userIdentifier) => NumberOfOwnedGames;
    public int GetNumberOfReviewsGiven(int userIdentifier) => NumberOfReviewsGiven;
    public int GetNumberOfReviewsReceived(int userIdentifier) => NumberOfReviewsReceived;
    public int GetNumberOfPosts(int userIdentifier) => NumberOfPosts;
    public Func<int, int> GetYearsOfAcftivity { get; set; } = _ => 0;
    int IAchievementsRepository.GetYearsOfAcftivity(int userIdentifier) => GetYearsOfAcftivity(userIdentifier);
    public Func<int, bool> IsUserDeveloper { get; set; } = _ => false;
    bool IAchievementsRepository.IsUserDeveloper(int userIdentifier) => IsUserDeveloper(userIdentifier);

    public int? GetAchievementIdByName(string achievementName)
    {
        foreach (var achievementCategory in AchievementIds)
        {
            foreach (var achievementCategoryPair in achievementCategory.Value)
            {
                var expectedName = achievementCategory.Key switch
                {
                    AchievementTypes.Friendships => string.Format(AchievementTypes.FriendshipFormat, achievementCategoryPair.Key),
                    AchievementTypes.OwnedGames => string.Format(AchievementTypes.OwnedGamesFormat, achievementCategoryPair.Key),
                    AchievementTypes.SoldGames => string.Format(AchievementTypes.SoldGamesFormat, achievementCategoryPair.Key),
                    AchievementTypes.ReviewsGiven => string.Format(AchievementTypes.ReviewsGivenFormat, achievementCategoryPair.Key),
                    AchievementTypes.ReviewsReceived => string.Format(AchievementTypes.ReviewsReceivedFormat, achievementCategoryPair.Key),
                    AchievementTypes.YearsOfActivity => string.Format(AchievementTypes.YearsOfActivityFormat, achievementCategoryPair.Key),
                    AchievementTypes.NumberOfPosts => string.Format(AchievementTypes.NumberOfPostsFormat, achievementCategoryPair.Key),
                    AchievementTypes.Developer => AchievementTypes.DeveloperFormat,
                    _ => string.Empty
                };

                if (expectedName == achievementName)
                {
                    return achievementCategoryPair.Value;
                }
            }
        }
        return null;
    }

    public static class AchievementTypes
    {
        public const string Friendships = "Friendships";
        public const string OwnedGames = "Owned Games";
        public const string SoldGames = "Sold Games";
        public const string ReviewsGiven = "Number of Reviews Given";
        public const string ReviewsReceived = "Number of Reviews Received";
        public const string YearsOfActivity = "Years of Activity";
        public const string NumberOfPosts = "Number of Posts";
        public const string Developer = "Developer";

        public const string FriendshipFormat = "FRIENDSHIP{0}";
        public const string OwnedGamesFormat = "OWNEDGAMES{0}";
        public const string SoldGamesFormat = "SOLDGAMES{0}";
        public const string ReviewsGivenFormat = "REVIEW{0}";
        public const string ReviewsReceivedFormat = "REVIEWR{0}";
        public const string YearsOfActivityFormat = "ACTIVITY{0}";
        public const string NumberOfPostsFormat = "POSTS{0}";
        public const string DeveloperFormat = "DEVELOPER";
    }
}

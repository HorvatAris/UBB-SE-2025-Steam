using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;
using System;

namespace SteamHub.ApiContract.Services
{
    public class AchievementsService : IAchievementsService
    {
        private readonly IAchievementsRepository achievementsRepository;

        public static class Categories
        {
            public const string Friendships = "Friendships";
            public const string OwnedGames = "Owned Games";
            public const string SoldGames = "Sold Games";
            public const string YearsOfActivity = "Years of Activity";
            public const string NumberOfPosts = "Number of Posts";
            public const string NumberOfReviewsGiven = "Number of Reviews Given";
            public const string NumberOfReviewsReceived = "Number of Reviews Received";
            public const string Developer = "Developer";
        }

        

        // Achievement Points Levels
        private const int PointsLevelBronze = 1;
        private const int PointsLevelSilver = 3;
        private const int PointsLevelGold = 5;
        private const int PointsLevelPlatinum = 10;
        private const int PointsLevelDiamond = 15;

        // Milestones for Achievements
        private const int MilestoneLevel1 = 1;
        private const int MilestoneLevel2 = 2;
        private const int MilestoneLevel3 = 3;
        private const int MilestoneLevel4 = 4;
        private const int MilestoneLevel5 = 5;
        private const int MilestoneLevel10 = 10;
        private const int MilestoneLevel50 = 50;
        private const int MilestoneLevel100 = 100;

        public async Task<GroupedAchievementsResult> GetGroupedAchievementsForUser(int userId)
        {
            try
            {
                // Unlock logic (business rules)
                await UnlockAchievementForUser(userId);

                // Get all achievements with status
                var achievements = await achievementsRepository.GetAchievementsWithStatusForUser(userId);

                // Group by category
                return new GroupedAchievementsResult
                {
                    AllAchievements = achievements,
                    Friendships = FilterByCategory(achievements, Categories.Friendships),
                    OwnedGames = FilterByCategory(achievements, Categories.OwnedGames),
                    SoldGames = FilterByCategory(achievements, Categories.SoldGames),
                    YearsOfActivity = FilterByCategory(achievements, Categories.YearsOfActivity),
                    NumberOfPosts = FilterByCategory(achievements, Categories.NumberOfPosts),
                    NumberOfReviewsGiven = FilterByCategory(achievements, Categories.NumberOfReviewsGiven),
                    NumberOfReviewsReceived = FilterByCategory(achievements, Categories.NumberOfReviewsReceived),
                    Developer = FilterByCategory(achievements, Categories.Developer)
                };
            }
            catch (Exception exception)
            {
                throw new Exception("Error grouping achievements for user.", exception);
            }
        }

        private List<AchievementWithStatus> FilterByCategory(List<AchievementWithStatus> allAchievements, string category)
        {
            var filteredAchievementsList = new List<AchievementWithStatus>();

            foreach (var achievement in allAchievements)
            {
                if (achievement.Achievement.AchievementType == category)
                {
                    filteredAchievementsList.Add(achievement);
                }
            }
            return filteredAchievementsList;
        }
        public AchievementsService(IAchievementsRepository achievementsRepository)
        {
            if (achievementsRepository == null)
            {
                throw new ArgumentNullException(nameof(achievementsRepository));
            }

            this.achievementsRepository = achievementsRepository;
        }

        public async Task InitializeAchievements()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Checking if achievements table is empty...");
                if (await achievementsRepository.IsAchievementsTableEmpty())
                {
                    System.Diagnostics.Debug.WriteLine("Achievements table is empty. Inserting achievements...");
                    achievementsRepository.InsertAchievements();
                    System.Diagnostics.Debug.WriteLine("Achievements inserted successfully.");
                    UpdateAchievementIconUrls();
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing achievements: {exception.Message}");
            }
        }

        private void UpdateAchievementIconUrls()
        {
            try
            {
                var iconUrls = new Dictionary<int, string>
                {
                    { PointsLevelBronze, "https://t4.ftcdn.net/jpg/00/99/53/31/360_F_99533164_fpE2O6vEjnXgYhonMyYBGtGUFCLqfTWA.jpg" },
                    { PointsLevelSilver, "https://png.pngtree.com/png-clipart/20200401/original/pngtree-gold-number-5-png-image_5330870.jpg" },
                    { PointsLevelGold, "https://t4.ftcdn.net/jpg/01/93/98/05/360_F_193980561_lymRkyDG6roPxmgA6x27fEaq3O3z3Mcf.jpg" },
                    { PointsLevelPlatinum, "https://as1.ftcdn.net/v2/jpg/02/42/16/20/1000_F_242162042_Ve21lDSZQl3Ebb9laV1WAJrR0ls3RGAn.jpg" },
                    { PointsLevelDiamond, "https://t3.ftcdn.net/jpg/02/79/95/72/360_F_279957287_UsAVf2woGRBWekMX68LiiWpwrrVVy9bI.jpg" }
                };
                foreach (var iconUrl in iconUrls)
                {
                    System.Diagnostics.Debug.WriteLine($"Updating icon URL for points: {iconUrl.Key}, URL: {iconUrl.Value}");
                    achievementsRepository.UpdateAchievementIconUrl(iconUrl.Key, iconUrl.Value);
                }

                System.Diagnostics.Debug.WriteLine("Achievement icon URLs updated successfully.");
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error updating achievement icon URLs: {exception.Message}");
            }
        }

        public async Task<List<Achievement>> GetAchievementsForUser(int userId)
        {
            try
            {
                return  await achievementsRepository.GetAllAchievements();
            }
            catch (Exception exception)
            {
                throw new Exception("Error retrieving achievements for user.", exception);
            }
        }

        public async Task UnlockAchievementForUser(int userId)
        {
            try
            {
                int numberOfSoldGames = await achievementsRepository.GetNumberOfSoldGames(userId);
                int numberOfFriends = await achievementsRepository.GetFriendshipCount(userId);
                int numberOfOwnedGames = await achievementsRepository.GetNumberOfOwnedGames(userId);
                int numberOfReviewsGiven = await achievementsRepository.GetNumberOfReviewsGiven(userId);
                int numberOfReviewsReceived = await achievementsRepository.GetNumberOfReviewsReceived(userId);
                int numberOfPosts = await achievementsRepository.GetNumberOfPosts(userId);
                int yearsOfActivity = await achievementsRepository.GetYearsOfAcftivity(userId);
                bool isDeveloper = await achievementsRepository.IsUserDeveloper(userId);

                if (numberOfFriends == MilestoneLevel1 || numberOfFriends == MilestoneLevel5 || numberOfFriends == MilestoneLevel10 || numberOfFriends == MilestoneLevel50 || numberOfFriends == MilestoneLevel100)
                {
                    int? achievementId = await GetAchievementIdByTypeAndCount(Categories.Friendships, numberOfFriends);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for friendships with count {numberOfFriends}: {achievementId}");
                    if (achievementId.HasValue && ! await achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        await achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfOwnedGames == MilestoneLevel1 || numberOfOwnedGames == MilestoneLevel5 || numberOfOwnedGames == MilestoneLevel10 || numberOfOwnedGames == MilestoneLevel50)
                {
                    int? achievementId = await GetAchievementIdByTypeAndCount(Categories.OwnedGames, numberOfOwnedGames);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for owned games with count {numberOfOwnedGames}: {achievementId}");
                    if (achievementId.HasValue && ! await achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                       await achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfSoldGames == MilestoneLevel1 || numberOfSoldGames == MilestoneLevel5 || numberOfSoldGames == MilestoneLevel10 || numberOfSoldGames == MilestoneLevel50)
                {
                    int? achievementId = await GetAchievementIdByTypeAndCount(Categories.SoldGames, numberOfSoldGames);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for sold games with count {numberOfSoldGames}: {achievementId}");
                    if (achievementId.HasValue && !await achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        await achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfReviewsGiven == MilestoneLevel1 || numberOfReviewsGiven == MilestoneLevel5 || numberOfReviewsGiven == MilestoneLevel10 || numberOfReviewsGiven == MilestoneLevel50)
                {
                    int? achievementId = await GetAchievementIdByTypeAndCount(Categories.NumberOfReviewsGiven, numberOfReviewsGiven);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for reviews given with count {numberOfReviewsGiven}: {achievementId}");
                    if (achievementId.HasValue && ! await achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        await achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfReviewsReceived == MilestoneLevel1 || numberOfReviewsReceived == MilestoneLevel5 || numberOfReviewsReceived == MilestoneLevel10 || numberOfReviewsReceived == MilestoneLevel50)
                {
                    int? achievementId = await GetAchievementIdByTypeAndCount(Categories.NumberOfReviewsReceived, numberOfReviewsReceived);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for reviews received with count {numberOfReviewsReceived}: {achievementId}");
                    if (achievementId.HasValue && ! await achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        await achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (numberOfPosts == MilestoneLevel1 || numberOfPosts == MilestoneLevel5 || numberOfPosts == MilestoneLevel10 || numberOfPosts == MilestoneLevel50)
                {
                    int? achievementId = await GetAchievementIdByTypeAndCount(Categories.NumberOfPosts, numberOfPosts);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for posts with count {numberOfPosts}: {achievementId}");
                    if (achievementId.HasValue && ! await achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        await achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }
                if (yearsOfActivity == MilestoneLevel1 || yearsOfActivity == MilestoneLevel2 || yearsOfActivity == MilestoneLevel3 || yearsOfActivity == MilestoneLevel4)
                {
                    int? achievementId = await GetAchievementIdByTypeAndCount(Categories.YearsOfActivity, yearsOfActivity);
                    System.Diagnostics.Debug.WriteLine($"Achievement ID for years of activity with count {yearsOfActivity}: {achievementId}");
                    if (achievementId.HasValue && !await achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        System.Diagnostics.Debug.WriteLine($"Unlocking achievement ID {achievementId.Value} for user {userId}");
                        await achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }

                if (isDeveloper)
                {
                    int? achievementId = await GetAchievementIdByTypeAndCount(Categories.Developer, MilestoneLevel1);
                    if (achievementId.HasValue && !await achievementsRepository.IsAchievementUnlocked(userId, achievementId.Value))
                    {
                        await achievementsRepository.UnlockAchievement(userId, achievementId.Value);
                    }
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {exception.Message}");
            }
        }

        public async Task<List<Achievement>> GetAllAchievements()
        {
            try
            {
                return await achievementsRepository.GetAllAchievements();
            }
            catch (Exception exception)
            {
                throw new Exception("Error retrieving unlocked achievements for user.", exception);
            }
        }

        public async Task <List<AchievementWithStatus>> GetAchievementsWithStatusForUser(int userId)
        {
            try
            {
                return await achievementsRepository.GetAchievementsWithStatusForUser(userId);
            }
            catch (Exception exception)
            {
                throw new Exception("Error retrieving achievements with status for user.", exception);
            }
        }

        public async Task<int?> GetAchievementIdByTypeAndCount(string type, int achievementsCount)
        {
            if (type == Categories.Friendships)
            {
                if (achievementsCount == MilestoneLevel1)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.FRIENDSHIP1);
                }
                else if (achievementsCount == MilestoneLevel5)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.FRIENDSHIP2);
                }
                else if (achievementsCount == MilestoneLevel10)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.FRIENDSHIP3);
                }
                else if (achievementsCount == MilestoneLevel50)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.FRIENDSHIP4);
                }
                else if (achievementsCount == MilestoneLevel100)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.FRIENDSHIP5);
                }
            }
            else if (type == Categories.OwnedGames)
            {
                if (achievementsCount == MilestoneLevel1)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.OWNEDGAMES1);
                }
                else if (achievementsCount == MilestoneLevel5)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.OWNEDGAMES2);
                }
                else if (achievementsCount == MilestoneLevel10)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.OWNEDGAMES3);
                }
                else if (achievementsCount == MilestoneLevel50)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.OWNEDGAMES4);
                }
            }
            else if (type == Categories.SoldGames)
            {
                if (achievementsCount == MilestoneLevel1)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.SOLDGAMES1);
                }
                else if (achievementsCount == MilestoneLevel5)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.SOLDGAMES2);
                }
                else if (achievementsCount == MilestoneLevel10)
                {
                    return  await achievementsRepository.GetAchievementIdByName(AchievementNames.SOLDGAMES3);
                }
                else if (achievementsCount == MilestoneLevel50)
                {
                    return await  achievementsRepository.GetAchievementIdByName(AchievementNames.SOLDGAMES4);
                }
            }
            else if (type == Categories.NumberOfReviewsGiven)
            {
                if (achievementsCount == MilestoneLevel1)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.REVIEW1);
                }
                else if (achievementsCount == MilestoneLevel5)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.REVIEW2);
                }
                else if (achievementsCount == MilestoneLevel10)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.REVIEW3);
                }
                else if (achievementsCount == MilestoneLevel50)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.REVIEW4);
                }
            }
            else if (type == Categories.NumberOfReviewsReceived)
            {
                if (achievementsCount == MilestoneLevel1)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.REVIEWR1);
                }
                else if (achievementsCount == MilestoneLevel5)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.REVIEWR2);
                }
                else if (achievementsCount == MilestoneLevel10)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.REVIEWR3);
                }
                else if (achievementsCount == MilestoneLevel50)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.REVIEWR4);
                }
            }
            else if (type == Categories.YearsOfActivity)
            {
                if (achievementsCount == MilestoneLevel1)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.ACTIVITY1);
                }
                else if (achievementsCount == MilestoneLevel2)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.ACTIVITY2);
                }
                else if (achievementsCount == MilestoneLevel3)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.ACTIVITY3);
                }
                else if (achievementsCount == MilestoneLevel4)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.ACTIVITY4);
                }
            }
            else if (type == Categories.NumberOfPosts)
            {
                if (achievementsCount == MilestoneLevel1)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.POSTS1);
                }
                else if (achievementsCount == MilestoneLevel5)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.POSTS2);
                }
                else if (achievementsCount == MilestoneLevel10)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.POSTS3);
                }
                else if (achievementsCount == MilestoneLevel50)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.POSTS4);
                }
            }
            else if (type == Categories.Developer)
            {
                if (achievementsCount == MilestoneLevel1)
                {
                    return await achievementsRepository.GetAchievementIdByName(AchievementNames.DEVELOPER);
                }
            }

            return null;
        }

        public async Task<int> GetPointsForUnlockedAchievement(int userId, int achievementId)
        {
            try
            {
                if (await achievementsRepository.IsAchievementUnlocked(userId, achievementId))
                {
                    List<Achievement> achievements = await achievementsRepository.GetAllAchievements();
                    Achievement foundAchievement = null;

                    foreach (var achievement in achievements)
                    {
                        if (achievement.AchievementId == achievementId)
                        {
                            foundAchievement = achievement;
                            break; // stop loop once found
                        }
                    }

                    if (foundAchievement != null)
                    {
                        return foundAchievement.Points;
                    }
                }

                throw new Exception("Achievement is not unlocked or does not exist.");
            }
            catch (Exception exception)
            {
                throw new Exception("Error retrieving points for unlocked achievement.", exception);
            }
        }

		
	}
}

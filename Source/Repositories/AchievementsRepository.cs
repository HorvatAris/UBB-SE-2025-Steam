using System;
using System.Data;
using System.Diagnostics;
using BusinessLayer.Data;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.Data.SqlClient;

namespace BusinessLayer.Repositories
{
    public class AchievementsRepository : IAchievementsRepository
    {
        private readonly IDataLink dataLink;

        public AchievementsRepository(IDataLink dataLink)
        {
            if (dataLink == null)
            {
                throw new ArgumentNullException(nameof(dataLink));
            }

            this.dataLink = dataLink;
        }

        public void InsertAchievements()
        {
            try
            {
                const string sqlCommand = @"
            SET NOCOUNT ON;

            INSERT INTO Achievements (achievement_name, description, achievement_type, points, icon_url) 
            VALUES
            ('FRIENDSHIP1', 'You made a friend, you get a point', 'Friendships', 1, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('FRIENDSHIP2', 'You made 5 friends, you get 3 points', 'Friendships', 3, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('FRIENDSHIP3', 'You made 10 friends, you get 5 points', 'Friendships', 5, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('FRIENDSHIP4', 'You made 50 friends, you get 10 points', 'Friendships', 10, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('FRIENDSHIP5', 'You made 100 friends, you get 15 points', 'Friendships', 15, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('OWNEDGAMES1', 'You own 1 game, you get 1 point', 'Owned Games', 1, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('OWNEDGAMES2', 'You own 5 games, you get 3 points', 'Owned Games', 3, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('OWNEDGAMES3', 'You own 10 games, you get 5 points', 'Owned Games', 5, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('OWNEDGAMES4', 'You own 50 games, you get 10 points', 'Owned Games', 10, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('SOLDGAMES1', 'You sold 1 game, you get 1 point', 'Sold Games', 1, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('SOLDGAMES2', 'You sold 5 games, you get 3 points', 'Sold Games', 3, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('SOLDGAMES3', 'You sold 10 games, you get 5 points', 'Sold Games', 5, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('SOLDGAMES4', 'You sold 50 games, you get 10 points', 'Sold Games', 10, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('REVIEW1', 'You gave 1 review, you get 1 point', 'Number of Reviews Given', 1, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('REVIEW2', 'You gave 5 reviews, you get 3 points', 'Number of Reviews Given', 3, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('REVIEW3', 'You gave 10 reviews, you get 5 points', 'Number of Reviews Given', 5, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('REVIEW4', 'You gave 50 reviews, you get 10 points', 'Number of Reviews Given', 10, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('REVIEWR1', 'You got 1 review, you get 1 point', 'Number of Reviews Received', 1, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('REVIEWR2', 'You got 5 reviews, you get 3 points', 'Number of Reviews Received', 3, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('REVIEWR3', 'You got 10 reviews, you get 5 points', 'Number of Reviews Received', 5, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('REVIEWR4', 'You got 50 reviews, you get 10 points', 'Number of Reviews Received', 10, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('DEVELOPER', 'You are a developer, you get 10 points', 'Developer', 10, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('ACTIVITY1', 'You have been active for 1 year, you get 1 point', 'Years of Activity', 1, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('ACTIVITY2', 'You have been active for 2 years, you get 3 points', 'Years of Activity', 3, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('ACTIVITY3', 'You have been active for 3 years, you get 5 points', 'Years of Activity', 5, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('ACTIVITY4', 'You have been active for 4 years, you get 10 points', 'Years of Activity', 10, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('POSTS1', 'You have made 1 post, you get 1 point', 'Number of Posts', 1, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('POSTS2', 'You have made 5 posts, you get 3 points', 'Number of Posts', 3, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('POSTS3', 'You have made 10 posts, you get 5 points', 'Number of Posts', 5, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png'),
            ('POSTS4', 'You have made 50 posts, you get 10 points', 'Number of Posts', 10, 'https://cdn-icons-png.flaticon.com/512/5139/5139999.png')
            ;";

                dataLink.ExecuteNonQuerySql(sqlCommand);
                System.Diagnostics.Debug.WriteLine("Achievements inserted successfully.");
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while inserting achievements.", exception);
            }
        }

        public bool IsAchievementsTableEmpty()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Executing SQL query to check if achievements table is empty...");
                const string sqlCommand = "SELECT COUNT(1) FROM Achievements";
                var result = dataLink.ExecuteScalarSql<int>(sqlCommand);
                System.Diagnostics.Debug.WriteLine($"Number of achievements in table: {result}");
                return result == 0;
            }
            catch (DatabaseOperationException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error while checking if achievements table is empty: {exception.Message}");
                throw new RepositoryException("An unexpected error occurred while checking if achievements table is empty.", exception);
            }
        }

        public void UpdateAchievementIconUrl(int points, string iconUrl)
        {
            try
            {
                const string sqlCommand = @"
            SET NOCOUNT ON;
            UPDATE Achievements
            SET icon_url = @iconUrl
            WHERE points = @points;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@points", points),
            new SqlParameter("@iconUrl", iconUrl)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error while updating achievement icon URL: {exception.Message}");
                throw new RepositoryException("An unexpected error occurred while updating achievement icon URL.", exception);
            }
        }

        public List<Achievement> GetAllAchievements()
        {
            try
            {
                const string sqlCommand = @"
            SELECT *
            FROM Achievements
            ORDER BY points DESC;";

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand);
                return MapDataTableToAchievements(dataTable);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving achievements.", exception);
            }
        }

        public List<Achievement> GetUnlockedAchievementsForUser(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SELECT a.achievement_id, a.achievement_name, a.description, a.achievement_type, a.points, a.icon_url, ua.unlocked_at
            FROM Achievements a
            INNER JOIN UserAchievements ua ON a.achievement_id = ua.achievement_id
            WHERE ua.user_id = @userId;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@userId", userIdentifier)
                };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return MapDataTableToAchievements(dataTable);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving unlocked achievements.", exception);
            }
        }

        public void UnlockAchievement(int userIdentifier, int achievementId)
        {
            try
            {
                const string sqlCommand = @"
            IF NOT EXISTS (
                SELECT 1 FROM UserAchievements WHERE user_id = @userId AND achievement_id = @achievementId
            )
            BEGIN
                INSERT INTO UserAchievements (user_id, achievement_id, unlocked_at)
                VALUES (@userId, @achievementId, GETDATE());
            END;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@userId", userIdentifier),
            new SqlParameter("@achievementId", achievementId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while unlocking achievement.", exception);
            }
        }

        public void RemoveAchievement(int userIdentifier, int achievementId)
        {
            try
            {
                const string sqlCommand = @"
            DELETE FROM UserAchievements
            WHERE user_id = @userId AND achievement_id = @achievementId;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@userId", userIdentifier),
            new SqlParameter("@achievementId", achievementId)
                };

                dataLink.ExecuteNonQuerySql(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while removing achievement.", exception);
            }
        }

        public AchievementUnlockedData GetUnlockedDataForAchievement(int userIdentifier, int achievementId)
        {
            try
            {
                const string sqlCommand = @"
            SELECT a.achievement_name AS AchievementName, a.description AS AchievementDescription, ua.unlocked_at AS UnlockDate
            FROM UserAchievements ua
            JOIN Achievements a ON ua.achievement_id = a.achievement_id
            WHERE ua.user_id = @user_id AND ua.achievement_id = @achievement_id;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier),
            new SqlParameter("@achievement_id", achievementId)
                };

                var dataTable = dataLink.ExecuteReaderSql(sqlCommand, parameters);
                return dataTable.Rows.Count > 0 ? MapDataRowToAchievementUnlockedData(dataTable.Rows[0]) : null;
            }
            catch (DatabaseOperationException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Exception: {exception.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack Trace: {exception.StackTrace}");
                throw new RepositoryException("An unexpected error occurred while retrieving achievement data.", exception);
            }
        }

        public bool IsAchievementUnlocked(int userIdentifier, int achievementId)
        {
            try
            {
                const string sqlCommand = @"
            SELECT COUNT(1) as IsUnlocked
            FROM UserAchievements
            WHERE user_id = @user_id
            AND achievement_id = @achievement_id;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier),
            new SqlParameter("@achievement_id", achievementId)
                };

                int? result = dataLink.ExecuteScalarSql<int>(sqlCommand, parameters);
                return result > 0;
            }
            catch (DatabaseOperationException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error during ExecuteScalar operation: {exception.Message}");
                throw new RepositoryException("Error checking if achievement is unlocked.", exception);
            }
        }

        public List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userIdentifier)
        {
            try
            {
                var allAchievements = GetAllAchievements();
                var achievementsWithStatus = new List<AchievementWithStatus>();

                foreach (var achievement in allAchievements)
                {
                    var isUnlocked = IsAchievementUnlocked(userIdentifier, achievement.AchievementId);
                    var unlockedData = GetUnlockedDataForAchievement(userIdentifier, achievement.AchievementId);
                    achievementsWithStatus.Add(new AchievementWithStatus
                    {
                        Achievement = achievement,
                        IsUnlocked = isUnlocked,
                        UnlockedDate = unlockedData?.UnlockDate
                    });
                }

                return achievementsWithStatus;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {exception.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {exception.StackTrace}");
                if (exception.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {exception.InnerException.Message}");
                }
                throw new RepositoryException("Error retrieving achievements with status for user.", exception);
            }
        }

        public int GetNumberOfSoldGames(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SET NOCOUNT ON;
            SELECT COUNT(*) AS NumberOfSoldGames
            FROM SoldGames
            WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier)
                };

                var result = dataLink.ExecuteScalarSql<int>(sqlCommand, parameters);
                return result;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving number of sold games.", exception);
            }
        }

        public int GetFriendshipCount(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SELECT COUNT(*) as friend_count
            FROM Friendships
            WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier)
                };

                return dataLink.ExecuteScalarSql<int>(sqlCommand, parameters);
            }
            catch (DatabaseOperationException exception)
            {
                Debug.WriteLine($"Unexpected Error: {exception.Message}");
                throw new RepositoryException("An unexpected error occurred while retrieving friendship count.", exception);
            }
        }

        public int GetNumberOfOwnedGames(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SET NOCOUNT ON;
            SELECT COUNT(*) AS NumberOfOwnedGames
            FROM OwnedGames
            WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier)
                };

                var result = dataLink.ExecuteScalarSql<int>(sqlCommand, parameters);
                return result;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving number of owned games.", exception);
            }
        }

        public int GetNumberOfReviewsGiven(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SET NOCOUNT ON;
            SELECT COUNT(*) AS NumberOfOwnedGames
            FROM ReviewsGiven
            WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier)
                };

                var result = dataLink.ExecuteScalarSql<int>(sqlCommand, parameters);
                return result;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving number of reviews given.", exception);
            }
        }

        public int GetNumberOfReviewsReceived(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SET NOCOUNT ON;
            SELECT COUNT(*) AS NumberOfOwnedGames
            FROM ReviewsReceived
            WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier)
                };

                var result = dataLink.ExecuteScalarSql<int>(sqlCommand, parameters);
                return result;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving number of reviews received.", exception);
            }
        }

        public int GetNumberOfPosts(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SET NOCOUNT ON;
            SELECT COUNT(*) AS NumberOfPosts
            FROM Posts
            WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier)
                };

                var result = dataLink.ExecuteScalarSql<int>(sqlCommand, parameters);
                return result;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving number of posts.", exception);
            }
        }

        public int GetYearsOfAcftivity(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SELECT created_at
            FROM Users
            WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier)
                };

                var createdAt = dataLink.ExecuteScalarSql<DateTime>(sqlCommand, parameters);
                var yearsOfActivity = DateTime.Now.Year - createdAt.Year;

                // Adjust for the case where the user hasn't completed the current year
                if (DateTime.Now.DayOfYear < createdAt.DayOfYear)
                {
                    yearsOfActivity--;
                }

                return yearsOfActivity;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving years of activity.", exception);
            }
        }

        public int? GetAchievementIdByName(string achievementName)
        {
            try
            {
                const string sqlCommand = @"
            SET NOCOUNT ON;
            SELECT achievement_id FROM Achievements WHERE achievement_name = @achievementName;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@achievementName", achievementName)
                };

                var result = dataLink.ExecuteScalarSql<int?>(sqlCommand, parameters);
                System.Diagnostics.Debug.WriteLine($"Achievement ID for name {achievementName}: {result}");
                return result;
            }
            catch (DatabaseOperationException exception)
            {
                System.Diagnostics.Debug.WriteLine($"Unexpected error while retrieving achievement ID: {exception.Message}");
                throw new RepositoryException("An unexpected error occurred while retrieving achievement ID.", exception);
            }
        }

        public bool IsUserDeveloper(int userIdentifier)
        {
            try
            {
                const string sqlCommand = @"
            SELECT developer
            FROM Users
            WHERE user_id = @user_id;";

                var parameters = new SqlParameter[]
                {
            new SqlParameter("@user_id", userIdentifier)
                };

                var result = dataLink.ExecuteScalarSql<bool>(sqlCommand, parameters);
                return result;
            }
            catch (DatabaseOperationException exception)
            {
                throw new RepositoryException("An unexpected error occurred while retrieving developer status.", exception);
            }
        }
        private static List<Achievement> MapDataTableToAchievements(DataTable dataTable)
        {
            var achievements = new List<Achievement>();
            foreach (DataRow row in dataTable.Rows)
            {
                achievements.Add(MapDataRowToAchievement(row));
            }
            return achievements;
        }
        private static Achievement MapDataRowToAchievement(DataRow row)
        {
            string achievementName = string.Empty;
            string description = string.Empty;
            string achievementType = string.Empty;
            string iconUrl = string.Empty;

            if (row["achievement_name"] != DBNull.Value)
            {
                achievementName = row["achievement_name"].ToString();
            }

            if (row["description"] != DBNull.Value)
            {
                description = row["description"].ToString();
            }

            if (row["achievement_type"] != DBNull.Value)
            {
                achievementType = row["achievement_type"].ToString();
            }

            if (row["icon_url"] != DBNull.Value)
            {
                iconUrl = row["icon_url"].ToString();
            }

            return new Achievement
            {
                AchievementId = Convert.ToInt32(row["achievement_id"]),
                AchievementName = achievementName,
                Description = description,
                AchievementType = achievementType,
                Points = Convert.ToInt32(row["points"]),
                Icon = iconUrl
            };
        }

        private static AchievementUnlockedData MapDataRowToAchievementUnlockedData(DataRow row)
        {
            string achievementName = string.Empty;
            string achievementDescription = string.Empty;
            DateTime? unlockDate = null;

            if (row["AchievementName"] != DBNull.Value)
            {
                achievementName = row["AchievementName"].ToString();
            }

            if (row["AchievementDescription"] != DBNull.Value)
            {
                achievementDescription = row["AchievementDescription"].ToString();
            }

            if (row["UnlockDate"] != DBNull.Value)
            {
                unlockDate = Convert.ToDateTime(row["UnlockDate"]);
            }

            return new AchievementUnlockedData
            {
                AchievementName = achievementName,
                AchievementDescription = achievementDescription,
                UnlockDate = unlockDate
            };
        }
    }
}

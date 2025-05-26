using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BusinessLayer.DataContext;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BusinessLayer.Repositories
{
    public class AchievementsRepository : IAchievementsRepository
    {
        private readonly ApplicationDbContext context;

        public AchievementsRepository(ApplicationDbContext newContext)
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public void InsertAchievements()
        {
            if (context.Achievements.Any())
            {
                return;
            }

            var list = new[]
            {
                new Achievement { AchievementName = "FRIENDSHIP1", Description = "You made a friend, you get a point", AchievementType = "Friendships", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "FRIENDSHIP2", Description = "You made 5 friends, you get 3 points", AchievementType = "Friendships", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "FRIENDSHIP3", Description = "You made 10 friends, you get 5 points", AchievementType = "Friendships", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "FRIENDSHIP4", Description = "You made 50 friends, you get 10 points", AchievementType = "Friendships", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "FRIENDSHIP5", Description = "You made 100 friends, you get 15 points", AchievementType = "Friendships", Points = 15, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new Achievement { AchievementName = "OWNEDGAMES1", Description = "You own 1 game, you get 1 point", AchievementType = "Owned Games", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "OWNEDGAMES2", Description = "You own 5 games, you get 3 points", AchievementType = "Owned Games", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "OWNEDGAMES3", Description = "You own 10 games, you get 5 points", AchievementType = "Owned Games", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "OWNEDGAMES4", Description = "You own 50 games, you get 10 points", AchievementType = "Owned Games", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new Achievement { AchievementName = "SOLDGAMES1", Description = "You sold 1 game, you get 1 point", AchievementType = "Sold Games", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "SOLDGAMES2", Description = "You sold 5 games, you get 3 points", AchievementType = "Sold Games", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "SOLDGAMES3", Description = "You sold 10 games, you get 5 points", AchievementType = "Sold Games", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "SOLDGAMES4", Description = "You sold 50 games, you get 10 points", AchievementType = "Sold Games", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new Achievement { AchievementName = "REVIEW1", Description = "You gave 1 review, you get 1 point", AchievementType = "Number of Reviews Given", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "REVIEW2", Description = "You gave 5 reviews, you get 3 points", AchievementType = "Number of Reviews Given", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "REVIEW3", Description = "You gave 10 reviews, you get 5 points", AchievementType = "Number of Reviews Given", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "REVIEW4", Description = "You gave 50 reviews, you get 10 points", AchievementType = "Number of Reviews Given", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new Achievement { AchievementName = "REVIEWR1", Description = "You got 1 review, you get 1 point", AchievementType = "Number of Reviews Received", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "REVIEWR2", Description = "You got 5 reviews, you get 3 points", AchievementType = "Number of Reviews Received", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "REVIEWR3", Description = "You got 10 reviews, you get 5 points", AchievementType = "Number of Reviews Received", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "REVIEWR4", Description = "You got 50 reviews, you get 10 points", AchievementType = "Number of Reviews Received", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new Achievement { AchievementName = "DEVELOPER", Description = "You are a developer, you get 10 points", AchievementType = "Developer", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new Achievement { AchievementName = "ACTIVITY1", Description = "You have been active for 1 year, you get 1 point", AchievementType = "Years of Activity", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "ACTIVITY2", Description = "You have been active for 2 years, you get 3 points", AchievementType = "Years of Activity", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "ACTIVITY3", Description = "You have been active for 3 years, you get 5 points", AchievementType = "Years of Activity", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "ACTIVITY4", Description = "You have been active for 4 years, you get 10 points", AchievementType = "Years of Activity", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new Achievement { AchievementName = "POSTS1", Description = "You have made 1 post, you get 1 point", AchievementType = "Number of Posts", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "POSTS2", Description = "You have made 5 posts, you get 3 points", AchievementType = "Number of Posts", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "POSTS3", Description = "You have made 10 posts, you get 5 points", AchievementType = "Number of Posts", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new Achievement { AchievementName = "POSTS4", Description = "You have made 50 posts, you get 10 points", AchievementType = "Number of Posts", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" }
            };

            context.Achievements.AddRange(list);
            context.SaveChanges();
        }

        public bool IsAchievementsTableEmpty()
            => !context.Achievements.Any();

        public void UpdateAchievementIconUrl(int points, string iconUrl)
        {
            var ach = context.Achievements.FirstOrDefault(a => a.Points == points);
            if (ach == null)
            {
                return;
            }
            ach.Icon = iconUrl;
            context.SaveChanges();
        }

        public List<Achievement> GetAllAchievements()
            => context.Achievements
                  .AsNoTracking()
                  .OrderByDescending(a => a.Points)
                  .ToList();

        public List<Achievement> GetUnlockedAchievementsForUser(int userIdentifier)
            => context.UserAchievements
                  .Where(ua => ua.UserId == userIdentifier)
                  .Include(ua => ua.Achievement)
                  .Select(ua => ua.Achievement)
                  .ToList();

        public void UnlockAchievement(int userIdentifier, int achievementId)
        {
            if (context.UserAchievements.Any(ua => ua.UserId == userIdentifier && ua.AchievementId == achievementId))
            {
                return;
            }

            context.UserAchievements.Add(new UserAchievement
            {
                UserId = userIdentifier,
                AchievementId = achievementId,
                UnlockedAt = DateTime.UtcNow
            });
            context.SaveChanges();
        }

        public void RemoveAchievement(int userIdentifier, int achievementId)
        {
            var ua = context.UserAchievements
                        .FirstOrDefault(x => x.UserId == userIdentifier && x.AchievementId == achievementId);
            if (ua == null)
            {
                return;
            }
            context.UserAchievements.Remove(ua);
            context.SaveChanges();
        }

        public AchievementUnlockedData GetUnlockedDataForAchievement(int userIdentifier, int achievementId)
        {
            var ua = context.UserAchievements
                        .Include(x => x.Achievement)
                        .FirstOrDefault(x => x.UserId == userIdentifier && x.AchievementId == achievementId);
            if (ua == null)
            {
                return null;
            }
            return new AchievementUnlockedData
            {
                AchievementName = ua.Achievement.AchievementName,
                AchievementDescription = ua.Achievement.Description,
                UnlockDate = ua.UnlockedAt
            };
        }

        public bool IsAchievementUnlocked(int userIdentifier, int achievementId)
            => context.UserAchievements.Any(ua => ua.UserId == userIdentifier && ua.AchievementId == achievementId);

        public List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userIdentifier)
        {
            var all = GetAllAchievements();
            var unlockedIds = new HashSet<int>(
                context.UserAchievements
                   .Where(ua => ua.UserId == userIdentifier)
                   .Select(ua => ua.AchievementId));

            return all.Select(a => new AchievementWithStatus
            {
                Achievement = a,
                IsUnlocked = unlockedIds.Contains(a.AchievementId),
                UnlockedDate = unlockedIds.Contains(a.AchievementId)
                    ? context.UserAchievements.First(ua => ua.UserId == userIdentifier && ua.AchievementId == a.AchievementId).UnlockedAt
                    : (DateTime?)null
            }).ToList();
        }

        public int GetFriendshipCount(int userIdentifier)
            => context.Friendships.Count(f => f.UserId == userIdentifier);

        public int GetNumberOfOwnedGames(int userIdentifier)
            => context.OwnedGames.Count(og => og.UserId == userIdentifier);

        public int GetNumberOfSoldGames(int userIdentifier)
            => context.SoldGames.Count(sg => sg.UserId == userIdentifier);

        public int GetNumberOfReviewsGiven(int userIdentifier)
            => context.Reviews.Count(r => r.UserIdentifier == userIdentifier);

        public int GetNumberOfReviewsReceived(int userIdentifier)
            => context.Reviews.Count(r => r.GameIdentifier == userIdentifier); // adjust if you store receiver differently

        public int GetNumberOfPosts(int userIdentifier)
            => context.NewsPosts.Count(p => p.AuthorId == userIdentifier);

        public int GetYearsOfAcftivity(int userIdentifier)
        {
            var created = context.Users
                             .Where(u => u.UserId == userIdentifier)
                             .Select(u => u.CreatedAt)
                             .SingleOrDefault();
            var years = DateTime.Now.Year - created.Year;
            if (DateTime.Now.DayOfYear < created.DayOfYear)
            {
                years--;
            }
            return years;
        }

        public int? GetAchievementIdByName(string achievementName)
            => context.Achievements
                  .Where(a => a.AchievementName == achievementName)
                  .Select(a => (int?)a.AchievementId)
                  .SingleOrDefault();

        public bool IsUserDeveloper(int userIdentifier)
            => context.Users
                  .Where(u => u.UserId == userIdentifier)
                  .Select(u => u.IsDeveloper)
                  .SingleOrDefault();

        // Old test methods // -------------------------------------------------
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

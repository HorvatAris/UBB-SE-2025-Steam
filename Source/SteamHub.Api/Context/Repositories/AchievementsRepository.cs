using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.EntityFrameworkCore;
using SteamHub.Api.Context;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Repositories;

namespace BusinessLayer.Repositories
{
    public class AchievementsRepository : IAchievementsRepository
    {
        private readonly DataContext context;

        public AchievementsRepository(DataContext newContext)
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
                new SteamHub.Api.Entities.Achievement { AchievementName = "FRIENDSHIP1", Description = "You made achievement friend, you get achievement point", AchievementType = "Friendships", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "FRIENDSHIP2", Description = "You made 5 friends, you get 3 points", AchievementType = "Friendships", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "FRIENDSHIP3", Description = "You made 10 friends, you get 5 points", AchievementType = "Friendships", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "FRIENDSHIP4", Description = "You made 50 friends, you get 10 points", AchievementType = "Friendships", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "FRIENDSHIP5", Description = "You made 100 friends, you get 15 points", AchievementType = "Friendships", Points = 15, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new SteamHub.Api.Entities.Achievement { AchievementName = "OWNEDGAMES1", Description = "You own 1 game, you get 1 point", AchievementType = "Owned Games", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "OWNEDGAMES2", Description = "You own 5 games, you get 3 points", AchievementType = "Owned Games", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "OWNEDGAMES3", Description = "You own 10 games, you get 5 points", AchievementType = "Owned Games", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "OWNEDGAMES4", Description = "You own 50 games, you get 10 points", AchievementType = "Owned Games", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new SteamHub.Api.Entities.Achievement { AchievementName = "SOLDGAMES1", Description = "You sold 1 game, you get 1 point", AchievementType = "Sold Games", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "SOLDGAMES2", Description = "You sold 5 games, you get 3 points", AchievementType = "Sold Games", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "SOLDGAMES3", Description = "You sold 10 games, you get 5 points", AchievementType = "Sold Games", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "SOLDGAMES4", Description = "You sold 50 games, you get 10 points", AchievementType = "Sold Games", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new SteamHub.Api.Entities.Achievement { AchievementName = "REVIEW1", Description = "You gave 1 review, you get 1 point", AchievementType = "Number of Reviews Given", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "REVIEW2", Description = "You gave 5 reviews, you get 3 points", AchievementType = "Number of Reviews Given", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "REVIEW3", Description = "You gave 10 reviews, you get 5 points", AchievementType = "Number of Reviews Given", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "REVIEW4", Description = "You gave 50 reviews, you get 10 points", AchievementType = "Number of Reviews Given", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new SteamHub.Api.Entities.Achievement { AchievementName = "REVIEWR1", Description = "You got 1 review, you get 1 point", AchievementType = "Number of Reviews Received", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "REVIEWR2", Description = "You got 5 reviews, you get 3 points", AchievementType = "Number of Reviews Received", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "REVIEWR3", Description = "You got 10 reviews, you get 5 points", AchievementType = "Number of Reviews Received", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "REVIEWR4", Description = "You got 50 reviews, you get 10 points", AchievementType = "Number of Reviews Received", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new SteamHub.Api.Entities.Achievement { AchievementName = "DEVELOPER", Description = "You are achievement developer, you get 10 points", AchievementType = "Developer", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new SteamHub.Api.Entities.Achievement { AchievementName = "ACTIVITY1", Description = "You have been active for 1 year, you get 1 point", AchievementType = "Years of Activity", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "ACTIVITY2", Description = "You have been active for 2 years, you get 3 points", AchievementType = "Years of Activity", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "ACTIVITY3", Description = "You have been active for 3 years, you get 5 points", AchievementType = "Years of Activity", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "ACTIVITY4", Description = "You have been active for 4 years, you get 10 points", AchievementType = "Years of Activity", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

                new SteamHub.Api.Entities.Achievement { AchievementName = "POSTS1", Description = "You have made 1 post, you get 1 point", AchievementType = "Number of Posts", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "POSTS2", Description = "You have made 5 posts, you get 3 points", AchievementType = "Number of Posts", Points = 3, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "POSTS3", Description = "You have made 10 posts, you get 5 points", AchievementType = "Number of Posts", Points = 5, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
                new SteamHub.Api.Entities.Achievement { AchievementName = "POSTS4", Description = "You have made 50 posts, you get 10 points", AchievementType = "Number of Posts", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" }
            };

            context.Achievements.AddRange(list);
            context.SaveChanges();
        }

        public async Task<bool> IsAchievementsTableEmpty()
        {
            return !await context.Achievements.AnyAsync();
        }

        public void UpdateAchievementIconUrl(int points, string iconUrl)
        {
            var achievement = context.Achievements.FirstOrDefault(achievement => achievement.Points == points);
            if (achievement == null)
            {
                return;
            }
            achievement.Icon = iconUrl;
            context.SaveChanges();
        }
        //in data context => entities
        public SteamHub.ApiContract.Models.Achievement MapEnitytToModel(Achievement achievement)
        {
            return new SteamHub.ApiContract.Models.Achievement
            {
                AchievementId = achievement.AchievementId,
                AchievementName = achievement.AchievementName,
                Description = achievement.Description,
                AchievementType = achievement.AchievementType,
                Points = achievement.Points,
                Icon = achievement.Icon
            };
        }
        public async Task<List<SteamHub.ApiContract.Models.Achievement>> GetAllAchievements()
        {
            return await context.Achievements
                .AsNoTracking()
                .OrderByDescending(achievement => achievement.Points)
                .Select(achievement => new SteamHub.ApiContract.Models.Achievement
                {
                    AchievementId = achievement.AchievementId,
                    AchievementName = achievement.AchievementName,
                    Description = achievement.Description,
                    AchievementType = achievement.AchievementType,
                    Points = achievement.Points,
                    Icon = achievement.Icon
                })
                .ToListAsync();
        }

        public List<SteamHub.ApiContract.Models.Achievement> GetUnlockedAchievementsForUser(int userIdentifier)
            => context.UserAchievements
                  .Where(currentUserAchievement => currentUserAchievement.UserId == userIdentifier)
                  .Include(currentUserAchievement => currentUserAchievement.Achievement)
                  .Select(user_achievement => new SteamHub.ApiContract.Models.Achievement
                  {
                      AchievementId = user_achievement.Achievement.AchievementId,
                      AchievementName = user_achievement.Achievement.AchievementName,
                      Description = user_achievement.Achievement.Description,
                      AchievementType = user_achievement.Achievement.AchievementType,
                      Points = user_achievement.Achievement.Points,
                      Icon = user_achievement.Achievement.Icon
                  })
                  .ToList();

        public async Task UnlockAchievement(int userIdentifier, int achievementId)
        {
            if (context.UserAchievements.Any(currentUserAchievement => currentUserAchievement.UserId == userIdentifier && currentUserAchievement.AchievementId == achievementId))
            {
                return;
            }

            context.UserAchievements.Add(new SteamHub.Api.Entities.UserAchievement
            {
                UserId = userIdentifier,
                AchievementId = achievementId,
                UnlockedAt = DateTime.UtcNow
            });
            context.SaveChanges();
        }

        public void RemoveAchievement(int userIdentifier, int achievementId)
        {
            var achievementForRemove = context.UserAchievements
                        .FirstOrDefault(user_achievement => user_achievement.UserId == userIdentifier && user_achievement.AchievementId == achievementId);
            if (achievementForRemove == null)
            {
                return;
            }
            context.UserAchievements.Remove(achievementForRemove);
            context.SaveChanges();
        }

        public AchievementUnlockedData GetUnlockedDataForAchievement(int userIdentifier, int achievementId)
        {
            var unlockedAchievement = context.UserAchievements
                        .Include(user_achievement => user_achievement.Achievement)
                        .FirstOrDefault(user_achievement => user_achievement.UserId == userIdentifier && user_achievement.AchievementId == achievementId);
            if (unlockedAchievement == null)
            {
                return null;
            }
            return new AchievementUnlockedData
            {
                AchievementName = unlockedAchievement.Achievement.AchievementName,
                AchievementDescription = unlockedAchievement.Achievement.Description,
                UnlockDate = unlockedAchievement.UnlockedAt
            };
        }

        public async Task<bool> IsAchievementUnlocked(int userIdentifier, int achievementId)
        {
            return await context.UserAchievements
                .AnyAsync(currentAchievement => currentAchievement.UserId == userIdentifier && currentAchievement.AchievementId == achievementId);
        }

        public async Task<List<AchievementWithStatus>> GetAchievementsWithStatusForUser(int userIdentifier)
        {
            var all_achievements = await GetAllAchievements();
            var unlockedIds = new HashSet<int>(
                context.UserAchievements
                   .Where(currentUserAchievement => currentUserAchievement.UserId == userIdentifier)
                   .Select(userAchievement => userAchievement.AchievementId));

            return all_achievements.Select(currentAchievement => new AchievementWithStatus
            {
                Achievement = currentAchievement,
                IsUnlocked = unlockedIds.Contains(currentAchievement.AchievementId),
                UnlockedDate = unlockedIds.Contains(currentAchievement.AchievementId)
                    ? context.UserAchievements.First(user_achievement => user_achievement.UserId == userIdentifier && user_achievement.AchievementId == currentAchievement.AchievementId).UnlockedAt
                    : (DateTime?)null
            }).ToList();
        }

        public int GetFriendshipCount(int userIdentifier)
            => context.Friendships.Count(friendship => friendship.UserId == userIdentifier);

        public int GetNumberOfOwnedGames(int userIdentifier)
            => context.OwnedGames.Count(owned_game => owned_game.UserId == userIdentifier);

        public int GetNumberOfSoldGames(int userIdentifier)
            => context.SoldGames.Count(sold_game => sold_game.UserId == userIdentifier);

        public int GetNumberOfReviewsGiven(int userIdentifier)
            => context.Reviews.Count(review => review.UserIdentifier == userIdentifier);

        public int GetNumberOfReviewsReceived(int userIdentifier)
            => context.Reviews.Count(review => review.GameIdentifier == userIdentifier); // adjust if you store receiver differently

        public int GetNumberOfPosts(int userIdentifier)
            => context.NewsPosts.Count(post => post.AuthorId == userIdentifier);

        public int GetYearsOfAcftivity(int userIdentifier)
        {
            var created = context.Users
                             .Where(user => user.UserId == userIdentifier)
                             .Select(user => user.CreatedAt)
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
                  .Where(achievement => achievement.AchievementName == achievementName)
                  .Select(achievement => (int?)achievement.AchievementId)
                  .SingleOrDefault();

        public bool IsUserDeveloper(int userIdentifier)
            => context.Users
                  .Where(user => user.UserId == userIdentifier)
                  .Select(user => user.UserRole == SteamHub.ApiContract.Models.Common.UserRole.Developer)
                  .SingleOrDefault();
    }

}


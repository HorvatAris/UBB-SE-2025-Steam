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
                new SteamHub.Api.Entities.Achievement { AchievementName = "FRIENDSHIP1", Description = "You made a friend, you get a point", AchievementType = "Friendships", Points = 1, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },
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

                new SteamHub.Api.Entities.Achievement { AchievementName = "DEVELOPER", Description = "You are a developer, you get 10 points", AchievementType = "Developer", Points = 10, Icon = "https://cdn-icons-png.flaticon.com/512/5139/5139999.png" },

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
            var ach = context.Achievements.FirstOrDefault(a => a.Points == points);
            if (ach == null)
            {
                return;
            }
            ach.Icon = iconUrl;
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
                  .Select(ua => new SteamHub.ApiContract.Models.Achievement
                  {
                      AchievementId = ua.Achievement.AchievementId,
                      AchievementName = ua.Achievement.AchievementName,
                      Description = ua.Achievement.Description,
                      AchievementType = ua.Achievement.AchievementType,
                      Points = ua.Achievement.Points,
                      Icon = ua.Achievement.Icon
                  })
                  .ToList();

        public async Task UnlockAchievement(int userIdentifier, int achievementId)
        {
            if (context.UserAchievements.Any(currentUserAchievement => currentUserAchievement.UserId == userIdentifier && currentUserAchievement.AchievementId == achievementId))
            {
                return;
            }

            context.UserAchievements.Add(new  SteamHub.Api.Entities.UserAchievement
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
                        .FirstOrDefault(x => x.UserId == userIdentifier && x.AchievementId == achievementId);
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
                        .Include(x => x.Achievement)
                        .FirstOrDefault(x => x.UserId == userIdentifier && x.AchievementId == achievementId);
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
            var all = await GetAllAchievements();
            var unlockedIdList = await context.UserAchievements
                .Where(currentUserAchievement => currentUserAchievement.UserId == userIdentifier)
                .Select(userAchievement => userAchievement.AchievementId)
                .ToListAsync();

            var unlockedIds = new HashSet<int>(unlockedIdList);

            return all.Select(currentAchievement => new AchievementWithStatus
            {
                Achievement = currentAchievement,
                IsUnlocked = unlockedIds.Contains(currentAchievement.AchievementId),
                UnlockedDate = unlockedIds.Contains(currentAchievement.AchievementId)
                    ? context.UserAchievements.First(ua => ua.UserId == userIdentifier && ua.AchievementId == currentAchievement.AchievementId).UnlockedAt
                    : (DateTime?)null
            }).ToList();
        }


        public async Task<int> GetFriendshipCount(int userIdentifier)
            => await context.Friendships.CountAsync(f => f.UserId == userIdentifier);

        public async Task<int> GetNumberOfOwnedGames(int userIdentifier)
            => await context.OwnedGames.CountAsync(og => og.UserId == userIdentifier);

        public async Task<int> GetNumberOfSoldGames(int userIdentifier)
            => await context.SoldGames.CountAsync(sg => sg.UserId == userIdentifier);

        public async Task<int> GetNumberOfReviewsGiven(int userIdentifier)
            => await context.Reviews.CountAsync(r => r.UserIdentifier == userIdentifier);

        public async Task<int> GetNumberOfReviewsReceived(int userIdentifier)
            => await context.Reviews.CountAsync(r => r.GameIdentifier == userIdentifier); // adjust if you store receiver differently

        public async Task<int> GetNumberOfPosts(int userIdentifier)
            => await context.NewsPosts.CountAsync(p => p.AuthorId == userIdentifier);

        public async Task<int> GetYearsOfAcftivity(int userIdentifier)
        {
            var created = await context.Users
                             .Where(u => u.UserId == userIdentifier)
                             .Select(u => u.CreatedAt)
                             .SingleOrDefaultAsync(); // Fixed: Use SingleOrDefaultAsync instead of SingleOrDefault

            var years = DateTime.Now.Year - created.Year;
            if (DateTime.Now.DayOfYear < created.DayOfYear)
            {
                years--;
            }
            return years;
        }

        public async Task<int?> GetAchievementIdByName(string achievementName)
            => await context.Achievements
        .Where(a => a.AchievementName == achievementName)
        .Select(a => (int?)a.AchievementId)
        .SingleOrDefaultAsync();


        public async Task<bool> IsUserDeveloper(int userIdentifier)
        {
            return await context.Users
                .Where(u => u.UserId == userIdentifier)
                .Select(u => u.UserRole == SteamHub.ApiContract.Models.Common.UserRole.Developer)
                .SingleOrDefaultAsync(); // Fixed: Use SingleOrDefaultAsync instead of SingleOrDefault
        }
        }
        
}


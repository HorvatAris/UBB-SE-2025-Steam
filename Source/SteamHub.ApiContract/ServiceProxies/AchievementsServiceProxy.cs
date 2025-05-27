using System;
using System.Collections.Generic;
using BusinessLayer.Models;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Exceptions;
using static SteamHub.ApiContract.Services.AchievementsService;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class AchievementsServiceProxy : ServiceProxy, IAchievementsService
    {
        public AchievementsServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }

        public void InitializeAchievements()
        {
            try
            {
                PostAsync("Achievements/initialize", null).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error initializing achievements: {ex.Message}");
            }
        }

        public GroupedAchievementsResult GetGroupedAchievementsForUser(int userIdentifier)
        {
            try
            {
                return GetAsync<GroupedAchievementsResult>($"Achievements/{userIdentifier}/grouped").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error grouping achievements for user", ex);
            }
        }

        public List<Achievement> GetAchievementsForUser(int userIdentifier)
        {
            try
            {
                return GetAsync<List<Achievement>>($"Achievements/{userIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving achievements for user", ex);
            }
        }

        public void UnlockAchievementForUser(int userIdentifier)
        {
            try
            {
                PostAsync($"Achievements/{userIdentifier}/unlock", null).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unlocking achievement for user: {ex.Message}");
            }
        }

        public void RemoveAchievement(int userIdentifier, int achievementIdentifier)
        {
            try
            {
                DeleteAsync<object>($"Achievements/{userIdentifier}/{achievementIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error removing achievement", ex);
            }
        }

        public List<Achievement> GetUnlockedAchievementsForUser(int userIdentifier)
        {
            try
            {
                return GetAsync<List<Achievement>>($"Achievements/{userIdentifier}/unlocked").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving unlocked achievements for user", ex);
            }
        }

        public List<Achievement> GetAllAchievements()
        {
            try
            {
                return GetAsync<List<Achievement>>("Achievements").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving all achievements", ex);
            }
        }

        public AchievementUnlockedData GetUnlockedDataForAchievement(int userIdentifier, int achievementIdentifier)
        {
            try
            {
                return GetAsync<AchievementUnlockedData>($"Achievements/{userIdentifier}/{achievementIdentifier}/data").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving unlocked data for achievement", ex);
            }
        }

        public List<AchievementWithStatus> GetAchievementsWithStatusForUser(int userIdentifier)
        {
            try
            {
                return GetAsync<List<AchievementWithStatus>>($"Achievements/{userIdentifier}/status").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving achievements with status for user", ex);
            }
        }

        public int GetPointsForUnlockedAchievement(int userIdentifier, int achievementIdentifier)
        {
            try
            {
                return GetAsync<int>($"Achievements/{userIdentifier}/{achievementIdentifier}/points").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Error retrieving points for unlocked achievement", ex);
            }
        }
    }
}
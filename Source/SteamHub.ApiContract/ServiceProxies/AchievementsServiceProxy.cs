using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using static SteamHub.ApiContract.Services.AchievementsService;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class AchievementsServiceProxy : ServiceProxy, IAchievementsService
    {
        public AchievementsServiceProxy(string baseUrl = "https://localhost:7241/api/")
            : base(baseUrl)
        {
        }

        public async Task InitializeAchievements()
        {
            try
            {
                await PostAsync("Achievements/initialize", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing achievements: {ex.Message}");
            }
        }

        public async Task<Models.GroupedAchievementsResult> GetGroupedAchievementsForUser(int userIdentifier)
        {
            try
            {
                return await GetAsync<Models.GroupedAchievementsResult>($"Achievements/{userIdentifier}/grouped");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetGroupedAchievementsForUser: {ex}");
                throw new Exception("Error grouping achievements for user", ex);
            }
        }

        public async Task<List<Achievement>> GetAchievementsForUser(int userIdentifier)
        {
            try
            {
                return await GetAsync<List<Achievement>>($"Achievements/{userIdentifier}");
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving achievements for user", ex);
            }
        }

        public async Task UnlockAchievementForUser(int userIdentifier)
        {
            try
            {
                await PostAsync($"Achievements/{userIdentifier}/unlock", null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error unlocking achievement for user: {ex.Message}");
            }
        }

        public async Task<List<Achievement>> GetAllAchievements()
        {
            try
            {
                return await GetAsync<List<Achievement>>("Achievements");
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving all achievements", ex);
            }
        }

        public async Task<List<AchievementWithStatus>> GetAchievementsWithStatusForUser(int userIdentifier)
        {
            try
            {
                return await GetAsync<List<AchievementWithStatus>>($"Achievements/user/{userIdentifier}/status");
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving achievements with status for user", ex);
            }
        }

        public async Task<int> GetPointsForUnlockedAchievement(int userIdentifier, int achievementIdentifier)
        {
            try
            {
                return await GetAsync<int>($"Achievements/{userIdentifier}/{achievementIdentifier}/points");
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving points for unlocked achievement", ex);
            }
        }
    }
}
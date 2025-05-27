using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.Json;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using static SteamHub.ApiContract.Services.AchievementsService;
using Google.Apis.Http;
using System.Net.Http;
using System.Net.Http.Json;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class AchievementsServiceProxy : ServiceProxy, IAchievementsService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public AchievementsServiceProxy(System.Net.Http.IHttpClientFactory httpClientFactory,   string baseUrl = "https://localhost:7241/api/")
            : base(baseUrl)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }
       
        public async Task InitializeAchievements()
        {
            try
            {
                await _httpClient.PostAsync("Achievements/initialize", null);
                //PostAsync("Achievements/initialize", null).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error initializing achievements: {ex.Message}");
            }
        }

        public async Task<GroupedAchievementsResult> GetGroupedAchievementsForUser(int userIdentifier)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Achievements/{userIdentifier}/grouped");
                response.EnsureSuccessStatusCode();
                
                var jsonString = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"Response JSON: {jsonString}"); // Log the JSON response

                // Deserialize the entire response
                var result = JsonSerializer.Deserialize<GroupedAchievementsResult>(jsonString, _options);
                
                if (result == null)
                {
                    throw new Exception("Failed to deserialize achievements response");
                }

                return result;
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
                var response = await _httpClient.GetAsync($"Achievements/{userIdentifier}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<Achievement>>(_options);

                return result;
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
                await _httpClient.PostAsync($"Achievements/{userIdentifier}/unlock", null);
                //PostAsync($"Achievements/{userIdentifier}/unlock", null).GetAwaiter().GetResult();
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
                var response=await _httpClient.GetAsync("Achievements");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<Achievement>>(_options);
                return result;

                //return GetAsync<List<Achievement>>("Achievements").GetAwaiter().GetResult();
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
                var response = await _httpClient.GetAsync($"Achievements/{userIdentifier}/status");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<AchievementWithStatus>>(_options);
                return result;
                //return await GetAsync<List<AchievementWithStatus>>($"Achievements/{userIdentifier}/status");
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
                var response = await _httpClient.GetAsync($"Achievements/{userIdentifier}/{achievementIdentifier}/points");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<int>(_options);
                return result;
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving points for unlocked achievement", ex);
            }
        }

    }
}
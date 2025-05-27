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

        public async Task<GroupedAchievementsResult> GetGroupedAchievementsForUser(int userIdentifier)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/Achievements/{userIdentifier}/grouped");
                response.EnsureSuccessStatusCode();
                
                var jsonString = await response.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"Response JSON: {jsonString}"); // Log the JSON response
                using var document = JsonDocument.Parse(jsonString);
                var resultElement = document.RootElement.GetProperty("result");
                System.Diagnostics.Debug.WriteLine($"Response JSON: {resultElement}");

                var result = JsonSerializer.Deserialize<GroupedAchievementsResult>(
                    resultElement.GetRawText(), _options);

                return result!;
            }
            catch (Exception ex)
            {
                throw new Exception("Error grouping achievements for user", ex);
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
                throw new Exception("Error retrieving achievements for user", ex);
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
                throw new   Exception("Error removing achievement", ex);
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
                throw new Exception("Error retrieving unlocked achievements for user", ex);
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
                throw new Exception("Error retrieving all achievements", ex);
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
                throw new Exception("Error retrieving unlocked data for achievement", ex);
            }
        }

        public async Task<List<AchievementWithStatus>> GetAchievementsWithStatusForUser(int userIdentifier)
        {
            try
            {
                return await GetAsync<List<AchievementWithStatus>>($"Achievements/{userIdentifier}/status");
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving achievements with status for user", ex);
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
                throw new Exception("Error retrieving points for unlocked achievement", ex);
            }
        }
        public class ApiResponse<T>
        {
            public T Result { get; set; }
        }

    }
}
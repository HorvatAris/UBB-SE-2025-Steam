using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class FeaturesServiceProxy : ServiceProxy, IFeaturesService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://localhost:7241/api/";

        public FeaturesServiceProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(BaseUrl);
            _httpClient.DefaultRequestVersion = new Version(1, 1);
        }

        public async Task<Dictionary<string, List<Feature>>> GetFeaturesByCategoriesAsync(int userId)
        {
            try
            {
                return await GetAsync<Dictionary<string, List<Feature>>>(_httpClient, $"Features/user/{userId}/categories");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve features from server", ex);
            }
        }

        public async Task<List<Feature>> GetAllFeaturesAsync(int userId)
        {
            try
            {
                return await GetAsync<List<Feature>>(_httpClient, $"Features?userId={userId}");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve features from server", ex);
            }
        }

        public async Task<List<Feature>> GetFeaturesByTypeAsync(string type)
        {
            try
            {
                return await GetAsync<List<Feature>>(_httpClient, $"Features/type/{type}");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve features by type from server", ex);
            }
        }

        public async Task<List<Feature>> GetUserFeaturesAsync(int userId)
        {
            try
            {
                return await GetAsync<List<Feature>>(_httpClient, $"Features/user/{userId}");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve user features from server", ex);
            }
        }

        public async Task<bool> IsFeaturePurchasedAsync(int userId, int featureId)
        {
            try
            {
                return await GetAsync<bool>(_httpClient, $"Features/user/{userId}/purchased/{featureId}");
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> EquipFeatureAsync(int userId, int featureId)
        {
            try
            {
                return await PostAsync<bool>(_httpClient, "Features/equip", new { UserId = userId, FeatureId = featureId });
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UnequipFeatureAsync(int userId, int featureId)
        {
            try
            {
                var response = await PostAsync<FeatureResponse>(_httpClient, "Features/unequip", new { UserId = userId, FeatureId = featureId });
                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UnequipFeaturesByTypeAsync(int userId, string featureType)
        {
            try
            {
                var response = await PostAsync<FeatureResponse>(_httpClient, "Features/unequip-type", new { UserId = userId, FeatureType = featureType });
                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> AddUserFeatureAsync(int userId, int featureId)
        {
            try
            {
                var response = await PostAsync<FeatureResponse>(_httpClient, "Features/purchase", new { UserId = userId, FeatureId = featureId });
                return response.Success;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Feature>> GetEquippedFeaturesAsync(int userId)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Fetching equipped features for user ID: {userId}");
                var response = await GetAsync<List<Feature>>(_httpClient, $"Features/user/{userId}/equipped");
                System.Diagnostics.Debug.WriteLine($"Successfully retrieved equipped features for user ID: {userId}");
                return response;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching equipped features for user ID {userId}: {ex.Message}");
                throw new Exception("Failed to retrieve equipped features from server", ex);
            }
        }

        public async Task<Feature> GetFeatureByIdAsync(int featureId)
        {
            try
            {
                return await GetAsync<Feature>(_httpClient, $"Features/{featureId}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve feature by ID from server", ex);
            }
        }

        public async Task<(string profilePicturePath, string bioText, List<Feature> equippedFeatures)> GetFeaturePreviewDataAsync(int userId, int featureId)
        {
            try
            {
                var response = await GetAsync<FeaturePreviewResponse>(_httpClient, $"Features/user/{userId}/preview/{featureId}");
                return (response.ProfilePicturePath, response.BioText, response.EquippedFeatures);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve feature preview data from server", ex);
            }
        }

        // Helper methods to use HttpClient with ServiceProxy's async helpers
        private async Task<T> GetAsync<T>(HttpClient client, string endpoint)
        {
            System.Diagnostics.Debug.WriteLine($"Sending GET request to endpoint: {endpoint}");
            var response = await client.GetAsync(endpoint);
            System.Diagnostics.Debug.WriteLine($"Response status code: {response.StatusCode}");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine($"Response content: {json}");
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        private async Task<T> PostAsync<T>(HttpClient client, string endpoint, object data)
        {
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endpoint, content);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public class FeatureResponse
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public class FeaturePreviewResponse
        {
            public string ProfilePicturePath { get; set; }
            public string BioText { get; set; }
            public List<Feature> EquippedFeatures { get; set; }
        }
    }
} 




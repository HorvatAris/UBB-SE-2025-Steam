using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class FeaturesServiceProxy : ServiceProxy, IFeaturesService
    {

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public FeaturesServiceProxy(IHttpClientFactory httpClientFactory, string baseUrl = "https://localhost:7262/api/")
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }

        public async Task<Dictionary<string, List<Feature>>> GetFeaturesByCategoriesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Features/user/{userId}/categories");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<Dictionary<string, List<Feature>>>(_options);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetFeaturesByCategoriesAsync: {ex}");
                throw new Exception("Failed to retrieve features from server", ex);
            }
        }

        public async Task<List<Feature>> GetAllFeaturesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Features?userId={userId}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<Feature>>(_options);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetAllFeaturesAsync: {ex}");
                throw new Exception("Failed to retrieve features from server", ex);
            }
        }

        public async Task<List<Feature>> GetFeaturesByTypeAsync(string type)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Features/type/{type}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<Feature>>(_options);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetFeaturesByTypeAsync: {ex}");
                throw new Exception("Failed to retrieve features by type from server", ex);
            }
        }

        public async Task<List<Feature>> GetUserFeaturesAsync(int userId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Features/user/{userId}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<Feature>>(_options);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetUserFeaturesAsync: {ex}");
                throw new Exception("Failed to retrieve user features from server", ex);
            }
        }

        public async Task<bool> IsFeaturePurchasedAsync(int userId, int featureId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Features/user/{userId}/purchased/{featureId}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<bool>(_options);
                return result;
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
                var response = await _httpClient.PostAsJsonAsync("Features/equip", new { UserId = userId, FeatureId = featureId });
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<bool>(_options);
                return result;
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
                var response = await _httpClient.PostAsJsonAsync("Features/unequip", new { UserId = userId, FeatureId = featureId });
                response.EnsureSuccessStatusCode();
                var featureResponse = await response.Content.ReadFromJsonAsync<FeatureResponse>(_options);
                return featureResponse?.Success ?? false;
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
                var response = await _httpClient.PostAsJsonAsync("Features/unequip-type", new { UserId = userId, FeatureType = featureType });
                response.EnsureSuccessStatusCode();
                var featureResponse = await response.Content.ReadFromJsonAsync<FeatureResponse>(_options);
                return featureResponse?.Success ?? false;
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
                var response = await _httpClient.PostAsJsonAsync("Features/purchase", new { UserId = userId, FeatureId = featureId });
                response.EnsureSuccessStatusCode();
                var featureResponse = await response.Content.ReadFromJsonAsync<FeatureResponse>(_options);
                return featureResponse?.Success ?? false;
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
                var response = await _httpClient.GetAsync($"Features/user/{userId}/equipped");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<List<Feature>>(_options);
                System.Diagnostics.Debug.WriteLine($"Successfully retrieved equipped features for user ID: {userId}");
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error fetching equipped features for user ID {userId}: {ex}");
                throw new Exception("Failed to retrieve equipped features from server", ex);
            }
        }

        public async Task<Feature> GetFeatureByIdAsync(int featureId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Features/{featureId}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<Feature>(_options);
                return result;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetFeatureByIdAsync: {ex}");
                throw new Exception($"Failed to retrieve feature by ID from server", ex);
            }
        }

        public async Task<(string profilePicturePath, string bioText, List<Feature> equippedFeatures)> GetFeaturePreviewDataAsync(int userId, int featureId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"Features/user/{userId}/preview/{featureId}");
                response.EnsureSuccessStatusCode();
                var previewResponse = await response.Content.ReadFromJsonAsync<FeaturePreviewResponse>(_options);
                return (previewResponse.ProfilePicturePath, previewResponse.BioText, previewResponse.EquippedFeatures);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetFeaturePreviewDataAsync: {ex}");
                throw new Exception("Failed to retrieve feature preview data from server", ex);
            }
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





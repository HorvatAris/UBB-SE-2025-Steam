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
    public class FeaturesServiceProxy : IFeaturesService
    {
        private readonly HttpClient http_client;
        private readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public FeaturesServiceProxy(IHttpClientFactory httpClientFactory)
        {
            http_client = httpClientFactory.CreateClient("SteamHubApi");
        }

        public async Task<List<Feature>> GetAllFeaturesAsync(int userId)
        {
            try
            {
                var response = await http_client.GetAsync($"/api/Feature");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Feature>>(options) 
                    ?? new List<Feature>();
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
                var response = await http_client.GetAsync($"/api/Feature/type/{type}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Feature>>(options) 
                    ?? new List<Feature>();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve features by type from server", ex);
            }
        }

        public async Task<List<Feature>> GetUserFeaturesAsync(int userIdentifier)
        {
            try
            {
                var response = await http_client.GetAsync($"/api/Feature/user/{userIdentifier}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Feature>>(options) 
                    ?? new List<Feature>();
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
                var response = await http_client.GetAsync($"/api/Feature/user/{userId}/purchased/{featureId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>(options);
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
                var response = await http_client.PostAsJsonAsync("/api/Feature/equip", new
                {
                    UserId = userId,
                    FeatureId = featureId
                }, options);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<bool>(options);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UnequipFeatureAsync(int userIdentifier, int featureIdentifier)
        {
            try
            {
                var response = await http_client.PostAsJsonAsync("/api/Feature/unequip", new
                {
                    UserId = userIdentifier,
                    FeatureId = featureIdentifier
                }, options);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<FeatureResponse>(options);
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to unequip feature: {ex.Message}", ex);
            }
        }

        public async Task<bool> UnequipFeaturesByTypeAsync(int userIdentifier, string featureType)
        {
            try
            {
                var response = await http_client.PostAsJsonAsync("/api/Feature/unequip-type", new
                {
                    UserId = userIdentifier,
                    FeatureType = featureType
                }, options);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<FeatureResponse>(options);
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to unequip features by type: {ex.Message}", ex);
            }
        }

        public async Task<bool> AddUserFeatureAsync(int userIdentifier, int featureIdentifier)
        {
            try
            {
                var response = await http_client.PostAsJsonAsync("/api/Feature/purchase", new
                {
                    UserId = userIdentifier,
                    FeatureId = featureIdentifier
                }, options);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<FeatureResponse>(options);
                return result?.Success ?? false;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to purchase feature: {ex.Message}", ex);
            }
        }

        public async Task<List<Feature>> GetEquippedFeaturesAsync(int userId)
        {
            try
            {
                var response = await http_client.GetAsync($"/api/Feature/user/{userId}/equipped");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<List<Feature>>(options) 
                    ?? new List<Feature>();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve equipped features from server", ex);
            }
        }

        public async Task<Feature> GetFeatureByIdAsync(int featureId)
        {
            try
            {
                var response = await http_client.GetAsync($"/api/Feature/{featureId}");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Feature>(options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve feature with ID {featureId} from server", ex);
            }
        }

        public async Task<Dictionary<string, List<Feature>>> GetFeaturesByCategoriesAsync(int userId)
        {
            try
            {
                var response = await http_client.GetAsync($"/api/Feature/user/{userId}/categories");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<Dictionary<string, List<Feature>>>(options) 
                    ?? new Dictionary<string, List<Feature>>();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve features by categories from server", ex);
            }
        }

        public async Task<(string profilePicturePath, string bioText, List<Feature> equippedFeatures)> GetFeaturePreviewDataAsync(int userId, int featureId)
        {
            try
            {
                var response = await http_client.GetAsync($"/api/Feature/user/{userId}/preview/{featureId}");
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<FeaturePreviewResponse>(options);
                return (result?.ProfilePicturePath ?? "ms-appx:///Assets/default-profile.png",
                        result?.BioText ?? "No bio available",
                        result?.EquippedFeatures ?? new List<Feature>());
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve feature preview data from server", ex);
            }
        }
    }

    // Helper classes for feature responses
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

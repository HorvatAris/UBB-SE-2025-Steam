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
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };

        public FeaturesServiceProxy(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("SteamHubApi");
        }

        public async Task<List<Feature>> GetAllFeaturesAsync(int userId)
        {
            try
            {
                var url = $"/api/Features?userId={userId}";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve features from server. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                return JsonSerializer.Deserialize<List<Feature>>(content, _options) ?? new List<Feature>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve features from server: {ex.Message}", ex);
            }
        }

        public async Task<List<Feature>> GetFeaturesByTypeAsync(string type)
        {
            try
            {
                var url = $"/api/Features/type/{type}";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve features by type from server. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                return JsonSerializer.Deserialize<List<Feature>>(content, _options) ?? new List<Feature>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve features by type from server: {ex.Message}", ex);
            }
        }

        public async Task<List<Feature>> GetUserFeaturesAsync(int userIdentifier)
        {
            try
            {
                var url = $"/api/Features/user/{userIdentifier}";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve user features from server. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                return JsonSerializer.Deserialize<List<Feature>>(content, _options) ?? new List<Feature>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve user features from server: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsFeaturePurchasedAsync(int userId, int featureId)
        {
            try
            {
                var url = $"/api/Features/user/{userId}/purchased/{featureId}";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to check if feature is purchased. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                return JsonSerializer.Deserialize<bool>(content, _options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to check if feature is purchased: {ex.Message}", ex);
            }
        }

        public async Task<bool> EquipFeatureAsync(int userId, int featureId)
        {
            try
            {
                var url = "/api/Features/equip";
                var payload = new { UserId = userId, FeatureId = featureId };
                var response = await _httpClient.PostAsJsonAsync(url, payload, _options);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to equip feature. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                return JsonSerializer.Deserialize<bool>(content, _options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to equip feature: {ex.Message}", ex);
            }
        }

        public async Task<bool> UnequipFeatureAsync(int userIdentifier, int featureIdentifier)
        {
            try
            {
                var url = "/api/Features/unequip";
                var payload = new { UserId = userIdentifier, FeatureId = featureIdentifier };
                var response = await _httpClient.PostAsJsonAsync(url, payload, _options);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to unequip feature. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                var result = JsonSerializer.Deserialize<FeatureResponse>(content, _options);
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
                var url = "/api/Features/unequip-type";
                var payload = new { UserId = userIdentifier, FeatureType = featureType };
                var response = await _httpClient.PostAsJsonAsync(url, payload, _options);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to unequip features by type. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                var result = JsonSerializer.Deserialize<FeatureResponse>(content, _options);
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
                var url = "/api/Features/purchase";
                var payload = new { UserId = userIdentifier, FeatureId = featureIdentifier };
                var response = await _httpClient.PostAsJsonAsync(url, payload, _options);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to purchase feature. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                var result = JsonSerializer.Deserialize<FeatureResponse>(content, _options);
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
                var url = $"/api/Features/user/{userId}/equipped";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve equipped features from server. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                return JsonSerializer.Deserialize<List<Feature>>(content, _options) ?? new List<Feature>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve equipped features from server: {ex.Message}", ex);
            }
        }

        public async Task<Feature> GetFeatureByIdAsync(int featureId)
        {
            try
            {
                var url = $"/api/Features/{featureId}";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve feature by ID from server. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                return JsonSerializer.Deserialize<Feature>(content, _options);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve feature by ID from server: {ex.Message}", ex);
            }
        }

        public async Task<Dictionary<string, List<Feature>>> GetFeaturesByCategoriesAsync(int userId)
        {
            try
            {
                var url = $"/api/Features/user/{userId}/categories";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve features by categories from server. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                return JsonSerializer.Deserialize<Dictionary<string, List<Feature>>>(content, _options) ?? new Dictionary<string, List<Feature>>();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve features by categories from server: {ex.Message}", ex);
            }
        }

        public async Task<(string profilePicturePath, string bioText, List<Feature> equippedFeatures)> GetFeaturePreviewDataAsync(int userId, int featureId)
        {
            try
            {
                var url = $"/api/Features/user/{userId}/preview/{featureId}";
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to retrieve feature preview data from server. URL: {url}, Status: {response.StatusCode}, Content: {content}");
                }
                var result = JsonSerializer.Deserialize<FeaturePreviewResponse>(content, _options);
                return (result?.ProfilePicturePath ?? "ms-appx:///Assets/default-profile.png",
                        result?.BioText ?? "No bio available",
                        result?.EquippedFeatures ?? new List<Feature>());
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to retrieve feature preview data from server: {ex.Message}", ex);
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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class FeaturesServiceProxy : ServiceProxy, IFeaturesService
    {
        public FeaturesServiceProxy(string baseUrl = "https://localhost:7241/api/")
            : base(baseUrl)
        {
        }

        public async Task<List<Feature>> GetAllFeaturesAsync(int userId)
        {
            try
            {
                return await GetAsync<List<Feature>>($"Features?userId={userId}");
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
                return await GetAsync<List<Feature>>($"Features/type/{type}");
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
                return await GetAsync<List<Feature>>($"Features/user/{userId}");
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
                return await GetAsync<bool>($"Features/user/{userId}/purchased/{featureId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in IsFeaturePurchasedAsync: {ex}");
                return false;
            }
        }

        public async Task<bool> EquipFeatureAsync(int userId, int featureId)
        {
            try
            {
                return await PostAsync<bool>("Features/equip", new
                {
                    UserId = userId,
                    FeatureId = featureId
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in EquipFeatureAsync: {ex}");
                return false;
            }
        }

        public async Task<bool> UnequipFeatureAsync(int userId, int featureId)
        {
            try
            {
                return await PostAsync<bool>("Features/unequip", new
                {
                    UserId = userId,
                    FeatureId = featureId
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UnequipFeatureAsync: {ex}");
                return false;
            }
        }

        public async Task<bool> UnequipFeaturesByTypeAsync(int userId, string featureType)
        {
            try
            {
                return await PostAsync<bool>("Features/unequip-by-type", new
                {
                    UserId = userId,
                    FeatureType = featureType
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in UnequipFeaturesByTypeAsync: {ex}");
                return false;
            }
        }

        public async Task<bool> AddUserFeatureAsync(int userId, int featureId)
        {
            try
            {
                return await PostAsync<bool>("Features/add", new
                {
                    UserId = userId,
                    FeatureId = featureId
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in AddUserFeatureAsync: {ex}");
                return false;
            }
        }

        public async Task<List<Feature>> GetEquippedFeaturesAsync(int userId)
        {
            try
            {
                return await GetAsync<List<Feature>>($"Features/user/{userId}/equipped");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetEquippedFeaturesAsync: {ex}");
                throw new Exception("Failed to retrieve equipped features from server", ex);
            }
        }

        public async Task<Feature> GetFeatureByIdAsync(int featureId)
        {
            try
            {
                return await GetAsync<Feature>($"Features/{featureId}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetFeatureByIdAsync: {ex}");
                throw new Exception($"Failed to retrieve feature with ID {featureId} from server", ex);
            }
        }

        public async Task<Dictionary<string, List<Feature>>> GetFeaturesByCategoriesAsync(int userId)
        {
            try
            {
                return await GetAsync<Dictionary<string, List<Feature>>>($"Features/user/{userId}/categories");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetFeaturesByCategoriesAsync: {ex}");
                throw new Exception("Failed to retrieve features by categories from server", ex);
            }
        }

        public async Task<(string profilePicturePath, string bioText, List<Feature> equippedFeatures)> GetFeaturePreviewDataAsync(int userId, int featureId)
        {
            try
            {
                var response = await GetAsync<FeaturePreviewResponse>($"Features/user/{userId}/preview/{featureId}");
                return (response.ProfilePicturePath, response.BioText, response.EquippedFeatures);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in GetFeaturePreviewDataAsync: {ex}");
                throw new Exception("Failed to retrieve feature preview data from server", ex);
            }
        }
    }

    // Helper class for feature preview response
    public class FeaturePreviewResponse
    {
        public string ProfilePicturePath { get; set; }
        public string BioText { get; set; }
        public List<Feature> EquippedFeatures { get; set; }
    }
} 





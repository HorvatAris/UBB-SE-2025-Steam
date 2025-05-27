using System;
using System.Collections.Generic;
using BusinessLayer.Models;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class FeaturesServiceProxy : ServiceProxy, IFeaturesService
    {
        private readonly IUserService userService;

        public FeaturesServiceProxy(IUserService userService, string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public IUserService UserService => userService;

        public Dictionary<string, List<Feature>> GetFeaturesByCategories()
        {
            try
            {
                return GetAsync<Dictionary<string, List<Feature>>>("Feature").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve features from server", ex);
            }
        }

        public Dictionary<string, List<Feature>> GetFeaturesByCategories(int userId)
        {
            try
            {
                return GetAsync<Dictionary<string, List<Feature>>>($"Feature/user/{userId}/categories").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve features from server", ex);
            }
        }

        public bool EquipFeature(int userIdentifier, int featureIdentifier)
        {
            try
            {
                return PostAsync<bool>("Feature/equip", new
                {
                    UserId = userIdentifier,
                    FeatureId = featureIdentifier
                }).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public (bool, string) UnequipFeature(int userIdentifier, int featureIdentifier)
        {
            try
            {
                var response = PostAsync<FeatureResponse>("Feature/unequip", new
                {
                    UserId = userIdentifier,
                    FeatureId = featureIdentifier
                }).GetAwaiter().GetResult();

                return (response.Success, response.Message);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to unequip feature: {ex.Message}");
            }
        }

        public List<Feature> GetUserEquippedFeatures(int userIdentifier)
        {
            try
            {
                return GetAsync<List<Feature>>($"Feature/user/{userIdentifier}/equipped").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve equipped features from server", ex);
            }
        }

        public bool IsFeaturePurchased(int userIdentifier, int featureIdentifier)
        {
            try
            {
                return GetAsync<bool>($"Feature/user/{userIdentifier}/purchased/{featureIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                return false;
            }
        }

        public (bool success, string message) PurchaseFeature(int userIdentifier, int featureIdentifier)
        {
            try
            {
                var response = PostAsync<FeatureResponse>("Feature/purchase", new
                {
                    UserId = userIdentifier,
                    FeatureId = featureIdentifier
                }).GetAwaiter().GetResult();

                return (response.Success, response.Message);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to purchase feature: {ex.Message}");
            }
        }

        public (string profilePicturePath, string bioText, List<Feature> equippedFeatures) GetFeaturePreviewData(int userIdentifier, int featureIdentifier)
        {
            try
            {
                var response = GetAsync<FeaturePreviewResponse>($"Feature/user/{userIdentifier}/preview/{featureIdentifier}").GetAwaiter().GetResult();

                return (response.ProfilePicturePath, response.BioText, response.EquippedFeatures);
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve feature preview data from server", ex);
            }
        }

        public List<Feature> GetUserFeatures(int userIdentifier)
        {
            try
            {
                return GetAsync<List<Feature>>($"Feature/user/{userIdentifier}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException("Failed to retrieve user features from server", ex);
            }
        }

        public Feature GetFeatureById(int featureId)
        {
            try
            {
                return GetAsync<Feature>($"Feature/{featureId}").GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to retrieve feature with ID {featureId} from server", ex);
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
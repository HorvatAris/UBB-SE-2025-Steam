using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Validators;

namespace SteamHub.ApiContract.Services
{
    public class FeaturesService : IFeaturesService
    {
        private readonly IFeaturesRepository featuresRepository;
        private readonly IUserService userService;
        private readonly IPointShopService walletService;

        public FeaturesService(
            IFeaturesRepository featuresRepository,
            IUserService userService,
            IPointShopService walletService)
        {
            this.featuresRepository = featuresRepository;
            this.userService = userService;
            this.walletService = walletService;
        }

        public async Task<List<Feature>> GetAllFeaturesAsync(int userId)
        {
            var features = await featuresRepository.GetAllFeaturesAsync(userId);
            foreach (var feature in features)
            {
                var validationResult = FeaturesValidator.ValidateFeature(feature);
                if (!validationResult.isValid)
                {
                    throw new InvalidOperationException(validationResult.errorMessage);
                }
            }
            return features;
        }

        public async Task<List<Feature>> GetFeaturesByTypeAsync(string type)
        {
            var validationResult = FeaturesValidator.ValidateFeatureType(type);
            if (!validationResult.isValid)
            {
                throw new InvalidOperationException(validationResult.errorMessage);
            }

            return await featuresRepository.GetFeaturesByTypeAsync(type);
        }

        public async Task<List<Feature>> GetUserFeaturesAsync(int userIdentifier)
        {
            try
            {
                var features = await featuresRepository.GetUserFeaturesAsync(userIdentifier);
                foreach (var feature in features)
                {
                    var validationResult = FeaturesValidator.ValidateFeature(feature);
                    if (!validationResult.isValid)
                    {
                        throw new InvalidOperationException(validationResult.errorMessage);
                    }
                }
                return features;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to retrieve features for user {userIdentifier}.", ex);
            }
        }

        public async Task<bool> IsFeaturePurchasedAsync(int userId, int featureId)
        {
            return await featuresRepository.IsFeaturePurchasedAsync(userId, featureId);
        }

        public async Task<bool> EquipFeatureAsync(int userId, int featureId)
        {
            var isPurchased = await IsFeaturePurchasedAsync(userId, featureId);
            var validationResult = FeaturesValidator.ValidateFeatureEquip(userId, featureId, isPurchased);
            if (!validationResult.isValid)
            {
                throw new InvalidOperationException(validationResult.errorMessage);
            }

            return await featuresRepository.EquipFeatureAsync(userId, featureId);
        }

        public async Task<bool> UnequipFeatureAsync(int userIdentifier, int featureIdentifier)
        {
            if (!await IsFeaturePurchasedAsync(userIdentifier, featureIdentifier))
            {
                throw new InvalidOperationException("Feature not purchased");
            }

            return await featuresRepository.UnequipFeatureAsync(userIdentifier, featureIdentifier);
        }

        public async Task<bool> UnequipFeaturesByTypeAsync(int userIdentifier, string featureType)
        {
            var validationResult = FeaturesValidator.ValidateFeatureType(featureType);
            if (!validationResult.isValid)
            {
                throw new InvalidOperationException(validationResult.errorMessage);
            }

            return await featuresRepository.UnequipFeaturesByTypeAsync(userIdentifier, featureType);
        }

        public async Task<bool> AddUserFeatureAsync(int userIdentifier, int featureIdentifier)
        {
            if (await IsFeaturePurchasedAsync(userIdentifier, featureIdentifier))
            {
                throw new InvalidOperationException("Feature is already purchased.");
            }

            var feature = await GetFeatureByIdAsync(featureIdentifier);
            if (feature == null)
            {
                throw new InvalidOperationException("Feature not found.");
            }

            var validationResult = FeaturesValidator.ValidateFeature(feature);
            if (!validationResult.isValid)
            {
                throw new InvalidOperationException(validationResult.errorMessage);
            }

            var user = userService.GetUserByIdentifierAsync(userIdentifier);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            var balance = 0;// await walletService.GetBalanceAsync();
            if (balance < feature.Value)
            {
                throw new InvalidOperationException("Insufficient funds to purchase this feature.");
            }

            //await walletService.BuyWithMoneyAsync(feature.Value, userIdentifier);
            return await featuresRepository.AddUserFeatureAsync(userIdentifier, featureIdentifier);
        }

        public async Task<List<Feature>> GetEquippedFeaturesAsync(int userId)
        {
            return await featuresRepository.GetEquippedFeaturesAsync(userId);
        }

        public async Task<Feature> GetFeatureByIdAsync(int featureId)
        {
            if (featureId <= 0)
            {
                throw new InvalidOperationException("Invalid feature ID.");
            }

            var feature = await featuresRepository.GetFeatureByIdAsync(featureId);
            if (feature == null)
            {
                throw new InvalidOperationException($"Feature with ID {featureId} not found.");
            }

            var validationResult = FeaturesValidator.ValidateFeature(feature);
            if (!validationResult.isValid)
            {
                throw new InvalidOperationException(validationResult.errorMessage);
            }

            return feature;
        }

        public async Task<Dictionary<string, List<Feature>>> GetFeaturesByCategoriesAsync(int userId)
        {
            var categories = new Dictionary<string, List<Feature>>();
            var allFeatures = await GetAllFeaturesAsync(userId);

            foreach (var feature in allFeatures)
            {
                if (!categories.ContainsKey(feature.Type))
                {
                    categories[feature.Type] = new List<Feature>();
                }
                categories[feature.Type].Add(feature);
            }

            return categories;
        }

        public async Task<(string profilePicturePath, string bioText, List<Feature> equippedFeatures)> GetFeaturePreviewDataAsync(int userId, int featureId)
        {
            var equippedFeatures = await GetEquippedFeaturesAsync(userId);

            string profilePicturePath = "ms-appx:///Assets/default-profile.png";
            string bioText = "No bio available";

            return (profilePicturePath, bioText, equippedFeatures);
        }
    }
} 
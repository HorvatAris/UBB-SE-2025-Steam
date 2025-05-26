using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Validators;

namespace BusinessLayer.Services
{
    public class FeaturesService : IFeaturesService
    {
        private readonly IFeaturesRepository featuresRepository;
        private readonly IUserService userService;
        private readonly IUserProfilesRepository userProfilesRepository;
        private readonly IWalletService walletService;

        public FeaturesService(IFeaturesRepository featuresRepository, IUserService userService, IUserProfilesRepository userProfilesRepository, IWalletService walletService)
        {
            this.featuresRepository = featuresRepository;
            this.userService = userService;
            this.userProfilesRepository = userProfilesRepository;
            this.walletService = walletService;
        }

        public IUserService UserService => userService;

        public Dictionary<string, List<Feature>> GetFeaturesByCategories()
        {
            var categories = new Dictionary<string, List<Feature>>();
            var currentUser = userService.GetCurrentUser();
            if (currentUser == null)
            {
                throw new InvalidOperationException("No user is currently logged in.");
            }
            var allFeatures = featuresRepository.GetAllFeatures(currentUser.UserId);

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

        public Dictionary<string, List<Feature>> GetFeaturesByCategories(int userId)
        {
            var categories = new Dictionary<string, List<Feature>>();
            var allFeatures = featuresRepository.GetAllFeatures(userId);
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

        public bool EquipFeature(int userId, int featureId)
        {
            return featuresRepository.EquipFeature(userId, featureId);
        }

        public (bool, string) UnequipFeature(int userId, int featureId)
        {
            if (!featuresRepository.IsFeaturePurchased(userId, featureId))
            {
                return (false, "Feature not purchased");
            }

            bool success = featuresRepository.UnequipFeature(userId, featureId);
            return (success, success ? "Feature unequipped successfully" : "Failed to unequip feature");
        }

        public List<Feature> GetUserEquippedFeatures(int userId)
        {
            return featuresRepository.GetEquippedFeatures(userId)
                .ToList();
        }

        public bool IsFeaturePurchased(int userId, int featureId)
        {
            return featuresRepository.IsFeaturePurchased(userId, featureId);
        }

        public (bool success, string message) PurchaseFeature(int userId, int featureId)
        {
            try
            {
                if (userId <= 0)
                {
                    return (false, "Invalid user ID.");
                }

                if (featureId <= 0)
                {
                    return (false, "Invalid feature ID.");
                }

                // Check if feature already purchased
                if (featuresRepository.IsFeaturePurchased(userId, featureId))
                {
                    return (false, "Feature is already purchased.");
                }

                // Get the feature to validate and check price
                var feature = featuresRepository.GetAllFeatures(userId)
                    .FirstOrDefault(currentFeature => currentFeature.FeatureId == featureId);

                if (feature == null)
                {
                    return (false, "Feature not found.");
                }

                var validationResult = FeaturesValidator.ValidateFeature(feature);
                if (!validationResult.isValid)
                {
                    return (false, validationResult.errorMessage);
                }

                var user = userService.GetUserByIdentifier(userId);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                var balance = walletService.GetBalance();
                if (balance < feature.Value)
                {
                    return (false, "Insufficient funds to purchase this feature.");
                }

                walletService.BuyWithMoney(feature.Value, userId);

                featuresRepository.AddUserFeature(userId, featureId);
                return (true, $"Successfully purchased {feature.Name} for {feature.Value} points.");
            }
            catch
            {
                return (false, "An error occurred while processing your purchase. Please try again later.");
            }
        }

        public (string profilePicturePath, string bioText, List<Feature> equippedFeatures) GetFeaturePreviewData(int userId, int featureId)
        {
            var equippedFeatures = GetUserEquippedFeatures(userId);

            string profilePicturePath = "ms-appx:///Assets/default-profile.png";
            string bioText = "No bio available";

            try
            {
                // Replace the problematic line with the properly injected repository
                var userProfile = userProfilesRepository.GetUserProfileByUserId(userId);

                if (userProfile != null)
                {
                    if (!string.IsNullOrEmpty(userProfile.ProfilePicture))
                    {
                        profilePicturePath = userProfile.ProfilePicture;
                        if (!profilePicturePath.StartsWith("ms-appx:///"))
                        {
                            profilePicturePath = $"ms-appx:///{profilePicturePath}";
                        }
                    }
                    if (!string.IsNullOrEmpty(userProfile.Bio))
                    {
                        bioText = userProfile.Bio;
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to get user equipped Features", exception);
            }
            return (profilePicturePath, bioText, equippedFeatures);
        }

        public List<Feature> GetUserFeatures(int userIdentifier)
        {
            try
            {
                var features = featuresRepository.GetUserFeatures(userIdentifier);
                foreach (var feature in features)
                {
                    var validationResult = FeaturesValidator.ValidateFeature(feature);
                    if (!validationResult.isValid)
                    {
                        throw new ValidationException(validationResult.errorMessage);
                    }
                }
                return features;
            }
            catch (DatabaseOperationException exception)
            {
                throw new DatabaseOperationException($"Failed to retrieve features for user {userIdentifier}.", exception);
            }
        }

        public Feature GetFeatureById(int featureId)
        {
            if (featureId <= 0)
            {
                throw new ArgumentException("Invalid feature ID.", nameof(featureId));
            }

            var feature = featuresRepository.GetFeatureById(featureId);
            if (feature == null)
            {
                throw new InvalidOperationException($"Feature with ID {featureId} not found.");
            }

            var (isValid, errorMessage) = FeaturesValidator.ValidateFeature(feature);
            if (!isValid)
            {
                throw new InvalidOperationException(errorMessage);
            }

            return feature;
        }

        public FeaturesRepository GetRepository()
        {
            return featuresRepository as FeaturesRepository;
        }
    }
}

using System;
using BusinessLayer.Models;

namespace BusinessLayer.Validators
{
    public static class FeaturesValidator
    {
        public static (bool isValid, string errorMessage) ValidateFeature(Feature feature)
        {
            if (feature == null)
            {
                return (false, "Feature cannot be null.");
            }

            if (string.IsNullOrWhiteSpace(feature.Name))
            {
                return (false, "Feature name cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(feature.Type))
            {
                return (false, "Feature type cannot be empty.");
            }

            if (feature.Value < 0)
            {
                return (false, "Feature value cannot be negative.");
            }

            return (true, string.Empty);
        }

        public static (bool isValid, string errorMessage) ValidateFeatureType(string type)
        {
            if (string.IsNullOrWhiteSpace(type))
            {
                return (false, "Feature type cannot be empty.");
            }

            string[] validTypes = { "frame", "emoji", "background", "pet", "hat" };
            if (!Array.Exists(validTypes, currentType => currentType.Equals(type, StringComparison.OrdinalIgnoreCase)))
            {
                return (false, "Invalid feature type.");
            }

            return (true, string.Empty);
        }

        public static (bool isValid, string errorMessage) ValidateFeatureEquip(int userId, int featureId, bool isPurchased)
        {
            if (userId <= 0)
            {
                return (false, "Invalid user ID.");
            }

            if (featureId <= 0)
            {
                return (false, "Invalid feature ID.");
            }

            if (!isPurchased)
            {
                return (false, "Feature is not purchased by the user.");
            }

            return (true, string.Empty);
        }
    }
}
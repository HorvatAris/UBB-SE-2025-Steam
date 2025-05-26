using System.Collections.Generic;
using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IFeaturesService
    {
        Dictionary<string, List<Feature>> GetFeaturesByCategories();
        Dictionary<string, List<Feature>> GetFeaturesByCategories(int userId);
        bool EquipFeature(int userIdentifier, int featureIdentifier);
        (bool, string) UnequipFeature(int userIdentifier, int featureIdentifier);
        List<Feature> GetUserEquippedFeatures(int userIdentifier);
        bool IsFeaturePurchased(int userIdentifier, int featureIdentifier);
        (bool success, string message) PurchaseFeature(int userIdentifier, int featureIdentifier);
        (string profilePicturePath, string bioText, List<Feature> equippedFeatures) GetFeaturePreviewData(int userIdentifier, int featureIdentifier);

        IUserService UserService { get; }

        List<Feature> GetUserFeatures(int userIdentifier);

        Feature GetFeatureById(int featureId);
    }
}
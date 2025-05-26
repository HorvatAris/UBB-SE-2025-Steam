using System.Collections.Generic;
using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IFeaturesRepository
    {
        List<Feature> GetAllFeatures(int userId);
        List<Feature> GetFeaturesByType(string type);
        List<Feature> GetUserFeatures(int userId);
        bool IsFeaturePurchased(int userId, int featureId);
        bool EquipFeature(int userId, int featureId);
        bool UnequipFeature(int userId, int featureId);
        bool UnequipFeaturesByType(int userId, string featureType);
        bool AddUserFeature(int userId, int featureId);
    }
}
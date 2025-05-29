using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface IFeaturesService
    {
        Task<List<Feature>> GetAllFeaturesAsync(int userId);
        Task<List<Feature>> GetFeaturesByTypeAsync(string type);
        Task<List<Feature>> GetUserFeaturesAsync(int userIdentifier);
        Task<bool> IsFeaturePurchasedAsync(int userId, int featureId);
        Task<bool> EquipFeatureAsync(int userId, int featureId);
        Task<bool> UnequipFeatureAsync(int userIdentifier, int featureIdentifier);
        Task<bool> UnequipFeaturesByTypeAsync(int userIdentifier, string featureType);
        Task<bool> AddUserFeatureAsync(int userIdentifier, int featureIdentifier);
        Task<List<Feature>> GetEquippedFeaturesAsync(int userId);
        Task<Feature> GetFeatureByIdAsync(int featureId);
        Task<Dictionary<string, List<Feature>>> GetFeaturesByCategoriesAsync(int userId);
        Task<(string profilePicturePath, string bioText, List<Feature> equippedFeatures)> GetFeaturePreviewDataAsync(int userId, int featureId);
    }
} 

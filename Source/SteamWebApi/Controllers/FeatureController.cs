using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeatureController : ControllerBase
    {
        private readonly IFeaturesService featuresService;

        public FeatureController(IFeaturesService featuresService)
        {
            this.featuresService = featuresService;
        }

        [HttpGet]
        public IActionResult GetFeatures()
        {
            var featuresByCategories = featuresService.GetFeaturesByCategories();
            return Ok(featuresByCategories);
        }

        [HttpGet("user/{userId}")]
        public IActionResult GetUserFeatures(int userId)
        {
            var features = featuresService.GetUserFeatures(userId);
            return Ok(features);
        }

        [HttpGet("user/{userId}/equipped")]
        public IActionResult GetUserEquippedFeatures(int userId)
        {
            var features = featuresService.GetUserEquippedFeatures(userId);
            return Ok(features);
        }

        [HttpGet("user/{userId}/purchased/{featureId}")]
        public IActionResult IsFeaturePurchased(int userId, int featureId)
        {
            var isPurchased = featuresService.IsFeaturePurchased(userId, featureId);
            return Ok(isPurchased);
        }

        [HttpGet("user/{userId}/preview/{featureId}")]
        public IActionResult GetFeaturePreviewData(int userId, int featureId)
        {
            var (profilePicturePath, bioText, equippedFeatures) = featuresService.GetFeaturePreviewData(userId, featureId);

            return Ok(new
            {
                ProfilePicturePath = profilePicturePath,
                BioText = bioText,
                EquippedFeatures = equippedFeatures
            });
        }

        [HttpPost("equip")]
        public IActionResult EquipFeature([FromBody] FeatureActionRequest request)
        {
            var success = featuresService.EquipFeature(request.UserId, request.FeatureId);
            return Ok(new { Success = success, Message = success ? "Feature equipped successfully." : "Failed to equip feature." });
        }

        [HttpPost("unequip")]
        public IActionResult UnequipFeature([FromBody] FeatureActionRequest request)
        {
            var (success, message) = featuresService.UnequipFeature(request.UserId, request.FeatureId);
            return Ok(new { Success = success, Message = message });
        }

        [HttpPost("purchase")]
        public IActionResult PurchaseFeature([FromBody] FeatureActionRequest request)
        {
            var (success, message) = featuresService.PurchaseFeature(request.UserId, request.FeatureId);
            return Ok(new { Success = success, Message = message });
        }

        [HttpGet("user/{userId}/categories")]
        public IActionResult GetFeaturesByCategoriesForUser(int userId)
        {
            var featuresByCategories = featuresService.GetFeaturesByCategories(userId);
            return Ok(featuresByCategories);
        }
    }

    public class FeatureActionRequest
    {
        public int UserId { get; set; }
        public int FeatureId { get; set; }
    }
}
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FeaturesController : ControllerBase
    {
        private readonly IFeaturesService featuresService;

        public FeaturesController(IFeaturesService featuresService)
        {
            this.featuresService = featuresService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int userId)
        {
            var result = await featuresService.GetAllFeaturesAsync(userId);
            return Ok(result);
        }

        [HttpGet("type/{type}")]
        public async Task<IActionResult> GetByType(string type)
        {
            var result = await featuresService.GetFeaturesByTypeAsync(type);
            return Ok(result);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserFeatures(int userId)
        {
            var result = await featuresService.GetUserFeaturesAsync(userId);
            return Ok(result);
        }

        [HttpGet("user/{userId}/purchased/{featureId}")]
        public async Task<IActionResult> IsFeaturePurchased(int userId, int featureId)
        {
            var result = await featuresService.IsFeaturePurchasedAsync(userId, featureId);
            return Ok(result);
        }

        [HttpPost("equip")]
        public async Task<IActionResult> EquipFeature([FromBody] EquipRequest request)
        {
            var result = await featuresService.EquipFeatureAsync(request.UserId, request.FeatureId);
            return Ok(result);
        }

        [HttpPost("unequip")]
        public async Task<IActionResult> UnequipFeature([FromBody] EquipRequest request)
        {
            var result = await featuresService.UnequipFeatureAsync(request.UserId, request.FeatureId);
            return Ok(result);
        }

        [HttpPost("unequip-type")]
        public async Task<IActionResult> UnequipFeaturesByType([FromBody] UnequipTypeRequest request)
        {
            var result = await featuresService.UnequipFeaturesByTypeAsync(request.UserId, request.FeatureType);
            return Ok(result);
        }

        [HttpPost("purchase")]
        public async Task<IActionResult> AddUserFeature([FromBody] EquipRequest request)
        {
            var result = await featuresService.AddUserFeatureAsync(request.UserId, request.FeatureId);
            return Ok(result);
        }

        [HttpGet("user/{userId}/equipped")]
        public async Task<IActionResult> GetEquippedFeatures(int userId)
        {
            var result = await featuresService.GetEquippedFeaturesAsync(userId);
            return Ok(result);
        }

        [HttpGet("{featureId}")]
        public async Task<IActionResult> GetFeatureById(int featureId)
        {
            var result = await featuresService.GetFeatureByIdAsync(featureId);
            return Ok(result);
        }

        [HttpGet("user/{userId}/categories")]
        public async Task<IActionResult> GetFeaturesByCategories(int userId)
        {
            var result = await featuresService.GetFeaturesByCategoriesAsync(userId);
            return Ok(result);
        }

        [HttpGet("user/{userId}/preview/{featureId}")]
        public async Task<IActionResult> GetFeaturePreviewData(int userId, int featureId)
        {
            var result = await featuresService.GetFeaturePreviewDataAsync(userId, featureId);
            return Ok(result);
        }

        public class EquipRequest
        {
            public int UserId { get; set; }
            public int FeatureId { get; set; }
        }

        public class UnequipTypeRequest
        {
            public int UserId { get; set; }
            public string FeatureType { get; set; }
        }
    }
} 
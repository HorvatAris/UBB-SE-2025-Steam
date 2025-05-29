using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

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
        public async Task<ActionResult<List<Feature>>> GetAll([FromQuery] int userId)
        {
            try
            {
                var result = await featuresService.GetAllFeaturesAsync(userId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("type/{type}")]
        public async Task<ActionResult<List<Feature>>> GetByType(string type)
        {
            try
            {
                var result = await featuresService.GetFeaturesByTypeAsync(type);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<Feature>>> GetUserFeatures(int userId)
        {
            try
            {
                var result = await featuresService.GetUserFeaturesAsync(userId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}/purchased/{featureId}")]
        public async Task<ActionResult<bool>> IsFeaturePurchased(int userId, int featureId)
        {
            try
            {
                var result = await featuresService.IsFeaturePurchasedAsync(userId, featureId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("equip")]
        public async Task<ActionResult<bool>> EquipFeature([FromBody] EquipRequest request)
        {
            try
            {
                var result = await featuresService.EquipFeatureAsync(request.UserId, request.FeatureId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("unequip")]
        public async Task<ActionResult<bool>> UnequipFeature([FromBody] EquipRequest request)
        {
            try
            {
                var result = await featuresService.UnequipFeatureAsync(request.UserId, request.FeatureId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("unequip-type")]
        public async Task<ActionResult<bool>> UnequipFeaturesByType([FromBody] UnequipTypeRequest request)
        {
            try
            {
                var result = await featuresService.UnequipFeaturesByTypeAsync(request.UserId, request.FeatureType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("purchase")]
        public async Task<ActionResult<bool>> AddUserFeature([FromBody] EquipRequest request)
        {
            try
            {
                var result = await featuresService.AddUserFeatureAsync(request.UserId, request.FeatureId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}/equipped")]
        public async Task<ActionResult<List<Feature>>> GetEquippedFeatures(int userId)
        {
            try
            {
                var result = await featuresService.GetEquippedFeaturesAsync(userId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{featureId}")]
        public async Task<ActionResult<Feature>> GetFeatureById(int featureId)
        {
            try
            {
                var result = await featuresService.GetFeatureByIdAsync(featureId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}/categories")]
        public async Task<ActionResult<Dictionary<string, List<Feature>>>> GetFeaturesByCategories(int userId)
        {
            try
            {
                var result = await featuresService.GetFeaturesByCategoriesAsync(userId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}/preview/{featureId}")]
        public async Task<ActionResult<object>> GetFeaturePreviewData(int userId, int featureId)
        {
            try
            {
                var result = await featuresService.GetFeaturePreviewDataAsync(userId, featureId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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
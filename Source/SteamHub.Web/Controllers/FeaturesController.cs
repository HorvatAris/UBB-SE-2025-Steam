using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SteamHub.Web.Controllers
{
    public class FeaturesController : Controller
    {
        private readonly IFeaturesService _featuresService;
        private readonly IUserService _userService;

        public FeaturesController(IFeaturesService featuresService, IUserService userService)
        {
            _featuresService = featuresService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["LoginRequired"] = "You need to log in to see the features.";
                return View(new FeaturesViewModel { FeaturesByCategories = new Dictionary<string, List<SteamHub.ApiContract.Models.Feature>>(), CurrentUserId = 0 });
            }

            var userId = GetCurrentUserId();
            var featuresByCategories = await _featuresService.GetFeaturesByCategoriesAsync(userId);
            var viewModel = new FeaturesViewModel
            {
                FeaturesByCategories = featuresByCategories,
                CurrentUserId = userId
            };
            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> UserFeatures()
        {
            var userId = GetCurrentUserId();
            var userFeatures = await _featuresService.GetUserFeaturesAsync(userId);
            var equippedFeatures = await _featuresService.GetEquippedFeaturesAsync(userId);

            var viewModel = new UserFeaturesViewModel
            {
                UserFeatures = userFeatures,
                EquippedFeatures = equippedFeatures,
                CurrentUserId = userId
            };

            return View(viewModel);
        }

        [Authorize]
        public async Task<IActionResult> PreviewFeature(int featureId)
        {
            var userId = GetCurrentUserId();
            var (profilePicturePath, bioText, equippedFeatures) = await _featuresService.GetFeaturePreviewDataAsync(userId, featureId);
            var previewedFeature = await _featuresService.GetFeatureByIdAsync(featureId);

            var viewModel = new FeaturePreviewViewModel
            {
                ProfilePicturePath = profilePicturePath,
                BioText = bioText,
                EquippedFeatures = equippedFeatures ?? new List<SteamHub.ApiContract.Models.Feature>(),
                FeatureId = featureId,
                PreviewedFeature = previewedFeature
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PurchaseFeature(int featureId)
        {
            var userId = GetCurrentUserId();
            var success = await _featuresService.AddUserFeatureAsync(userId, featureId);
            return Json(new { success, message = success ? "Feature purchased successfully!" : "Failed to purchase feature." });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EquipFeature(int featureId)
        {
            var userId = GetCurrentUserId();
            var success = await _featuresService.EquipFeatureAsync(userId, featureId);
            return Json(new { success, message = success ? "Feature equipped successfully!" : "Failed to equip feature." });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnequipFeature(int featureId)
        {
            var userId = GetCurrentUserId();
            var success = await _featuresService.UnequipFeatureAsync(userId, featureId);
            return Json(new { success, message = success ? "Feature unequipped successfully!" : "Failed to unequip feature." });
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;
            return 0;
        }
    }
} 
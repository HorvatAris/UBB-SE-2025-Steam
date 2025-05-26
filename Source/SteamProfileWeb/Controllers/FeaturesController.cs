using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamProfileWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace SteamProfileWeb.Controllers
{
    public class FeaturesController : Controller
    {
        private readonly IFeaturesService featuresService;
        private readonly IUserService userService;

        public FeaturesController(IFeaturesService featuresService, IUserService userService)
        {
            this.featuresService = featuresService ?? throw new ArgumentNullException(nameof(featuresService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public IActionResult Index()
        {
            if (!User.Identity.IsAuthenticated)
            {
                TempData["LoginRequired"] = "You need to log in to see the features.";
                return View(new FeaturesViewModel { FeaturesByCategories = new Dictionary<string, List<Feature>>(), CurrentUserId = 0 });
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["LoginRequired"] = "You need to log in to see the features.";
                return View(new FeaturesViewModel { FeaturesByCategories = new Dictionary<string, List<Feature>>(), CurrentUserId = 0 });
            }

            try
            {
                var featuresByCategories = featuresService.GetFeaturesByCategories(userId);
                var viewModel = new FeaturesViewModel
                {
                    FeaturesByCategories = featuresByCategories,
                    CurrentUserId = userId
                };
                return View(viewModel);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("No user is currently logged in"))
            {
                TempData["LoginRequired"] = "You need to log in to see the features.";
                return View(new FeaturesViewModel { FeaturesByCategories = new Dictionary<string, List<Feature>>(), CurrentUserId = 0 });
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult UserFeatures()
        {
            var currentUserId = GetCurrentUserId();
            var userFeatures = featuresService.GetUserFeatures(currentUserId);
            var equippedFeatures = featuresService.GetUserEquippedFeatures(currentUserId);

            var viewModel = new UserFeaturesViewModel
            {
                UserFeatures = userFeatures,
                EquippedFeatures = equippedFeatures,
                CurrentUserId = currentUserId
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult EquipFeature(int featureId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Json(new { success = false, message = "You need to be logged in to equip features." });
            }

            try
            {
                var result = featuresService.EquipFeature(userId, featureId);
                return Json(new { success = result });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult UnequipFeature(int featureId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Json(new { success = false, message = "You need to be logged in to unequip features." });
            }

            try
            {
                var (success, message) = featuresService.UnequipFeature(userId, featureId);
                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult PurchaseFeature(int featureId)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Json(new { success = false, message = "You need to be logged in to purchase features." });
            }

            try
            {
                var (success, message) = featuresService.PurchaseFeature(userId, featureId);
                return Json(new { success, message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize]
        public IActionResult PreviewFeature(int featureId)
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var (profilePicturePath, bioText, equippedFeatures) = featuresService.GetFeaturePreviewData(currentUserId, featureId);
                var previewedFeature = featuresService.GetFeatureById(featureId);

                var viewModel = new FeaturePreviewViewModel
                {
                    ProfilePicturePath = profilePicturePath,
                    BioText = bioText,
                    EquippedFeatures = equippedFeatures,
                    FeatureId = featureId,
                    PreviewedFeature = previewedFeature
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private int GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    var currentUser = userService.GetCurrentUser();
                    if (currentUser != null)
                    {
                        return (int)currentUser.UserId;
                    }
                }
                catch
                {
                    // Fall through to other methods if this fails
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    return userId;
                }

                var nameClaim = User.FindFirst(ClaimTypes.Name);
                if (nameClaim != null)
                {
                    try
                    {
                        var user = userService.GetUserByUsername(nameClaim.Value);
                        if (user != null)
                        {
                            return (int)user.UserId;
                        }
                    }
                    catch
                    {
                        // Continue to fallback
                    }
                }
            }

            return 1; // Default to user ID 1 if not authenticated
        }
    }
} 
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using SteamProfileWeb.ViewModels;
using System.Security.Claims;
namespace SteamProfileWeb.Controllers
{
    public class AchievementsController : Controller
    {
        private readonly IAchievementsService _achievementsService;

        public AchievementsController(IAchievementsService achievementsService)
        {
            _achievementsService = achievementsService;
        }
        public IActionResult Index()
        {
            string userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth"); //  redirect to login
            }

            var result = _achievementsService.GetGroupedAchievementsForUser(userId);

            var vm = new AchievementsViewModel
            {
                FriendshipsAchievements = result.Friendships,
                OwnedGamesAchievements = result.OwnedGames,
                SoldGamesAchievements = result.SoldGames,
                NumberOfPostsAchievements = result.NumberOfPosts,
                NumberOfReviewsGivenAchievements = result.NumberOfReviewsGiven,
                NumberOfReviewsReceivedAchievements = result.NumberOfReviewsReceived,
                YearsOfActivityAchievements = result.YearsOfActivity,
                DeveloperAchievements = result.Developer
            };

            return View(vm);
        }

        //private int GetCurrentUserId()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        // First, try to get user from UserService
        //        try
        //        {
        //            var currentUser = _userService.GetCurrentUser();
        //            if (currentUser != null)
        //            {
        //                return (int)currentUser.UserId;
        //            }
        //        }
        //        catch
        //        {
        //            // Fall through to other methods if this fails
        //        }

        //        // Then, try to get from claims
        //        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        //        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        //        {
        //            return userId;
        //        }

        //        // Try to get from name claim
        //        var nameClaim = User.FindFirst(ClaimTypes.Name);
        //        if (nameClaim != null)
        //        {
        //            // Look up user by username
        //            try
        //            {
        //                var user = _userService.GetUserByUsername(nameClaim.Value);
        //                if (user != null)
        //                {
        //                    return (int)user.UserId;
        //                }
        //            }
        //            catch
        //            {
        //                // Continue to fallback
        //            }
        //        }
        //    }

        //    // If we get here and can't determine user ID, log it for debugging
        //    Console.WriteLine("Warning: Unable to determine current user ID, defaulting to 1");
        //    return 1; // Default to user ID 1 if not authenticated
        //}
    }
}


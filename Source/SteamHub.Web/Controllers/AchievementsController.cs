using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using System.Security.Claims;
namespace SteamHub.Web.Controllers
{
    public class AchievementsController : Controller
    {
        private readonly IAchievementsService achievements_service;

        public AchievementsController(IAchievementsService achievementsService)
        {
            achievements_service = achievementsService;
        }
        public async Task<IActionResult> Index()
        {
            string user_id_string = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(user_id_string, out int userId))
            {
                return RedirectToAction("Login", "Auth"); //  redirect to login
            }

            var result = await achievements_service.GetGroupedAchievementsForUser(userId);

            var achievements_view_model = new AchievementsViewModel
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

            return View(achievements_view_model);
        }

        //private int GetCurrentUserId()
        //{
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        // First, try to get user from UserService
        //        try
        //        {
        //            var currentUser = user_service.GetCurrentUser();
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
        //                var user = user_service.GetUserByUsername(nameClaim.Value);
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


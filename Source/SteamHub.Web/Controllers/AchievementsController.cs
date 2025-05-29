using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;
using SteamHub.Web.Services;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Web.Controllers
{
    public class AchievementsController : Controller
    {
        private readonly IAchievementsService _achievementsService;
        private readonly IUserDetails _userDetails;

        public AchievementsController(
            IAchievementsService achievementsService,
            IUserDetails userDetails)
        {
            _achievementsService = achievementsService;
            _userDetails = userDetails;
        }

        public async Task<IActionResult> Index()
        {
            var result = await _achievementsService.GetGroupedAchievementsForUser(_userDetails.UserId);

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


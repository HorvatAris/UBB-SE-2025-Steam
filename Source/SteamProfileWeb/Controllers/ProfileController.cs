using Microsoft.AspNetCore.Mvc;
using SteamProfileWeb.ViewModels;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BusinessLayer.Models;

namespace SteamProfileWeb.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService userService;
        private readonly IFriendsService friendsService;
        private readonly IUserProfilesRepository userProfileRepository;
        private readonly ICollectionsRepository collectionsRepository;
        private readonly IFeaturesService featuresService;
        private readonly IAchievementsService achievementsService;

        public ProfileController(
            IUserService userService,
            IFriendsService friendsService,
            IUserProfilesRepository userProfileRepository,
            ICollectionsRepository collectionsRepository,
            IFeaturesService featuresService,
            IAchievementsService achievementsService)
        {
            this.userService = userService;
            this.friendsService = friendsService;
            this.userProfileRepository = userProfileRepository;
            this.collectionsRepository = collectionsRepository;
            this.featuresService = featuresService;
            this.achievementsService = achievementsService;
        }

        public IActionResult Index()
        {
            string userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var user = userService.GetUserByIdentifier(userId);
            if (user == null)
                return NotFound();

            var userProfile = userProfileRepository.GetUserProfileByUserId(userId);
            var collections = collectionsRepository.GetLastThreeCollectionsForUser(userId);

            var vm = new ProfileViewModel
            {
                UserIdentifier = user.UserId,
                Username = user.Username,
                Email = user.Email,
                ProfilePhotoPath = userProfile?.ProfilePicture ?? "/images/default-profile.png",
                Biography = userProfile?.Bio ?? "",
                FriendCount = friendsService.GetFriendshipCount(userId),
                GameCollections = collections
            };

            return View(vm);
        }
    }
}
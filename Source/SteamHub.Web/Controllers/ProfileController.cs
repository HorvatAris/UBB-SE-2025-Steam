﻿using Microsoft.AspNetCore.Mvc;
using SteamHub.Web.ViewModels;
using SteamHub.ApiContract.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IUserService userService;
        private readonly IFriendsService friendsService;
        private readonly ICollectionsService collectionsService;
        private readonly IFeaturesService featuresService;
        private readonly IAchievementsService achievementsService;

        public ProfileController(
            IUserService userService,
            IFriendsService friendsService,
            ICollectionsService collectionsService,
            IFeaturesService featuresService,
            IAchievementsService achievementsService)
        {
            this.userService = userService;
            this.friendsService = friendsService;
            this.collectionsService = collectionsService;
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

            var user = userService.GetUserByIdentifierAsync(userId).Result;
            // FIX: CurrentUser is null, these are 100% Problems with web login
            // var currentUser = userService.GetCurrentUserAsync().Result;
            var currentUser = user;
            if (user == null || currentUser == null)
                return NotFound();

            var collections = collectionsService.GetLastThreeCollectionsForUser(userId).Result;

            var vm = new ProfileViewModel
            {
                UserIdentifier = user.UserId,
                Username = user.Username,
                Email = user.Email,
                IsProfileOwner = user.UserId == currentUser.UserId,
                IsDeveloper = user.IsDeveloper,
                ProfilePhotoPath = user?.ProfilePicture ?? "/images/default-profile.png",
                Biography = user?.Bio ?? "",
                FriendCount = friendsService.GetFriendshipCountAsync(userId).Result,
                GameCollections = collections
            };

            return View(vm);
        }
    }
}
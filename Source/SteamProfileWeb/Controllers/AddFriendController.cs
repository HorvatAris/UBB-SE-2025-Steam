using Microsoft.AspNetCore.Mvc;
using SteamProfileWeb.ViewModels;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Linq;

namespace SteamProfileWeb.Controllers
{
    [Authorize]
    public class AddFriendController : Controller
    {
        private readonly IUserService userService;
        private readonly IFriendsService friendsService;
        private readonly IUserProfilesRepository userProfilesRepository;

        public AddFriendController(IUserService userService, IFriendsService friendsService, IUserProfilesRepository userProfilesRepository)
        {
            this.userService = userService;
            this.friendsService = friendsService;
            this.userProfilesRepository = userProfilesRepository;
        }

        public IActionResult Index()
        {
            string currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdStr, out int currentUserId))
            {
                return RedirectToAction("Login", "Auth");
            }
            var allUsers = userService.GetAllUsers().Where(u => u.UserId != currentUserId).ToList();
            var friendships = friendsService.GetAllFriendships();
            var friendIds = friendships.Select(f => f.FriendId).ToHashSet();

            var model = new AddFriendViewModel
            {
                Users = allUsers.Select(u => {
                    var userProfile = userProfilesRepository.GetUserProfileByUserId(u.UserId);
                    return new AddFriendUserViewModel
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Email = u.Email,
                        ProfilePhotoPath = userProfile?.ProfilePicture ?? "/images/default-profile.png",
                        IsFriend = friendIds.Contains(u.UserId)
                    };
                }).ToList(),
                CurrentUserId = currentUserId,
                ErrorMessage = TempData["ErrorMessage"] as string
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult AddFriend(int userId)
        {
            string currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdStr, out int currentUserId))
            {
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                friendsService.AddFriend(currentUserId, userId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RemoveFriend(int userId)
        {
            string currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdStr, out int currentUserId))
            {
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                var friendshipId = friendsService.GetFriendshipIdentifier(currentUserId, userId);
                if (friendshipId.HasValue)
                    friendsService.RemoveFriend(friendshipId.Value);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
} 
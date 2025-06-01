using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Web.ViewModels;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class AddFriendController : Controller
    {
        private readonly IUserService userService;
        private readonly IFriendsService friendsService;

        public AddFriendController(IUserService userService, IFriendsService friendsService)
        {
            this.userService = userService;
            this.friendsService = friendsService;
        }

        public async Task<IActionResult> IndexAsync()
        {
            string currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdStr, out int currentUserId))
            {
                return RedirectToAction("Login", "Auth");
            }
            var allUsers = (await userService.GetAllUsersAsync()).Where(u => u.UserId != currentUserId).ToList();
            var friendships = await friendsService.GetAllFriendshipsAsync(currentUserId);
            var friendIds = friendships.Select(f => f.FriendId).ToHashSet();

            var model = new AddFriendViewModel
            {
                Users = allUsers.Select(u => {

                    return new AddFriendUserViewModel
                    {
                        UserId = u.UserId,
                        Username = u.Username,
                        Email = u.Email,
                        ProfilePhotoPath = u?.ProfilePicture ?? "/images/default-profile.png",
                        IsFriend = friendIds.Contains(u.UserId)
                    };
                }).ToList(),
                CurrentUserId = currentUserId,
                ErrorMessage = TempData["ErrorMessage"] as string
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddFriendAsync(int userId)
        {
            string currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdStr, out int currentUserId))
            {
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                await friendsService.AddFriendAsync(currentUserId, userId);
                await friendsService.AddFriendAsync(userId, currentUserId);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriendAsync(int userId)
        {
            string currentUserIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(currentUserIdStr, out int currentUserId))
            {
                return RedirectToAction("Login", "Auth");
            }
            try
            {
                var friendshipId = await friendsService.GetFriendshipIdentifierAsync(currentUserId, userId);
                if (friendshipId.HasValue)
                    await friendsService.RemoveFriendAsync(friendshipId.Value);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Index");
        }
    }
}

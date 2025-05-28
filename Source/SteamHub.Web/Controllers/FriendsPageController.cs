using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    public class FriendsPageController : Controller
    {
        private readonly IFriendsService _friendsService;
        private readonly IUserDetails _userDetails;

        public FriendsPageController(IFriendsService friendsService, IUserDetails userDetails)
        {
            _friendsService = friendsService;
            _userDetails = userDetails;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var friendships = await _friendsService.GetAllFriendshipsAsync(_userDetails.UserId);
                return View(friendships);
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = "Error loading friends: " + ex.Message;
                return View(new List<Friendship>());
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFriend(int friendshipId)
        {
            try
            {
                await _friendsService.RemoveFriendAsync(friendshipId);
                return RedirectToAction(nameof(Index));
            }
            catch (System.Exception ex)
            {
                ViewBag.ErrorMessage = "Error removing friend: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
    }
} 
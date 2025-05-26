using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService friendService;

        public FriendController(IFriendService friendService)
        {
            this.friendService = friendService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFriends([FromQuery] string username)
        {
            var friends = await friendService.GetFriendsAsync(username);
            return Ok(friends);
        }

        [HttpPost]
        public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest request)
        {
            var result = await friendService.AddFriendAsync(
                request.User1Username,
                request.User2Username,
                request.FriendEmail,
                request.FriendProfilePhotoPath);

            return Ok(result);
        }

        [HttpPost("remove")]
        public async Task<IActionResult> RemoveFriend([FromBody] RemoveFriendRequest request)
        {
            var result = await friendService.RemoveFriendAsync(
                request.User1Username,
                request.User2Username);

            return Ok(result);
        }
    }

    public class AddFriendRequest
    {
        public string User1Username { get; set; }
        public string User2Username { get; set; }
        public string FriendEmail { get; set; }
        public string FriendProfilePhotoPath { get; set; }
    }

    public class RemoveFriendRequest
    {
        public string User1Username { get; set; }
        public string User2Username { get; set; }
    }
}
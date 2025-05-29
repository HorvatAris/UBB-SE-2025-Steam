using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendsService friendsService;

        public FriendsController(IFriendsService friendsService)
        {
            this.friendsService = friendsService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFriendshipsAsync(int userId)
        {
            var friendships = await friendsService.GetAllFriendshipsAsync(userId);
            return Ok(friendships);
        }

        [HttpGet("{userId}/count")]
        public async Task<IActionResult> GetFriendshipCountAsync(int userId)
        {
            var count = await friendsService.GetFriendshipCountAsync(userId);
            return Ok(count);
        }

        [HttpGet("check")]
        public async Task<IActionResult> AreUsersFriendsAsync([FromQuery] int user1, [FromQuery] int user2)
        {
            var areFriends = await friendsService.AreUsersFriendsAsync(user1, user2);
            return Ok(areFriends);
        }

        [HttpGet("id")]
        public async Task<IActionResult> GetFriendshipIdAsync([FromQuery] int user1, [FromQuery] int user2)
        {
            var friendshipId = await friendsService.GetFriendshipIdentifierAsync(user1, user2);
            return Ok(friendshipId);
        }

        [HttpPost]
        public async Task<IActionResult> AddFriendAsync([FromBody] FriendshipRequest request)
        {
            await friendsService.AddFriendAsync(request.UserId, request.FriendId);
            return Ok(new { success = true });
        }

        [HttpDelete("{friendshipId}")]
        public async Task<IActionResult> RemoveFriendAsync(int friendshipId)
        {
            await friendsService.RemoveFriendAsync(friendshipId);
            return Ok(new { success = true, message = "Friend removed successfully" });
        }

        [HttpPost("add-by-username")]
        public async Task<IActionResult> AddFriend([FromBody] AddFriendRequest request)

        {
            var result = await friendsService.AddFriendByUsernameAsync(
                request.User1Username,
                request.User2Username,
                request.FriendEmail,
                request.FriendProfilePhotoPath);

            return Ok(new { success = result });
        }
    }

    public class FriendshipRequest
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
    }

    public class AddFriendRequest
    {
        public string User1Username { get; set; }
        public string User2Username { get; set; }
        public string FriendEmail { get; set; }
        public string FriendProfilePhotoPath { get; set; }
    }

}

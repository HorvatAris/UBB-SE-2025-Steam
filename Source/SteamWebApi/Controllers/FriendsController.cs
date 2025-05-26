using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
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
        public IActionResult GetFriendships(int userId)
        {
            var friendships = friendsService.GetAllFriendships();
            return Ok(friendships);
        }

        [HttpGet("{userId}/count")]
        public IActionResult GetFriendshipCount(int userId)
        {
            var count = friendsService.GetFriendshipCount(userId);
            return Ok(count);
        }

        [HttpGet("check")]
        public IActionResult AreUsersFriends([FromQuery] int user1, [FromQuery] int user2)
        {
            var areFriends = friendsService.AreUsersFriends(user1, user2);
            return Ok(areFriends);
        }

        [HttpGet("id")]
        public IActionResult GetFriendshipId([FromQuery] int user1, [FromQuery] int user2)
        {
            var friendshipId = friendsService.GetFriendshipIdentifier(user1, user2);
            return Ok(friendshipId);
        }

        [HttpPost]
        public IActionResult AddFriend([FromBody] FriendshipRequest request)
        {
            friendsService.AddFriend(request.UserId, request.FriendId);
            return Ok();
        }

        [HttpDelete("{friendshipId}")]
        public IActionResult RemoveFriend(int friendshipId)
        {
            friendsService.RemoveFriend(friendshipId);
            return Ok();
        }
    }

    public class FriendshipRequest
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
    }
}
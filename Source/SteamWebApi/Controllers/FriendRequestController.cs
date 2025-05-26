using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendRequestController : ControllerBase
    {
        private readonly IFriendRequestService friendRequestService;

        public FriendRequestController(IFriendRequestService friendRequestService)
        {
            this.friendRequestService = friendRequestService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFriendRequests(string username)
        {
            var requests = await friendRequestService.GetFriendRequestsAsync(username);
            return Ok(requests);
        }

        [HttpPost]
        public async Task<IActionResult> SendFriendRequest(FriendRequest request)
        {
            var result = await friendRequestService.SendFriendRequestAsync(request);
            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to send friend request.");
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFriendRequest([FromBody] FriendRequestAction request)
        {
            var result = await friendRequestService.AcceptFriendRequestAsync(
                request.SenderUsername,
                request.ReceiverUsername);

            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to accept friend request.");
        }

        [HttpPost("reject")]
        public async Task<IActionResult> RejectFriendRequest([FromBody] FriendRequestAction request)
        {
            var result = await friendRequestService.RejectFriendRequestAsync(
                request.SenderUsername,
                request.ReceiverUsername);

            if (result)
            {
                return Ok();
            }
            return BadRequest("Failed to reject friend request.");
        }
    }

    public class FriendRequestAction
    {
        public string SenderUsername { get; set; }
        public string ReceiverUsername { get; set; }
    }
}
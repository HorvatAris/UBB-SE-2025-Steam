using BusinessLayer.Exceptions;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BusinessLayer.Services.AchievementsService;

namespace SteamWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService chatService;

        public ChatController(IChatService chatService)
        {
            this.chatService = chatService;
        }

        [HttpGet("Messages/{myId}/{friendId}")]
        public IActionResult Messages(int myId,int friendId)
        {
            /*
            var currentUserIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentUserIdStr, out int currentUserId))
            {
                return Unauthorized("Invalid or missing JWT user ID.");
            }

            if (currentUserId != myId && currentUserId != friendId)
            {
                return Forbid("You are not a participant in this conversation.");
            }
            */

            var messages = chatService.GetAllMessages(myId,friendId);
            return Ok(messages);
        }

        [HttpPost("Send")]
        public IActionResult Send([FromBody] SendMessageRequest request)
        {
            /*
            var currentUserIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(currentUserIdStr, out int currentUserId))
            {
                return Unauthorized("Invalid or missing JWT user ID.");
            }

            if (currentUserId != request.MyId && currentUserId != request.FriendId)
            {
                return Forbid("You are not a participant in this conversation.");
            }
            */

            ChatMessage msg = chatService.SendMessage(request.MyId, request.FriendId, request.Data);
            return Ok(msg);
        }

      
    }

    public class SendMessageRequest
    {
        public int MyId { get; set; }
        public int FriendId { get; set; }
        public string Data { get; set; }
    }
}
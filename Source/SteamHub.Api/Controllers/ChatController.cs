using Microsoft.AspNetCore.Mvc;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Services.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamHub.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet("Messages/{myId}/{friendId}")]
        public async Task<ActionResult<List<ChatMessage>>> GetAllMessages(int myId, int friendId)
        {
            var messages = await _chatService.GetAllMessages(myId, friendId);
            return Ok(messages);
        }

        [HttpPost("Send")]
        public async Task<ActionResult<ChatMessage>> SendMessage([FromBody] SendMessageRequest request)
        {
            var message = await _chatService.SendMessage(request.MyId, request.FriendId, request.Data);
            return Ok(message);
        }
    }

    public class SendMessageRequest
    {
        public int MyId { get; set; }
        public int FriendId { get; set; }
        public string Data { get; set; } = string.Empty;
    }
} 
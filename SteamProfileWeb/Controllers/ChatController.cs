// ChatController.cs (UI Project)
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamProfileWeb.ViewModels;
using System.Net.Http.Json;
using System.Security.Claims;

namespace SteamProfileWeb.Controllers
{
    [Authorize]
    [Route("Chat")]
    public class ChatController : Controller
    {
        private readonly IUserService _userService;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatController(IHttpClientFactory httpClientFactory,IUserService userService)
        {
            _userService = userService;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet("{friendId}")]
        public IActionResult Index(int friendId)
        {
            var myIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(myIdStr, out int myId))
                return RedirectToAction("Login", "Auth");

            var apiUrl = HttpContext.RequestServices.GetService<IConfiguration>()?["ApiSettings:BaseUrl"] ?? "";

            var vm = new ChatViewModel
            {
                MyId = myId,
                FriendId = friendId,
                FriendName = _userService.GetUserByIdentifier(friendId).Username ?? "Unknown",
                ApiUrl = apiUrl
            };

            return View(vm);
        }

        [HttpGet("FetchMessages/{friendId}")]
        public async Task<IActionResult> FetchMessages(int friendId)
        {
            var myIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(myIdStr, out int myId)) return Unauthorized();

            var client = _httpClientFactory.CreateClient("SteamWebApi");
            var response = await client.GetAsync($"Chat/Messages/{myId}/{friendId}");
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var messages = await response.Content.ReadFromJsonAsync<List<ChatMessage>>();
            return Json(messages);
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            var myIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(myIdStr, out int myId) || myId != request.MyId)
                return Unauthorized();

            var client = _httpClientFactory.CreateClient("SteamWebApi");
            var response = await client.PostAsJsonAsync("Chat/Send", request);
            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode);

            var msg = await response.Content.ReadFromJsonAsync<ChatMessage>();
            return Json(msg);
        }
    }

    public class SendMessageRequest
    {
        public int MyId { get; set; }
        public int FriendId { get; set; }
        public string Data { get; set; } = string.Empty;
    }
}

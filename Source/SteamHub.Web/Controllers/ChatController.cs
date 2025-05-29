using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamHub.Web.ViewModels;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models;
using System.Net.Http.Json;
using System.Security.Claims;

namespace SteamHub.Web.Controllers
{
    [Authorize]
    [Route("Chat")]
    public class ChatController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserService _userService;

        public ChatController(IHttpClientFactory httpClientFactory, IUserService userService)
        {
            _httpClientFactory = httpClientFactory;
            _userService = userService;
        }

        [HttpGet("{friendId}")]
        public async Task<IActionResult> Index(int friendId)
        {
            var myIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(myIdStr, out int myId))
                return RedirectToAction("Login", "Auth");

            var apiUrl = HttpContext.RequestServices.GetService<IConfiguration>()?["ApiSettings:BaseUrl"] ?? "";

            var user = await _userService.GetUserByIdentifierAsync(friendId);

            var vm = new ChatViewModel
            {
                MyId = myId,
                FriendId = friendId,
                FriendName = user?.Username ?? "Unknown",
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

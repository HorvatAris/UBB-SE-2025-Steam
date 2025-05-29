using SteamHub.ApiContract.Models;

namespace SteamHub.Web.ViewModels
{
    public class ChatViewModel
    {
        public string ApiUrl { get; set; }
        public int MyId { get; set; }
        public int FriendId { get; set; }
        public string FriendName { get; set; }
        public List<ChatMessage> Messages { get; set; } = new();
        public string NewMessage { get; set; }
    }


}
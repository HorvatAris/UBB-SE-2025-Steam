using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.ServiceProxies;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class ChatServiceProxy : ServiceProxy, IChatService
    {
        public ChatServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }
        public async Task<List<ChatMessage>> GetAllMessages(int myId, int friendId)
        {
            return await GetAsync<List<ChatMessage>>($"Chat/Messages/{myId}/{friendId}");
        }

        public async Task<ChatMessage?> SendMessage(int myId, int friendId, string message)
        {
            return await PostAsync<ChatMessage>($"Chat/Send", new { MyId = myId, FriendId = friendId, Data = message });
        }
    }
}
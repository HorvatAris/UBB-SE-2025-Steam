using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Proxies
{
    public class ChatServiceProxy : ServiceProxy, IChatService
    {
        public ChatServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }
        public List<ChatMessage> GetAllMessages(int myId, int friendId)
        {
            return GetAsync<List<ChatMessage>>($"Chat/Messages/{myId}/{friendId}").GetAwaiter().GetResult();
        }

        public ChatMessage SendMessage(int myId, int friendId, string message)
        {
            return PostAsync<ChatMessage>($"Chat/Send", new { MyId = myId, FriendId = friendId, Data=message }).GetAwaiter().GetResult();
        }
    }
}

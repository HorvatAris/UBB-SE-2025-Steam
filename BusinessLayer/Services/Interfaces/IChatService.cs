using System;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IChatService
    {
        public List<ChatMessage> GetAllMessages(int myId, int targetedUserId);
        public ChatMessage SendMessage(int myId, int targetedUserId, string message);
    }
}

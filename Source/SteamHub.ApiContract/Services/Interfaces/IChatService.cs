using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface IChatService
    {
        Task<List<ChatMessage>> GetAllMessages(int myId, int targetedUserId);
        Task<ChatMessage?> SendMessage(int myId, int targetedUserId, string message);
    }
}
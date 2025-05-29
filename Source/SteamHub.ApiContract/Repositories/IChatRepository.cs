using SteamHub.ApiContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Repositories
{
    public interface IChatRepository
    {
        Task<ChatConversation> CreateConversation(int user1, int user2);

        Task<ChatConversation?> GetConversation(int user1, int user2);

        Task<ChatMessage> SendMessage(int senderId, int conversationId, string messageContent, string messageFormat);

        Task<List<ChatMessage>> GetAllMessagesOfConversation(int conv_id);
    }
}

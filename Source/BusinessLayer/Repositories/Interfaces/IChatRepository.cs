using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IChatRepository
    {
        public ChatConversation CreateConversation(int user1, int user2);

        public ChatConversation GetConversation(int user1, int user2);

        public ChatMessage SendMessage(int senderId, int conversationId, string messageContent, string messageFormat);

        public List<ChatMessage> GetAllMessagesOfConversation(int conv_id);
    }
}

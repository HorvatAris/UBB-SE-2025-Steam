using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.DataContext;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly ApplicationDbContext context;

        public ChatRepository(ApplicationDbContext newContext)
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public ChatConversation CreateConversation(int user1, int user2)
        {
            ChatConversation chatConversation = GetConversation(user1, user2);
            if (chatConversation != null)
            {
                return chatConversation;
            }

            ChatConversation conversation = new ChatConversation
            {
                User1Id = user1,
                User2Id = user2
            };
            context.ChatConversations.Add(conversation);
            context.SaveChanges();
            return conversation;
        }

        public ChatConversation GetConversation(int user1, int user2)
        {
            List<ChatConversation> conversations = context.ChatConversations
                .Where(c => (c.User1Id == user1 && c.User2Id == user2) || (c.User1Id == user2 && c.User2Id == user1))
                .ToList();
            if (conversations.Count == 0)
            {
                return null;
            }
            return conversations[0];
        }

        public ChatMessage SendMessage(int senderId, int conversationId, string messageContent, string messageFormat)
        {
            ChatMessage message = new ChatMessage
            {
                SenderId = senderId,
                ConversationId = conversationId,
                MessageContent = messageContent,
                MessageFormat = messageFormat,
                Timestamp = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds()
            };
            context.ChatMessages.Add(message);
            context.SaveChanges();
            return message;
        }

        public List<ChatMessage> GetAllMessagesOfConversation(int conv_id)
        {
            List<ChatConversation> conversations = context.ChatConversations
                .Where(c => c.ConversationId == conv_id)
                .ToList();
            if (conversations.Count == 0)
            {
                return new List<ChatMessage>();
            }
            List<ChatMessage> messages = new List<ChatMessage>();
            for (int i = 0; i < conversations.Count; i++)
            {
                messages.AddRange(context.ChatMessages
                    .Where(m => m.ConversationId == conversations[i].ConversationId)
                    .Select(m => new ChatMessage
                    {
                        MessageId = m.MessageId,
                        SenderId = m.SenderId,
                        ConversationId = m.ConversationId,
                        MessageContent = m.MessageContent,
                        MessageFormat = m.MessageFormat,
                        Timestamp = m.Timestamp
                    })
                    .OrderBy(m => m.Timestamp)
                    .ToList());
            }
            return messages;
        }
    }
}

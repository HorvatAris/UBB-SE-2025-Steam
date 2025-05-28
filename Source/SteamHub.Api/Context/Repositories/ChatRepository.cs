
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Context.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly DataContext context;

        public ChatRepository(DataContext newContext)
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

            // Ensure the correct type is used for ChatConversation
            SteamHub.Api.Entities.ChatConversation conversation = new SteamHub.Api.Entities.ChatConversation
            {
                User1Id = user1,
                User2Id = user2
            };
            context.ChatConversations.Add(conversation);
            context.SaveChanges();
            return new ChatConversation
            {
                ConversationId = conversation.ConversationId,
                User1Id = conversation.User1Id,
                User2Id = conversation.User2Id
            };
        }

        public ChatConversation GetConversation(int user1, int user2)
        {
            var conversation = context.ChatConversations
                .FirstOrDefault(chat_conversation => (chat_conversation.User1Id == user1 && chat_conversation.User2Id == user2) || (chat_conversation.User1Id == user2 && chat_conversation.User2Id == user1));

            if (conversation == null)
            {
                return null;
            }

            return new ChatConversation
            {
                ConversationId = conversation.ConversationId,
                User1Id = conversation.User1Id,
                User2Id = conversation.User2Id
            };
        }

        public ChatMessage SendMessage(int senderId, int conversationId, string messageContent, string messageFormat)
        {
            SteamHub.Api.Entities.ChatMessage message = new SteamHub.Api.Entities.ChatMessage
            {
                SenderId = senderId,
                ConversationId = conversationId,
                MessageContent = messageContent,
                MessageFormat = messageFormat,
                Timestamp = DateTime.Now
            };
            context.ChatMessages.Add(message);
            context.SaveChanges();
            return new ChatMessage
            {
                MessageId = message.MessageId,
                SenderId = message.SenderId,
                ConversationId = message.ConversationId,
                MessageContent = message.MessageContent,
                MessageFormat = message.MessageFormat,
                Timestamp = message.Timestamp.Ticks 
            };
        }

        public List<ChatMessage> GetAllMessagesOfConversation(int conv_id)
        {
            var messages = context.ChatMessages
                .Where(chat_message => chat_message.ConversationId == conv_id)
                .OrderBy(chat_message => chat_message.Timestamp)
                .Select(chat_message => new ChatMessage
                {
                    MessageId = chat_message.MessageId,
                    SenderId = chat_message.SenderId,
                    ConversationId = chat_message.ConversationId,
                    MessageContent = chat_message.MessageContent,
                    MessageFormat = chat_message.MessageFormat,
                    Timestamp = chat_message.Timestamp.Ticks
                })
                .ToList();

            return messages;
        }
    }
}

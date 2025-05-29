using SteamHub.ApiContract.Models;
using Microsoft.EntityFrameworkCore;
using ChatConversationEntity = SteamHub.Api.Entities.ChatConversation;
using ChatMessageEntity = SteamHub.Api.Entities.ChatMessage;
using SteamHub.Api.Context;

namespace SteamHub.ApiContract.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly DataContext context;

        public ChatRepository(DataContext newContext)
        {
            context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public async Task<ChatConversation> CreateConversation(int user1, int user2)
        {
            var existing = await GetConversationEntity(user1, user2);
            if (existing != null)
                return MapToContractConversation(existing);

            var conversation = new ChatConversationEntity
            {
                User1Id = user1,
                User2Id = user2
            };
            context.ChatConversations.Add(conversation);
            await context.SaveChangesAsync();
            return MapToContractConversation(conversation);
        }

        public async Task<ChatConversation?> GetConversation(int user1, int user2)
        {
            var entity = await GetConversationEntity(user1, user2);
            return entity == null ? null : MapToContractConversation(entity);
        }

        public async Task<ChatMessage> SendMessage(int senderId, int conversationId, string messageContent, string messageFormat)
        {
            var message = new ChatMessageEntity
            {
                SenderId = senderId,
                ConversationId = conversationId,
                MessageContent = messageContent,
                MessageFormat = messageFormat,
                Timestamp = DateTime.UtcNow
            };
            context.ChatMessages.Add(message);
            await context.SaveChangesAsync();
            return MapToContractMessage(message);
        }

        public async Task<List<ChatMessage>> GetAllMessagesOfConversation(int conv_id)
        {
            var messages = await context.ChatMessages
                .Where(m => m.ConversationId == conv_id)
                .OrderBy(m => m.Timestamp)
                .ToListAsync();
            return messages.Select(MapToContractMessage).ToList();
        }

        // Helper to get entity conversation
        private async Task<ChatConversationEntity?> GetConversationEntity(int user1, int user2)
        {
            return await context.ChatConversations
                .FirstOrDefaultAsync(c => (c.User1Id == user1 && c.User2Id == user2) || (c.User1Id == user2 && c.User2Id == user1));
        }

        // Mapping helpers
        private ChatConversation MapToContractConversation(ChatConversationEntity entity)
        {
            if (entity == null) return null!;
            return new ChatConversation
            {
                ConversationId = entity.ConversationId,
                User1Id = entity.User1Id,
                User2Id = entity.User2Id
            };
        }

        private ChatMessage MapToContractMessage(ChatMessageEntity entity)
        {
            if (entity == null) return null!;
            return new ChatMessage
            {
                MessageId = entity.MessageId,
                ConversationId = entity.ConversationId,
                SenderId = entity.SenderId,
                MessageContent = entity.MessageContent,
                MessageFormat = entity.MessageFormat,
                Timestamp = new DateTimeOffset(entity.Timestamp).ToUnixTimeMilliseconds()
            };
        }
    }
}
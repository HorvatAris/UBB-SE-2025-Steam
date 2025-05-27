using System;
using System.Collections.Generic;

namespace SteamHub.Api.Entities
{
    public class ChatConversation
    {
        public int ConversationId { get; set; }
        public int User1Id { get; set; }
        public int User2Id { get; set; }

        // Navigation properties
        public User User1 { get; set; }
        public User User2 { get; set; }
        public ICollection<ChatMessage> Messages { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.Api.Entities
{
    public class ChatMessage
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string MessageContent { get; set; }
        public string MessageFormat { get; set; }
        public DateTime Timestamp { get; set; }

        // Navigation properties
        public ChatConversation Conversation { get; set; }
        public User Sender { get; set; }
    }
}

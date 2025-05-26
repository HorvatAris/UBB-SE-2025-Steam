using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Models
{
    public class ChatMessage
    {
        public int MessageId { get; set; }
        public int ConversationId { get; set; }
        public int SenderId { get; set; }
        public string MessageContent { get; set; }
        public string MessageFormat { get; set; }
        public long Timestamp { get; set; }
    }
}

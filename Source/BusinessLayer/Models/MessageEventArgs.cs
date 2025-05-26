using System;
using BusinessLayer.Models;

namespace BusinessLayer.Models
{
    /// <summary>
    /// Event arguments for message-related events.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public ChatMessage Message { get; }
        public MessageEventArgs(ChatMessage message) => Message = message;
    }
}
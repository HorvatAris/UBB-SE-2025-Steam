using System;
using SteamHub.ApiContract.Models;

namespace SteamHub.ApiContract.Models
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
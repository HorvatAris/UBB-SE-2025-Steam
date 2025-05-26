using System;
using BusinessLayer.Models;

namespace SteamProfile.Implementation
{
    // Used for events (a new message has been received => show it to the user)
    public class MessageEventArgs : EventArgs
    {
        public ChatMessage Message { get; }
        public MessageEventArgs(ChatMessage message) => Message = message;
    }
}

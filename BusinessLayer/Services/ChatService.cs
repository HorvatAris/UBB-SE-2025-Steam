using System;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Services
{
    public partial class ChatService : IChatService
    {
        private IChatRepository chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        public List<ChatMessage> GetAllMessages(int myId, int targetedUserId)
        {
            ChatConversation conversation = this.chatRepository.CreateConversation(myId, targetedUserId);
            List<ChatMessage> messages = this.chatRepository.GetAllMessagesOfConversation(conversation.ConversationId);
            return messages;
        }

        public ChatMessage SendMessage(int myId, int targetedUserId, string data)
        {
            ChatConversation conv = this.chatRepository.CreateConversation(myId, targetedUserId);
            try
            {
                ChatMessage message = this.chatRepository.SendMessage(myId, conv.ConversationId, data, "text");
                return message;
            }
            catch (Exception exception)
            {
                Debug.WriteLine($"Error sending message: {exception.Message}");
                return null;
            }
        }
    }
}

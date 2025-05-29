using System;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Repositories;
using System.Threading.Tasks;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContract.Services
{
    public partial class ChatService : IChatService
    {
        private IChatRepository chatRepository;

        public ChatService(IChatRepository chatRepository)
        {
            this.chatRepository = chatRepository;
        }

        public async Task<List<ChatMessage>> GetAllMessages(int myId, int targetedUserId)
        {
            ChatConversation conversation = await this.chatRepository.CreateConversation(myId, targetedUserId);
            List<ChatMessage> messages = await this.chatRepository.GetAllMessagesOfConversation(conversation.ConversationId);
            return messages;
        }

        public async Task<ChatMessage?> SendMessage(int myId, int targetedUserId, string data)
        {
            ChatConversation conv = await this.chatRepository.CreateConversation(myId, targetedUserId);
            try
            {
                ChatMessage message = await this.chatRepository.SendMessage(myId, conv.ConversationId, data, "text");
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
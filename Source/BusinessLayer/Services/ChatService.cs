using System;
using System.Collections.Generic;
using System.Net;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using Google.Protobuf.WellKnownTypes;

namespace BusinessLayer.Services
{
    public partial class ChatService : IChatService
    {
        private ChatConversation conversation;
        public event EventHandler<MessageEventArgs> NewMessageEvent;
        public event EventHandler<ExceptionEventArgs> ExceptionEvent;

        private int myId;
        private int targetedUserId;
        private IChatRepository chatRepository;

        public ChatService(IChatRepository chatRepository, int myId, int targetedUserId)
        {
            this.myId = myId;
            this.targetedUserId = targetedUserId;
            this.chatRepository = chatRepository;
        }

        public async void ConnectUserToServer()
        {
            this.conversation = this.chatRepository.CreateConversation(this.myId, this.targetedUserId);
            List<ChatMessage> messages = this.chatRepository.GetAllMessagesOfConversation(this.conversation.ConversationId);
            for (int i = 0; i < messages.Count; i++)
            {
                int idx = i;
                this.NewMessageEvent?.Invoke(this, new MessageEventArgs(messages[idx]));
            }
        }

        public void SendMessage(string data)
        {
            try
            {
                ChatMessage message = this.chatRepository.SendMessage(this.myId, this.conversation.ConversationId, data, "text");
                NewMessageEvent?.Invoke(this, new MessageEventArgs(message));
            }
            catch (Exception exception)
            {
                // Alert the UI about the new exception
                this.ExceptionEvent?.Invoke(this, new ExceptionEventArgs(exception));
            }
        }

        private void UpdateNewMessage(object? sender, MessageEventArgs messageEventArgs)
        {
            /*
            Message newMessage = messageEventArgs.Message;
            // Messages are sent with a left alligment by the server
            // The service aligns the messages for each client so that messages sent by the user
            // are on the right, received messages are on the left
            newMessage.MessageAligment = newMessage.MessageSenderName == this.userName ? "Right" : "Left";
            // Proceeds to alert the ui about the new aligned message
            this.uiThread.TryEnqueue(() => this.NewMessageEvent?.Invoke(this, new MessageEventArgs(newMessage)));
            */
        }

        public void DisconnectClient()
        {
            // Further call to disconnect the client on window close
            // this.client?.Disconnect();
        }

        public void TryChangeMuteStatus(string targetedUser)
        {
            // Commands can be found in more detail on the Server class
            // They follow a defined patter like "<something>|something|<something>"
            // Any user can send a message that is a command, even if they don't have access to
            // but the server will check for user status in the chat (this is where the try comes from)
            // Change means that it can become true or false
            // string command = "<" + this.userName + ">|" + Server.MUTE_STATUS + "|<" + targetedUser + ">";
            // this.SendMessage(command);
        }

        public void TryChangeAdminStatus(string targetedUser)
        {
            // string command = "<" + this.userName + ">|" + Server.ADMIN_STATUS + "|<" + targetedUser + ">";
            // this.SendMessage(command);
        }

        public void TryKick(string targetedUser)
        {
            // string command = "<" + this.userName + ">|" + Server.KICK_STATUS + "|<" + targetedUser + ">";
            // this.SendMessage(command);
        }
    }
}

﻿using System;
using System.Threading.Tasks;

namespace SteamProfile.Implementation
{
    internal interface IChatService
    {
        public event EventHandler<MessageEventArgs> NewMessageEvent;
        public event EventHandler<ClientStatusEventArgs> ClientStatusChangedEvent;
        public event EventHandler<ExceptionEventArgs> ExceptionEvent;
        public void ConnectUserToServer();
        public void SendMessage(string message);
        public void DisconnectClient();
        public void TryChangeMuteStatus(string targetedUser);
        public void TryChangeAdminStatus(string targetedUser);
        public void TryKick(string targetedUser);
    }
}

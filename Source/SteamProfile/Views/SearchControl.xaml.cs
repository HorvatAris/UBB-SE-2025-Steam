using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;
using BusinessLayer.Services;
using BusinessLayer.Models;

namespace SteamProfile.Views
{
    public sealed partial class SearchControl : UserControl
    {
        public SearchViewModel ViewModel;

        public SearchControl()
        {
            this.InitializeComponent();
            var service = new Service();
            ViewModel = new SteamProfile.ViewModels.SearchViewModel(service);
            this.DataContext = ViewModel;

            ViewModel.ChatRoomOpened += (s, e) =>
            {
                ChatRoomOpened?.Invoke(this, e);
            };
        }

        public event EventHandler<ChatRoomOpenedEventArgs>? ChatRoomOpened;

        public void OnClosing(object? sender, WindowEventArgs e)
        {
            ViewModel.OnClosing();
        }

        public void StoppedHosting(object? sender, WindowEventArgs e)
        {
            ViewModel.StoppedHosting();
        }

        private void MessageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                ViewModel.HandleMessage(user, button);
            }
        }

        private void SendFriendRequestButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                ViewModel.ToggleFriendRequest(user, button);
            }
        }

        private void AcceptChatInviteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                ViewModel.AcceptInvite(user);
            }
        }

        private void DeclineChatInviteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is User user)
            {
                ViewModel.DeclineInvite(user);
            }
        }
    }
    // Event arguments for when a chat room is opened
    public class ChatRoomOpenedEventArgs : EventArgs
    {
        public string Username { get; set; }
        public string IpAddress { get; set; }
    }
}

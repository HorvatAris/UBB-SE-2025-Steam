using System;
using System.Diagnostics;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class AddFriendsPage : Page
    {
        private readonly AddFriendsViewModel friendsViewModel;
        private readonly UsersViewModel usersViewModel;

        public AddFriendsPage()
        {
            InitializeComponent();
            friendsViewModel = App.AddFriendsViewModel;
            usersViewModel = App.UsersViewModel;
            DataContext = friendsViewModel;

            // Load friends immediately when page is created
            friendsViewModel.LoadFriends();
        }

        private async void AddFriend_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.Tag is int friendshipId)
            {
                friendsViewModel.AddFriend(friendshipId);
            }
        }

        private async void RemoveFriend_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.Tag is int friendshipId)
            {
                friendsViewModel.RemoveFriend(friendshipId);
            }
        }

        private void BackToProfileButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(ProfilePage), usersViewModel.GetCurrentUser().UserId);
        }
    }
}
using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;

namespace SteamHub.Pages
{
    public sealed partial class AddFriendsPage : Page
    {
        private readonly AddFriendsViewModel addFriendsViewModel;

        public AddFriendsPage(IFriendsService friendsService, IUserService userService)
        {
            InitializeComponent();
            addFriendsViewModel = new AddFriendsViewModel(friendsService, userService);
            DataContext = addFriendsViewModel;

            // Load friends immediately when page is created

            addFriendsViewModel.LoadFriendsAsync();
        }

        private async void AddFriend_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.Tag is int friendshipId)
            {
                await addFriendsViewModel.AddFriendAsync(friendshipId);
            }
        }

        private async void RemoveFriend_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (sender is Button button && button.Tag is int friendshipId)
            {
               await addFriendsViewModel.RemoveFriendAsync(friendshipId);
            }
        }

        private void BackToProfileButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            // should navigate to the profile page of the current user
            // Frame.Navigate(typeof(ProfilePage), addFriendsViewModel.GetCurrentUser().UserId);
        }
    }
}
using System;
using SteamHub.ViewModels;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ViewModels;

namespace SteamHub.Pages
{
    public sealed partial class FriendRequestsPage : Page
    {
        public ProfileViewModel ProfileViewModel { get; set; }
        public FriendRequestViewModel FriendRequestViewModel { get; set; }

        public FriendRequestsPage(ProfileViewModel profileViewModel, FriendRequestViewModel friendRequestViewModel)
        {
            // Get view models from DI container (same as in ProfileControl)
            FriendRequestViewModel = friendRequestViewModel;

            ProfileViewModel = profileViewModel;

            this.InitializeComponent();
        }
    }
}
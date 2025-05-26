using System;
using SteamProfile.ViewModels;
using Microsoft.UI.Xaml.Controls;
using BusinessLayer.Models;

namespace SteamProfile.Views
{
    public sealed partial class FriendRequestsPage : Page
    {
        public ProfileViewModel ProfileViewModel { get; }
        public FriendRequestViewModel FriendRequestViewModel { get; }

        public FriendRequestsPage()
        {
            // Get view models from DI container (same as in ProfileControl)
            FriendRequestViewModel = SteamProfile.App.GetService<FriendRequestViewModel>();

            // Create profile view model (it will use FriendRequestViewModel internally)
            ProfileViewModel = new SteamProfile.ViewModels.ProfileViewModel();

            this.InitializeComponent();
        }
    }
}
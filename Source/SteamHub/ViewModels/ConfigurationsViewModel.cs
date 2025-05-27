using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Services;
using SteamHub.Pages;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels
{
    public partial class ConfigurationsViewModel : ObservableObject
    {
        private readonly Frame frame;
        private readonly IUserService userService;

        public ConfigurationsViewModel(Frame frame, IUserService userService)
        {
            this.frame = frame;
            this.userService = userService ?? throw new NullReferenceException(nameof(userService));
        }

        [RelayCommand]
        private void NavigateToFeatures()
        {
            frame.Navigate(typeof(FeaturesPage));
        }

        [RelayCommand]
        private void NavigateToProfile()
        {
            frame.Navigate(typeof(ProfilePage), userService.GetCurrentUser().UserId);
        }
        [RelayCommand]
        private void NavigateToProfileSettings()
        {
            frame.Navigate(typeof(ModifyProfilePage));
        }
        [RelayCommand]

        private void NavigateToAccountSettings()
        {
            frame.Navigate(typeof(AccountSettingsPage));
        }
    }
}

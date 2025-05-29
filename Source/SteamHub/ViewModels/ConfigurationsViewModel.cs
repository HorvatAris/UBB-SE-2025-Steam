using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Services;
using SteamHub.Pages;
using SteamHub.ApiContract.Services.Interfaces;
using System.Threading.Tasks;

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
            //  Change like the rest
            frame.Navigate(typeof(FeaturesPage));
        }

        [RelayCommand]
        private void NavigateToProfileSettings()
        {
            frame.Content = new ModifyProfilePage(this.frame, this.userService);
        }

        [RelayCommand]
        private void NavigateToAccountSettings()
        {
            frame.Content = new AccountSettingsPage(this.frame, this.userService);
        }
    }
}

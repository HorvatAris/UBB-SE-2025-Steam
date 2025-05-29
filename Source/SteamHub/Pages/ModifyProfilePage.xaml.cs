using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace SteamHub.Pages
{
    public sealed partial class ModifyProfilePage : Page
    {
        public ModifyProfileViewModel ViewModel { get; private set; }
        private Frame frame;
        private IUserService userService;
        public ModifyProfilePage(Frame frame, IUserService userService)
        {
            this.InitializeComponent();
            this.frame = frame;
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            // Initialize the ViewModel with the Frame
            ViewModel = new ModifyProfileViewModel(userService, frame);
            DataContext = ViewModel;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            frame.Content = new ConfigurationsPage(this.userService, this.frame);
        }
    }
}
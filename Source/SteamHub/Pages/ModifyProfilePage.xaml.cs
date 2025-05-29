using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace SteamHub.Pages
{
    public sealed partial class ModifyProfilePage : Page
    {
        public ModifyProfileViewModel ViewModel { get; private set; }

        public ModifyProfilePage()
        {
            this.InitializeComponent();

            // Initialize the ViewModel with the Frame
            ViewModel = new ModifyProfileViewModel(this.Frame);
            DataContext = ViewModel;
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}
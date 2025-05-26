using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels.ConfigurationsViewModels;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace SteamProfile.Views.ConfigurationsView
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
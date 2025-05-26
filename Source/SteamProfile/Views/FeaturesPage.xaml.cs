using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

namespace SteamProfile.Views
{
    public sealed partial class FeaturesPage : Page
    {
        private readonly FeaturesViewModel featuresViewModel;

        public FeaturesPage()
        {
            this.InitializeComponent();
            featuresViewModel = new FeaturesViewModel(App.FeaturesService, App.UserService);
            this.DataContext = featuresViewModel;
            this.Loaded += FeaturesPage_Loaded;
        }

        private void FeaturesPage_Loaded(object sender, RoutedEventArgs routedEventArguments)
        {
            featuresViewModel.Initialize(this.XamlRoot);
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs tappedRoutedEventArguments)
        {
            if (sender is FrameworkElement element && element.DataContext is FeatureDisplay feature)
            {
                featuresViewModel.ShowPreview(feature);
            }
        }

        private void Image_ImageFailed(object sender, ExceptionRoutedEventArgs exceptionRoutedEventArguments)
        {
            var image = sender as Image;
            if (image != null)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to load image. Error: {exceptionRoutedEventArguments.ErrorMessage}");
            }
        }

        private void BackToProfileButton_Click(object sender, RoutedEventArgs routedEventArguments)
        {
            // Get the current user ID from the service or ViewModel
            int userId = App.UserService.GetCurrentUser().UserId;

            // If we can navigate back (in case we came from Profile)
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
            else
            {
                // Navigate directly to ProfilePage with the user ID
                this.Frame.Navigate(typeof(ProfilePage), userId);
            }
        }
    }
}

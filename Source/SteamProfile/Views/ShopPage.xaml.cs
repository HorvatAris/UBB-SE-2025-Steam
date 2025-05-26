using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace SteamProfile.Views
{
    public sealed partial class ShopPage : Page
    {
        public ShopPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);
            if (eventArgs.Parameter is int featureId)
            {
                // Initialize shop with specific feature
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            if (this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
            }
        }
    }
}
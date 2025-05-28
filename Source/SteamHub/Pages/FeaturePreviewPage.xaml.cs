using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ViewModels;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Pages
{
    public sealed partial class FeaturePreviewPage : Page
    {
        public FeaturesViewModel ViewModel { get; }

        public FeaturePreviewPage()
        {
            this.InitializeComponent();
            // Inject real services and user
            var featuresService = App.FeaturesService;
            var userService = App.UserService;
            var currentUser = App.CurrentUser;
            ViewModel = new FeaturesViewModel(featuresService, userService, currentUser);
            this.DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is FeatureDisplay featureDisplay)
            {
                ViewModel.SelectedFeature = featureDisplay;
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Frame.CanGoBack)
            {
                Frame.GoBack();
            }
        }
    }
} 
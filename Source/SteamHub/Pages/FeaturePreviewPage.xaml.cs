using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ViewModels;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System.Runtime.CompilerServices;

namespace SteamHub.Pages
{
    public sealed partial class FeaturePreviewPage : Page
    {
        public FeaturesViewModel ViewModel { get; }
        private IUserService userService;
        private IFeaturesService featuresService;
        private User currentUser;
        private Frame frame;

        public FeaturePreviewPage(IUserService userService, IFeaturesService featuresService, Frame frame, FeatureDisplay feature, User user)
        {
            this.InitializeComponent();
            // Inject real services and user
            this.featuresService = featuresService;
            this.userService = userService;
            this.frame = frame;
            this.currentUser = user;
            ViewModel = new FeaturesViewModel(featuresService, userService, currentUser, frame);
            ViewModel.SelectedFeature = feature;
            // OnNavigatedTo(null);
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
            if (frame.CanGoBack)
            {
                frame.Content = new FeaturesPage(featuresService, userService, this.currentUser, frame);
            }
        }
    }
} 
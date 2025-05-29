using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ViewModels;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.Pages
{
    public sealed partial class FeaturesPage : Page
    {
        public FeaturesViewModel ViewModel { get; }

        public FeaturesPage(IFeaturesService featuresService, IUserService userService, User currentUser, Frame frame)
        {
            this.InitializeComponent();
            ViewModel = new FeaturesViewModel(featuresService, userService, currentUser, frame);
            this.DataContext = ViewModel;
            ViewModel.LoadFeaturesAsync();
            // OnNavigatedTo(null);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
    }
} 
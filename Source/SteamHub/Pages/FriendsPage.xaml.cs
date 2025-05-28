using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ViewModels;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SteamHub.Pages
{
    /// <summary>
    ///
    /// </summary>
    public sealed partial class FriendsPage : Page
    {
        public FriendsViewModel ViewModel { get; }

        public FriendsPage(IFriendsService friendsService)
        {
            this.InitializeComponent();
            ViewModel = new FriendsViewModel(friendsService);
            this.DataContext = ViewModel;
        }

        private void FriendsPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.InitializeAsync().ConfigureAwait(false);
        }
    }
}

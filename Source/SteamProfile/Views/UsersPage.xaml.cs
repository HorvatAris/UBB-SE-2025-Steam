using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SteamProfile.ViewModels;

namespace SteamProfile.Views
{
    public sealed partial class UsersPage : Page
    {
        public UsersViewModel UsersViewModel { get; }

        public UsersPage()
        {
            UsersViewModel = UsersViewModel.Instance;
            this.InitializeComponent();
            UsersViewModel = UsersViewModel.Instance;
            this.DataContext = UsersViewModel;
        }
    }
}
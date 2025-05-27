using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Pages.WalletViews;
using SteamHub.ViewModels;

namespace SteamHub.Pages
{
    public sealed partial class WalletPage : Page
    {
        private WalletViewModel ViewModel { get; set; }

        public WalletPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is IWalletService walletService)
            {
                ViewModel = new WalletViewModel(walletService);
                this.DataContext = ViewModel;
                ViewModel.RefreshWalletData();
            }
        }

        public WalletPage(IWalletService walletService)
        {
            this.InitializeComponent();
            ViewModel = new WalletViewModel(walletService);
            this.DataContext = ViewModel;
        }

        private void AddMoneyButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(AddMoneyPage), ViewModel);
        }

        //private void GoBack(object sender, RoutedEventArgs eventArgs)
        //{
        //    Frame.;
        //}
    }
}
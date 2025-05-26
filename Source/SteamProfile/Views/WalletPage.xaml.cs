using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;
using SteamProfile.Views.WalletViews;
using BusinessLayer.Services;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Repositories;

namespace SteamProfile.Views
{
    public sealed partial class WalletPage : Page
    {
        private WalletViewModel ViewModel { get; }

        public WalletPage()
        {
            this.InitializeComponent();
            ViewModel = new WalletViewModel(App.WalletService);
            this.DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);

            // Refresh wallet data when navigating to this page
            ViewModel.RefreshWalletData();
        }

        private void AddMoneyButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.Navigate(typeof(AddMoneyPage), ViewModel);
        }

        private void GoBack(object sender, RoutedEventArgs eventArgs)
        {
            Frame.GoBack();
        }
    }
}
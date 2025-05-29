using System;
using System.Diagnostics;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.Pages.WalletViews;
using SteamHub.ViewModels;
using SteamHub.ApiContract.Models.User;

namespace SteamHub.Pages
{
    public sealed partial class WalletPage : Page
    {
        private WalletViewModel ViewModel { get; set; }

        public WalletPage()
        {
            this.InitializeComponent();
            Debug.WriteLine("WalletPage constructor called");
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Debug.WriteLine("WalletPage OnNavigatedTo called");

            try
            {
                if (e.Parameter is (IWalletService walletService, IUserService userService))
                {
                    //Debug.WriteLine($"Creating WalletViewModel for user: {loggedInUser.Username}");
                    ViewModel = new WalletViewModel(walletService, userService);
                    this.DataContext = ViewModel;
                    
                    Debug.WriteLine("Initializing WalletViewModel");
                    await ViewModel.InitializeAsync();
                }
                else
                {
                    Debug.WriteLine($"Invalid navigation parameter type: {e.Parameter?.GetType().Name ?? "null"}");
                    ShowError("Invalid navigation parameters");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in OnNavigatedTo: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                ShowError($"Error loading wallet: {ex.Message}");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Debug.WriteLine("WalletPage OnNavigatedFrom called");
        }

        private void ShowError(string message)
        {
            // You might want to add a TextBlock in your XAML for error messages
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }

        private void AddMoneyButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            Debug.WriteLine("Navigating to AddMoneyPage");
            Frame.Navigate(typeof(AddMoneyPage), ViewModel);
        }

        //private void GoBack(object sender, RoutedEventArgs eventArgs)
        //{
        //    Frame.;
        //}
    }
}
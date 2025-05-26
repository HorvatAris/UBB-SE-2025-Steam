using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using BusinessLayer.Models;
using SteamProfile.ViewModels;

namespace SteamProfile.Views.WalletViews
{
    public sealed partial class PaypalPaymentPage : Page
    {
        public PaypalPaymentViewModel ViewModel { get; } = new PaypalPaymentViewModel();

        public PaypalPaymentPage()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);

            if (eventArgs.Parameter is Dictionary<string, object> parameters)
            {
                ViewModel.Initialize(parameters);
            }
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs eventArgs)
        {
            ViewModel.ValidateEmail(EmailTextBox.Text);
        }

        private void PasswordText_PasswordChanged(object sender, RoutedEventArgs eventArgs)
        {
            ViewModel.ValidatePassword(PasswordText.Password);
        }

        private async void AddMoneyButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            bool success = await ViewModel.ProcessPaymentAsync();
            if (success)
            {
                Frame.GoBack();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs eventArgs)
        {
            Frame.GoBack();
        }
    }
}
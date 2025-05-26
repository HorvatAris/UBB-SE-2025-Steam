using System.Collections.Generic;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;

namespace SteamProfile.Views.WalletViews
{
    public sealed partial class CardPaymentPage : Page
    {
        private CardPaymentViewModel ViewModel { get; set; }

        public CardPaymentPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);

            if (eventArgs.Parameter is Dictionary<string, object> parameters)
            {
                // Create and initialize the ViewModel
                ViewModel = new CardPaymentViewModel();
                this.DataContext = ViewModel;

                // Initialize ViewModel with navigation parameters
                ViewModel.Initialize(parameters);
            }
        }

        private void ValidName(object sender, TextChangedEventArgs eventArgs)
        {
            ViewModel.ValidateName(OwnerNameTextBox.Text);
        }

        private void ValidNumber(object sender, TextChangedEventArgs eventArgs)
        {
            ViewModel.ValidateCardNumber(CardNumberTextBox.Text);
        }

        private void ValidCardVerificationValue(object sender, TextChangedEventArgs eventArgs)
        {
            ViewModel.ValidateCardVerificationValue(CardVerificationValueTextBox.Text);
        }

        private void ValidDate(object sender, TextChangedEventArgs eventArgs)
        {
            ViewModel.ValidateExpirationDate(ExpirationDateTextBox.Text);
        }

        private async void AddMoneyToAccount(object sender, RoutedEventArgs eventArgs)
        {
            if (await ViewModel.ProcessPaymentAsync())
            {
                Frame.GoBack();
            }
        }

        private void CancelPayment(object sender, RoutedEventArgs eventArgs)
        {
            Frame.GoBack();
        }
    }
}
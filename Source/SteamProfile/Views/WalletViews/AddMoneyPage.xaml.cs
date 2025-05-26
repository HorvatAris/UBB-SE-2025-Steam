using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SteamProfile.ViewModels;

namespace SteamProfile.Views.WalletViews
{
    public sealed partial class AddMoneyPage : Page
    {
        private AddMoneyViewModel ViewModel { get; set; }

        public AddMoneyPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs eventArgs)
        {
            base.OnNavigatedTo(eventArgs);

            if (eventArgs.Parameter is WalletViewModel walletViewModel)
            {
                ViewModel = new AddMoneyViewModel(walletViewModel);
                this.DataContext = ViewModel;
            }
        }

        private void SumToBeAddedTextBox_TextChanged(object sender, TextChangedEventArgs eventArgs)
        {
            ViewModel.ValidateInput(sumToBeAddedTextBox.Text);
        }

        private void UseCardForPayment(object sender, RoutedEventArgs eventArgs)
        {
            if (ViewModel.IsInputValid)
            {
                Frame.Navigate(typeof(CardPaymentPage), ViewModel.CreateNavigationParameters());
            }
        }

        private void UsePaypalForPayment(object sender, RoutedEventArgs eventArgs)
        {
            if (ViewModel.IsInputValid)
            {
                Frame.Navigate(typeof(PaypalPaymentPage), ViewModel.CreateNavigationParameters());
            }
        }

        private void Cancel(object sender, RoutedEventArgs eventArgs)
        {
            Frame.GoBack();
        }
    }
}
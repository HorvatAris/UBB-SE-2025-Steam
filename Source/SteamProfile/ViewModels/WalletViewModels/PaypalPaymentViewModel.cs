using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using BusinessLayer.Models;
using BusinessLayer.Validators;

namespace SteamProfile.ViewModels
{
    public partial class PaypalPaymentViewModel : ObservableObject
    {
        private const string AmountKey = "amount";
        private const string ViewModelKey = "viewModel";
        private const string UserKey = "user";

        [ObservableProperty]
        private int amount;

        [ObservableProperty]
        private WalletViewModel walletViewModel;

        [ObservableProperty]
        private User user;

        [ObservableProperty]
        private string amountText;

        [ObservableProperty]
        private string email = string.Empty;

        [ObservableProperty]
        private bool isEmailValid;

        [ObservableProperty]
        private bool isPasswordValid;

        [ObservableProperty]
        private bool showErrorMessage;

        [ObservableProperty]
        private string statusMessage;

        [ObservableProperty]
        private Visibility statusMessageVisibility = Visibility.Collapsed;

        public Visibility ErrorMessageVisibility
        {
            get { return ShowErrorMessage ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool AreAllFieldsValid
        {
            get { return IsEmailValid && IsPasswordValid; }
        }

        public PaypalPaymentViewModel()
        {
            ShowErrorMessage = false;
        }

        public void Initialize(int paymentAmount, User currentUser, WalletViewModel currentWalletViewModel)
        {
            Amount = paymentAmount;
            User = currentUser;
            WalletViewModel = currentWalletViewModel;
            AmountText = $"Sum: {Amount}";
        }

        public void Initialize(Dictionary<string, object> parameters)
        {
            Amount = parameters.ContainsKey(AmountKey) ? (int)parameters[AmountKey] : 0;

            WalletViewModel = parameters.ContainsKey(ViewModelKey) ? (WalletViewModel)parameters[ViewModelKey] : null;

            User = parameters.ContainsKey(UserKey) ? (User)parameters[UserKey] : null;

            AmountText = $"Sum: {Amount}";
        }

        partial void OnShowErrorMessageChanged(bool oldValue, bool newValue)
        {
            OnPropertyChanged(nameof(ErrorMessageVisibility));
        }

        public void ValidateEmail(string emailAddress)
        {
            IsEmailValid = PaymentValidator.IsEmailValid(emailAddress);
            ShowErrorMessage = !IsEmailValid;
        }

        public void ValidatePassword(string password)
        {
            IsPasswordValid = PaymentValidator.IsPasswordValid(password);
            ShowErrorMessage = !IsPasswordValid;
        }

        public async Task<bool> ProcessPaymentAsync()
        {
            if (!AreAllFieldsValid)
            {
                ShowErrorMessage = true;
                StatusMessage = "Please correct the invalid fields.";
                StatusMessageVisibility = Visibility.Visible;
                return false;
            }

            ShowErrorMessage = false;
            StatusMessageVisibility = Visibility.Visible;
            StatusMessage = "Processing...";

            await Task.Delay(1000);

            WalletViewModel?.AddFunds(Amount);

            StatusMessage = "Payment was performed successfully";

            await Task.Delay(5000);

            return true;
        }
    }
}
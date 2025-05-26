using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using BusinessLayer.Models;
using BusinessLayer.Validators;

namespace SteamProfile.ViewModels
{
    public partial class CardPaymentViewModel : ObservableObject
    {
        private const string AmountKey = "amount";
        private const string ViewModelKey = "viewModel";
        private const string UserKey = "user";

        private int amount;
        private WalletViewModel walletViewModel;
        private User user;

        [ObservableProperty]
        private string amountText;

        [ObservableProperty]
        private bool isNameValid;

        [ObservableProperty]
        private bool isCardNumberValid;

        [ObservableProperty]
        private bool isCardVerificationValueValid;

        [ObservableProperty]
        private bool isDateValid;

        [ObservableProperty]
        private bool showErrorMessage;

        [ObservableProperty]
        private string statusMessage;

        [ObservableProperty]
        private Visibility statusMessageVisibility;

        public Visibility ErrorMessageVisibility
        {
            get { return ShowErrorMessage ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool AreAllFieldsValid
        {
            get { return IsNameValid && IsCardNumberValid && IsCardVerificationValueValid && IsDateValid; }
        }

        // Initialization
        public CardPaymentViewModel()
        {
            // Initialize properties in constructor
            statusMessageVisibility = Visibility.Collapsed;
            showErrorMessage = false;
            AmountText = "Sum: 0";
        }

        public void Initialize(Dictionary<string, object> parameters)
        {
            amount = parameters.ContainsKey(AmountKey) ? (int)parameters[AmountKey] : 0;

            walletViewModel = parameters.ContainsKey(ViewModelKey) ? (WalletViewModel)parameters[ViewModelKey] : null;

            user = parameters.ContainsKey(UserKey) ? (User)parameters[UserKey] : null;

            AmountText = $"Sum: {amount}";
        }

        public void ValidateName(string name)
        {
            IsNameValid = PaymentValidator.IsCardNameValid(name);
        }

        public void ValidateCardNumber(string cardNumber)
        {
            IsCardNumberValid = PaymentValidator.IsCardNumberValid(cardNumber);
        }

        public void ValidateCardVerificationValue(string cardVerificationValue)
        {
            IsCardVerificationValueValid = PaymentValidator.IsCardVerificationValueValid(cardVerificationValue);
        }

        public void ValidateExpirationDate(string expirationDate)
        {
            IsDateValid = PaymentValidator.IsExpirationDateValid(expirationDate);
        }

        partial void OnIsNameValidChanged(bool value)
        {
            UpdateErrorMessageVisibility();
            OnPropertyChanged(nameof(AreAllFieldsValid));
        }
        partial void OnIsCardNumberValidChanged(bool value)
        {
            UpdateErrorMessageVisibility();
            OnPropertyChanged(nameof(AreAllFieldsValid));
        }
        partial void OnIsCardVerificationValueValidChanged(bool value)
        {
            UpdateErrorMessageVisibility();
            OnPropertyChanged(nameof(AreAllFieldsValid));
        }
        partial void OnIsDateValidChanged(bool value)
        {
            UpdateErrorMessageVisibility();
            OnPropertyChanged(nameof(AreAllFieldsValid));
        }

        private void UpdateErrorMessageVisibility()
        {
            ShowErrorMessage = !AreAllFieldsValid;
            OnPropertyChanged(nameof(ErrorMessageVisibility));
        }

        // Payment Processing
        public async Task<bool> ProcessPaymentAsync()
        {
            // Final check before processing
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

            if (walletViewModel != null)
            {
                walletViewModel.AddFunds(amount);
            }
            else
            {
                StatusMessage = "Error: Wallet context not found.";
                await Task.Delay(5000);
                return false;
            }

            StatusMessage = "Payment was performed successfully";

            await Task.Delay(5000);

            return true;
        }
    }
}
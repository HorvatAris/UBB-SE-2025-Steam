using System;
using System.Collections.Generic;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using BusinessLayer.Models;
using BusinessLayer.Validators;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SteamProfile.ViewModels
{
    public partial class AddMoneyViewModel : ObservableObject
    {
        private const string AmountNavigationKey = "amount";
        private const string ViewModelNavigationKey = "viewModel";

        private readonly WalletViewModel walletViewModel;
        private const int MAXIMUM_MONEY_AMOUNT = 500;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ErrorMessageVisibility))]
        private bool showErrorMessage;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(PaymentButtonsEnabled))]
        private bool isInputValid;

        [ObservableProperty]
        private string amountToAdd;

        public ICommand AddFundsCommand { get; }

        public Visibility ErrorMessageVisibility
        {
            get
            {
                return ShowErrorMessage ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool PaymentButtonsEnabled
        {
            get { return IsInputValid; }
        }

        public AddMoneyViewModel(WalletViewModel walletViewModel)
        {
            this.walletViewModel = walletViewModel ?? throw new ArgumentNullException(nameof(walletViewModel));
            isInputValid = false;
            showErrorMessage = false;
            amountToAdd = string.Empty;

            AddFundsCommand = new RelayCommand(ProcessAddFunds, CanProcessAddFunds);
        }

        partial void OnAmountToAddChanged(string value)
        {
            ValidateInput(value);
            (AddFundsCommand as RelayCommand)?.NotifyCanExecuteChanged();
        }

        public void ValidateInput(string input)
        {
            ShowErrorMessage = false;
            IsInputValid = false;

            if (string.IsNullOrEmpty(input))
            {
                return;
            }

            IsInputValid = PaymentValidator.IsMonetaryAmountValid(input, MAXIMUM_MONEY_AMOUNT);
            ShowErrorMessage = !IsInputValid;
        }

        private void ProcessAddFunds()
        {
            if (!IsInputValid || string.IsNullOrEmpty(AmountToAdd))
            {
                ShowErrorMessage = true;
                return;
            }
        }

        private bool CanProcessAddFunds()
        {
            return IsInputValid;
        }

        public Dictionary<string, object> CreateNavigationParameters()
        {
            if (!IsInputValid || !int.TryParse(AmountToAdd, out int amountValue))
            {
                return new Dictionary<string, object>();
            }

            return new Dictionary<string, object>
            {
                 { AmountNavigationKey, amountValue },
                 { ViewModelNavigationKey, walletViewModel }
            };
        }
    }
}
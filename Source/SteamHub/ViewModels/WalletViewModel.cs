using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels
{
    public partial class WalletViewModel : ObservableObject
    {
        private readonly IWalletService walletService;
        private readonly IUserDetails user;

        public WalletViewModel(IWalletService walletService) 
        {
            this.walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            this.user = walletService.GetUser();
            RefreshWalletData();
        }

        [ObservableProperty]
        private decimal balance;

        [ObservableProperty]
        private int points;

        private int walletId;

        public string BalanceText
        {
            get { return $"${Balance:F2}"; }
        }

        public string PointsText
        {
            get { return $"{Points} points"; }
        }

        partial void OnBalanceChanged(decimal value)
        {
            OnPropertyChanged(nameof(BalanceText));
        }

        [RelayCommand]
        public async void RefreshWalletData()
        {
            Balance = await walletService.GetBalance(user.UserId);
            Points = await walletService.GetPoints(user.UserId);
        }

        [RelayCommand]
        public async Task AddFunds(decimal amount)
        {
            if (amount <= 0)
            {
                return;
            }

            await walletService.AddMoney(amount, user.UserId);
            RefreshWalletData();
        }
    }
}
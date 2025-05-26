using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using BusinessLayer.Models;
using BusinessLayer.Repositories;
using BusinessLayer.Services;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Repositories.Interfaces;

namespace SteamProfile.ViewModels
{
    public partial class WalletViewModel : ObservableObject
    {
        private readonly IWalletService walletService;

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

        public WalletViewModel(IWalletService walletService)
        {
            this.walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            RefreshWalletData();
        }

        partial void OnBalanceChanged(decimal value)
        {
            OnPropertyChanged(nameof(BalanceText));
        }

        [RelayCommand]
        public void RefreshWalletData()
        {
            Balance = walletService.GetBalance();
            Points = walletService.GetPoints();
        }

        [RelayCommand]
        public void AddFunds(decimal amount)
        {
            if (amount <= 0)
            {
                return;
            }

            walletService.AddMoney(amount);
            RefreshWalletData();
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels
{
    public partial class WalletViewModel : ObservableObject
    {
        private readonly IWalletService walletService;
        private readonly IUserService userService;

        public WalletViewModel(IWalletService walletService, IUserService userService) 
        {
            this.walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [ObservableProperty]
        private decimal balance;

        [ObservableProperty]
        private int points;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage;

        public string BalanceText => $"${Balance:F2}";
        public string PointsText => $"{Points} points";

        partial void OnBalanceChanged(decimal value)
        {
            OnPropertyChanged(nameof(BalanceText));
        }

        partial void OnPointsChanged(int value)
        {
            OnPropertyChanged(nameof(PointsText));
        }


        public async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                await RefreshWalletDataAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error loading wallet data. Please try again.";
                Debug.WriteLine($"Error in InitializeAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task RefreshWalletDataAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var currentUser = await this.userService.GetCurrentUserAsync();

                Debug.WriteLine($"Refreshing wallet data for user: {currentUser.Username} (ID: {currentUser.UserId})");
                
                // Make parallel API calls for better performance
                var balanceTask = walletService.GetBalance(currentUser.UserId);
                var pointsTask = walletService.GetPoints(currentUser.UserId);

                await Task.WhenAll(balanceTask, pointsTask);

                Balance = await balanceTask;
                Points = await pointsTask;

                Debug.WriteLine($"Wallet refreshed for {currentUser.Username} - Balance: {Balance}, Points: {Points}");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error refreshing wallet data";
                Debug.WriteLine($"Error in RefreshWalletDataAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        public async Task AddFunds(decimal amount)
        {
            if (amount <= 0)
            {
                return;
            }

            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var currentUser = await this.userService.GetCurrentUserAsync();

                Debug.WriteLine($"Adding {amount:C} to wallet for user: {currentUser.Username} (ID: {currentUser.UserId})");
                await walletService.AddMoney(amount, currentUser.UserId);
                await RefreshWalletDataAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error adding funds";
                Debug.WriteLine($"Error in AddFunds: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
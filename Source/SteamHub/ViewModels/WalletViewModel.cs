using System;
using System.Threading.Tasks;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ViewModels
{
    public partial class WalletViewModel : BaseViewModel
    {
        private readonly IWalletService walletService;

        public WalletViewModel(IWalletService walletService, IUserService userService, User currentUser) 
            : base(userService, currentUser)
        {
            this.walletService = walletService ?? throw new ArgumentNullException(nameof(walletService));
            Debug.WriteLine($"WalletViewModel initialized for user: {currentUser.Username}");
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

        protected override void OnUserChanged()
        {
            base.OnUserChanged();
            Debug.WriteLine($"User changed in WalletViewModel - refreshing wallet data for user: {CurrentUser.Username}");
            _ = RefreshWalletDataAsync();
        }

        public async Task InitializeAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                Debug.WriteLine($"Starting WalletViewModel initialization for user: {CurrentUser.Username}");
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

                Debug.WriteLine($"Refreshing wallet data for user: {CurrentUser.Username} (ID: {CurrentUser.UserId})");
                
                // Make parallel API calls for better performance
                var balanceTask = walletService.GetBalance(CurrentUser.UserId);
                var pointsTask = walletService.GetPoints(CurrentUser.UserId);

                await Task.WhenAll(balanceTask, pointsTask);

                Balance = await balanceTask;
                Points = await pointsTask;

                Debug.WriteLine($"Wallet refreshed for {CurrentUser.Username} - Balance: {Balance}, Points: {Points}");
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
                
                Debug.WriteLine($"Adding {amount:C} to wallet for user: {CurrentUser.Username} (ID: {CurrentUser.UserId})");
                await walletService.AddMoney(amount, CurrentUser.UserId);
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
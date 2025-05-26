using System;
using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Services.Proxies
{
    public class WalletServiceProxy : ServiceProxy, IWalletService
    {
        private readonly IUserService userService;

        public WalletServiceProxy(IUserService userService, string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public void CreateWallet(int userIdentifier)
        {
            try
            {
                PostAsync($"Wallet/create/{userIdentifier}", null).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to create wallet: {ex.Message}", ex);
            }
        }

        public decimal GetBalance()
        {
            try
            {
                var userId = userService.GetCurrentUser()?.UserId ??
                    throw new InvalidOperationException("User is not logged in");

                var wallet = GetAsync<WalletInfo>($"Wallet/{userId}").GetAwaiter().GetResult();
                return wallet.Balance;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to get balance: {ex.Message}", ex);
            }
        }

        public int GetPoints()
        {
            try
            {
                var userId = userService.GetCurrentUser()?.UserId ??
                    throw new InvalidOperationException("User is not logged in");

                var wallet = GetAsync<WalletInfo>($"Wallet/{userId}").GetAwaiter().GetResult();
                return wallet.Points;
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to get points: {ex.Message}", ex);
            }
        }

        public void AddMoney(decimal amount)
        {
            try
            {
                var userId = userService.GetCurrentUser()?.UserId ??
                    throw new InvalidOperationException("User is not logged in");

                PostAsync("Wallet/add-money", new { UserId = userId, Amount = amount })
                    .GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to add money: {ex.Message}", ex);
            }
        }

        public void CreditPoints(int userId, int numberOfPoints)
        {
            try
            {
                // userId is now passed as a parameter, no need to get it from userService
                PostAsync($"Wallet/credit-points/{userId}", new { NumberOfPoints = numberOfPoints })
                    .GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to credit points: {ex.Message}", ex);
            }
        }

        public void BuyWithMoney(decimal amount, int userId)
        {
            try
            {
                // Call the API endpoint to buy with money for the specified user
                PostAsync("Wallet/buy-with-money", new { UserId = userId, Amount = amount })
                    .GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                throw new ServiceException($"Failed to buy with money: {ex.Message}", ex);
            }
        }
    }

    // Helper class for wallet information
    public class WalletInfo
    {
        public int UserId { get; set; }
        public decimal Balance { get; set; }
        public int Points { get; set; }
    }
}
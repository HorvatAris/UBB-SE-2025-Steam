﻿
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Services.Interfaces;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SteamHub.ApiContract.ServiceProxies
{
    public class WalletServiceProxy : ServiceProxy, IWalletService
    {
        public WalletServiceProxy(string baseUrl = "http://172.30.245.56:8000/api/") : base(baseUrl)
        {

        }

        private const int InitialZeroSum = 0;

        public async Task CreateWallet(int userIdentifier)
        {
            try
            {
                await PostAsync($"Wallet/create/{userIdentifier}", null);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to create wallet: {ex.Message}", ex);
            }
        }

        public async Task<decimal> GetBalance(int userId)
        {
            try
            {
                var wallet = await GetAsync<WalletInfo>($"Wallet/{userId}");
                return wallet.Balance;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get balance: {ex.Message}", ex);
            }
        }

        public async Task<int> GetPoints(int userId)
        {
            try
            {
                var wallet = await GetAsync<WalletInfo>($"Wallet/{userId}");
                return wallet.Points;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get points: {ex.Message}", ex);
            }
        }

        public async Task AddMoney(decimal amount, int userId)
        {
            try
            {
                await PostAsync("Wallet/add-money", new { UserId = userId, Amount = amount });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add money: {ex.Message}", ex);
            }
        }

        public async Task CreditPoints(int userId, int numberOfPoints)
        {
            try
            {
                // userId is now passed as a parameter, no need to get it from userService
                await PostAsync($"Wallet/credit-points/{userId}", new { NumberOfPoints = numberOfPoints });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to credit points: {ex.Message}", ex);
            }
        }

        public async Task BuyWithMoney(decimal amount, int userId)
        {
            try
            {
                // Call the API endpoint to buy with money for the specified user
                await PostAsync("Wallet/buy-with-money", new { UserId = userId, Amount = amount });
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to buy with money: {ex.Message}", ex);
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
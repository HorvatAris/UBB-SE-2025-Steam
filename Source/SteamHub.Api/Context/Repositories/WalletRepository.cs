using System;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SteamHub.ApiContract.Models;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Context.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly DataContext context;

        public WalletRepository(DataContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public async Task<Wallet> GetWallet(int walletId)
        {
            var walletEntity = await context.Wallets.FindAsync(walletId);
            if (walletEntity == null)
            {
                throw new Exception($"Wallet with ID {walletId} not found.");
            }

            // Map the entity to the model
            return new Wallet
            {
                WalletId = walletEntity.WalletId,
                UserId = walletEntity.UserId,
                Balance = walletEntity.Balance,
                Points = walletEntity.Points
            };
        }

        public async Task<int> GetWalletIdByUserId(int userId)
        {
            var walletEntity = await context.Wallets
                .Where(w => w.UserId == userId)
                .Select(w => w.WalletId)
                .FirstOrDefaultAsync();
            if (walletEntity == 0)
            {
                throw new Exception($"Wallet for user with ID {userId} not found.");
            }
            return walletEntity;
        }

        public async Task AddMoneyToWallet(decimal moneyToAdd, int userId)
        {
            var walletEntity = await context.Wallets.SingleOrDefaultAsync(w => w.UserId == userId)
                ?? throw new Exception($"No wallet for user {userId}");
            walletEntity.Balance += moneyToAdd;
            context.SaveChanges();
        }

        public async Task AddPointsToWallet(int pointsToAdd, int userId)
        {
            var walletEntity = await context.Wallets.SingleOrDefaultAsync(w => w.UserId == userId)
                ?? throw new Exception($"No wallet for user {userId}");
            walletEntity.Points += pointsToAdd;
            context.SaveChanges();
        }

        public async Task<decimal> GetMoneyFromWallet(int walletId)
        {
            var wallet = await GetWallet(walletId);
            return wallet.Balance;
        }

        public async Task<int> GetPointsFromWallet(int walletId)
        {
            var wallet = await GetWallet(walletId);
            return wallet.Points;
        }

        public async Task BuyWithMoney(decimal amount, int userId)
        {
            var walletEntity =  await context.Wallets.SingleOrDefaultAsync(w => w.UserId == userId)
                ?? throw new Exception($"No wallet for user {userId}");
            walletEntity.Balance -= amount;
            context.SaveChanges();
        }

        public async Task BuyWithPoints(int amount, int userId)
        {
            var walletEntity = await context.Wallets.SingleOrDefaultAsync(w => w.UserId == userId)
                ?? throw new Exception($"No wallet for user {userId}");
            walletEntity.Points -= amount;
            context.SaveChanges();
        }

        public async Task AddNewWallet(int userId)
        {
            var walletEntity = new SteamHub.Api.Entities.Wallet
            {
                UserId = userId,
                Points = 0,
                Balance = 0m
            };
            await context.Wallets.AddAsync(walletEntity);
            context.SaveChanges();
        }

        public async Task RemoveWallet(int userId)
        {
            var walletEntity = await context.Wallets.SingleOrDefaultAsync(w => w.UserId == userId);
            if (walletEntity != null)
            {
                context.Wallets.Remove(walletEntity);
                context.SaveChanges();
            }
        }

        public async Task WinPoints(int amount, int userId)
        {
            await AddPointsToWallet(amount, userId);
        }
    }
}
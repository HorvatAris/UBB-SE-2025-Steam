using System;
using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.DataContext;

namespace BusinessLayer.Repositories
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext context;

        public WalletRepository(ApplicationDbContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public Wallet GetWallet(int walletId)
        {
            var wallet = context.Wallets.Find(walletId);
            if (wallet == null)
            {
                throw new RepositoryException($"Wallet with ID {walletId} not found.");
            }
            return wallet;
        }

        public int GetWalletIdByUserId(int userId)
        {
            var wallet = context.Wallets
                .Where(w => w.UserId == userId)
                .Select(w => w.WalletId)
                .FirstOrDefault();
            if (wallet == 0)
            {
                throw new RepositoryException($"Wallet for user with ID {userId} not found.");
            }
            return wallet;
        }

        public void AddMoneyToWallet(decimal moneyToAdd, int userId)
        {
            var wallet = context.Wallets.SingleOrDefault(w => w.UserId == userId)
                ?? throw new RepositoryException($"No wallet for user {userId}");
            wallet.Balance += moneyToAdd;
            context.SaveChanges();
        }

        public void AddPointsToWallet(int pointsToAdd, int userId)
        {
            var wallet = context.Wallets.SingleOrDefault(w => w.UserId == userId)
                ?? throw new RepositoryException($"No wallet for user {userId}");
            wallet.Points += pointsToAdd;
            context.SaveChanges();
        }

        public decimal GetMoneyFromWallet(int walletId)
        {
            return GetWallet(walletId).Balance;
        }

        public int GetPointsFromWallet(int walletId)
        {
            return GetWallet(walletId).Points;
        }

        public void BuyWithMoney(decimal amount, int userId)
        {
            var wallet = context.Wallets.SingleOrDefault(w => w.UserId == userId)
                ?? throw new RepositoryException($"No wallet for user {userId}");
            wallet.Balance -= amount;
            context.SaveChanges();
        }

        public void BuyWithPoints(int amount, int userId)
        {
            var wallet = context.Wallets.SingleOrDefault(w => w.UserId == userId)
                ?? throw new RepositoryException($"No wallet for user {userId}");
            wallet.Points -= amount;
            context.SaveChanges();
        }

        public void AddNewWallet(int userId)
        {
            var wallet = new Wallet
            {
                UserId = userId,
                Points = 0,
                Balance = 0m
            };
            context.Wallets.Add(wallet);
            context.SaveChanges();
        }

        public void RemoveWallet(int userId)
        {
            var wallet = context.Wallets.SingleOrDefault(w => w.UserId == userId);
            if (wallet != null)
            {
                context.Wallets.Remove(wallet);
                context.SaveChanges();
            }
        }

        public void WinPoints(int amount, int userId)
        {
            AddPointsToWallet(amount, userId);
        }
    }
}
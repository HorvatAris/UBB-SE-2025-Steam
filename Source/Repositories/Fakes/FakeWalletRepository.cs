using System;
using System.Collections.Generic;
using BusinessLayer.Models;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Repositories.Fakes
{
    public class FakeWalletRepository : IWalletRepository
    {
        private readonly Dictionary<int, Wallet> wallets;
        private readonly Dictionary<int, int> userWalletMapping;

        public FakeWalletRepository()
        {
            wallets = new Dictionary<int, Wallet>();
            userWalletMapping = new Dictionary<int, int>();

            // Initialize with test data
            var wallet1 = new Wallet
            {
                WalletId = 1,
                UserId = 1,
                Balance = 100.00m,
                Points = 50
            };

            var wallet2 = new Wallet
            {
                WalletId = 2,
                UserId = 2,
                Balance = 200.00m,
                Points = 75
            };

            wallets.Add(wallet1.WalletId, wallet1);
            wallets.Add(wallet2.WalletId, wallet2);

            userWalletMapping.Add(wallet1.UserId, wallet1.WalletId);
            userWalletMapping.Add(wallet2.UserId, wallet2.WalletId);
        }

        public Wallet GetWallet(int walletId)
        {
            if (wallets.TryGetValue(walletId, out var wallet))
            {
                return wallet;
            }

            throw new RepositoryException($"Failed to retrieve wallet with ID {walletId} from the database.");
        }

        public int GetWalletIdByUserId(int userId)
        {
            if (userWalletMapping.TryGetValue(userId, out var walletId))
            {
                return walletId;
            }

            throw new RepositoryException($"No wallet found for user ID {userId}.");
        }

        public void AddMoneyToWallet(decimal moneyToAdd, int walletId)
        {
            if (wallets.TryGetValue(walletId, out var wallet))
            {
                wallet.Balance += moneyToAdd;
            }
            else
            {
                throw new RepositoryException($"Failed to add money to wallet. Wallet ID {walletId} not found.");
            }
        }

        public void AddPointsToWallet(int pointsToAdd, int walletId)
        {
            if (wallets.TryGetValue(walletId, out var wallet))
            {
                wallet.Points += pointsToAdd;
            }
            else
            {
                throw new RepositoryException($"Failed to add points to wallet. Wallet ID {walletId} not found.");
            }
        }

        public decimal GetMoneyFromWallet(int walletId)
        {
            if (wallets.TryGetValue(walletId, out var wallet))
            {
                return wallet.Balance;
            }

            throw new RepositoryException($"Failed to get money from wallet. Wallet ID {walletId} not found.");
        }

        public int GetPointsFromWallet(int walletId)
        {
            if (wallets.TryGetValue(walletId, out var wallet))
            {
                return wallet.Points;
            }

            throw new RepositoryException($"Failed to get points from wallet. Wallet ID {walletId} not found.");
        }

        public void PurchasePoints(PointsOffer offer, int walletId)
        {
            if (wallets.TryGetValue(walletId, out var wallet))
            {
                if (wallet.Balance >= offer.Price)
                {
                    wallet.Balance -= offer.Price;
                    wallet.Points += offer.Points;
                }
                else
                {
                    throw new InvalidOperationException("Insufficient funds to purchase points.");
                }
            }
            else
            {
                throw new RepositoryException($"Failed to purchase points. Wallet ID {walletId} not found.");
            }
        }

        public void AddNewWallet(int userId)
        {
            // Generate a new wallet ID
            int newWalletId = wallets.Count + 1;
            while (wallets.ContainsKey(newWalletId))
            {
                newWalletId++;
            }

            var newWallet = new Wallet
            {
                WalletId = newWalletId,
                UserId = userId,
                Balance = 0,
                Points = 0
            };

            wallets.Add(newWalletId, newWallet);
            userWalletMapping.Add(userId, newWalletId);
        }

        public void RemoveWallet(int userId)
        {
            if (userWalletMapping.TryGetValue(userId, out var walletId))
            {
                wallets.Remove(walletId);
                userWalletMapping.Remove(userId);
            }
        }
    }
}
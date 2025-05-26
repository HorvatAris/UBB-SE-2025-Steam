using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Models;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IWalletRepository
    {
        Wallet GetWallet(int walletId);

        int GetWalletIdByUserId(int userId);

        void AddMoneyToWallet(decimal amount, int walletId);

        void AddPointsToWallet(int amount, int walletId);

        decimal GetMoneyFromWallet(int walletId);

        int GetPointsFromWallet(int walletId);

        void PurchasePoints(PointsOffer offer, int walletId);

        void AddNewWallet(int userId);

        void RemoveWallet(int userId);
    }
}

using SteamHub.ApiContract.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamHub.ApiContract.Repositories
{
    public interface IWalletRepository
    {
        Task<Wallet> GetWallet(int walletId);

        Task<int> GetWalletIdByUserId(int userId);

        Task AddMoneyToWallet(decimal amount, int walletId);

        Task AddPointsToWallet(int amount, int walletId);

        Task<decimal> GetMoneyFromWallet(int walletId);

        Task<int> GetPointsFromWallet(int walletId);

        Task AddNewWallet(int userId);

        Task RemoveWallet(int userId);
        Task BuyWithMoney(decimal amount, int userId);
        Task BuyWithPoints(int amount, int userId);
    }
}

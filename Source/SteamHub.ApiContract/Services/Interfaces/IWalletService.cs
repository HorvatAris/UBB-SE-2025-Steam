using SteamHub.ApiContract.Models.User;

namespace SteamHub.ApiContract.Services.Interfaces
{
    public interface IWalletService
    {
        Task CreateWallet(int userIdentifier);
        Task<decimal> GetBalance(int userId);
        Task<int> GetPoints(int userId);
        Task AddMoney(decimal amount, int userId);
        Task CreditPoints(int userId, int numberOfPoints);
        Task BuyWithMoney(decimal amount, int userId);
    }
}

using BusinessLayer.Models;

namespace BusinessLayer.Services.Interfaces
{
    public interface IWalletService
    {
        void CreateWallet(int userIdentifier);
        decimal GetBalance();
        int GetPoints();
        void AddMoney(decimal amount);
        void CreditPoints(int userId, int numberOfPoints);
        void BuyWithMoney(decimal amount, int userId);
    }
}

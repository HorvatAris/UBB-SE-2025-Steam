using BusinessLayer.Models;
using BusinessLayer.Services.Interfaces;

namespace BusinessLayer.Services.Fakes
{
    public class FakeWalletService : IWalletService
    {
        private decimal walletBalance = 100m;
        private int points = 0;
        public List<string> Actions = new();

        public void AddMoney(decimal amount)
        {
            walletBalance += amount;
        }

        public void CreditPoints(int userId, int numberOfPoints)
        {
            this.points += numberOfPoints;
        }

        public decimal GetBalance()
        {
            return walletBalance;
        }

        public int GetPoints()
        {
            return points;
        }

        public void CreateWallet(int userId)
        {
        }

        public void BuyWithMoney(decimal amount, int userId)
        {
        }
    }
}

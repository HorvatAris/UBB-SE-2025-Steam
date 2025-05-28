using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;

namespace SteamHub.ApiContext.Services
{
    public class WalletService : IWalletService
    {
        private readonly IWalletRepository walletRepository;
        private readonly IUserDetails user;

        public WalletService(IWalletRepository walletRepository)
        {
            this.walletRepository = walletRepository ?? throw new ArgumentNullException(nameof(walletRepository));
        }


        public async Task AddMoney(decimal amount, int userId)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");
            }
            if (amount > 500)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount cannot be greater than 500.");
            }
            await walletRepository.AddMoneyToWallet(amount, userId);
        }

        public async Task CreditPoints(int userId, int numberOfPoints)
        {
            await walletRepository.AddPointsToWallet(numberOfPoints, userId);
        }

        public async Task<decimal> GetBalance(int userId)
        {

            try
            {
                int walletIdentifier = await walletRepository.GetWalletIdByUserId(userId);
                return await walletRepository.GetMoneyFromWallet(walletIdentifier);
            }
            catch (Exception ex) when (ex.Message.Contains("No wallet found"))
            {
                // No wallet found, create one
                await CreateWallet(userId);
                return 0m; // New wallet has 0 balance
            }
        }

        public async Task<int> GetPoints(int userId)
        {

            try
            {
                int walletId = await walletRepository.GetWalletIdByUserId(userId);
                return await walletRepository.GetPointsFromWallet(walletId);
            }
            catch (Exception ex) when (ex.Message.Contains("No wallet found"))
            {
                // No wallet found, create one
                await CreateWallet(userId);
                return 0; // New wallet has 0 points
            }
        }

        public async Task CreateWallet(int userIdentifier)
        {
            try
            {
                // Check if a wallet already exists
                await walletRepository.GetWalletIdByUserId(userIdentifier);
                // If GetWalletIdByUserId does not throw, a wallet exists.
            }
            catch (Exception ex) when (ex.Message.Equals($"Wallet for user with ID {userIdentifier} not found.", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("not found"))
            {
                // No wallet found, so create one
                await walletRepository.AddNewWallet(userIdentifier);
            }
        }

        public async Task BuyWithMoney(decimal amount, int userId)
        {
            if (amount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");
            }
            await walletRepository.BuyWithMoney(amount, userId);
        }

        public IUserDetails GetUser()
        {
            return user;
        }
    }
}
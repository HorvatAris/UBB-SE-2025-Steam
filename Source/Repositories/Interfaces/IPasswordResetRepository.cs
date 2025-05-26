using System;

namespace BusinessLayer.Repositories.Interfaces
{
    public interface IPasswordResetRepository
    {
        void StoreResetCode(int userId, string code, DateTime expiryTime);
        bool VerifyResetCode(string email, string code);
        bool ValidateResetCode(string email, string code);
        bool ResetPassword(string email, string code, string hashedPassword);
        void CleanupExpiredCodes();
    }
}
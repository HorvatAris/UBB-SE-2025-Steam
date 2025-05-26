using System.Threading.Tasks;

namespace BusinessLayer.Services.Interfaces
{
    public interface IPasswordResetService
    {
        Task<(bool isValid, string message)> SendResetCode(string email);
        (bool isValid, string message) VerifyResetCode(string email, string code);
        (bool isValid, string message) ResetPassword(string email, string code, string newPassword);
        void CleanupExpiredCodes();
    }
}
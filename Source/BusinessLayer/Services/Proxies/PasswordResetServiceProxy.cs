using System;
using System.Threading.Tasks;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Exceptions;

namespace BusinessLayer.Services.Proxies
{
    public class PasswordResetServiceProxy : ServiceProxy, IPasswordResetService
    {
        public PasswordResetServiceProxy(string baseUrl = "https://localhost:7262/api/")
            : base(baseUrl)
        {
        }

        public async Task<(bool isValid, string message)> SendResetCode(string email)
        {
            try
            {
                var response = await PostAsync<ResetCodeResponse>("PasswordReset/send", new { Email = email });
                return (response.IsValid, response.Message);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to send reset code: {ex.Message}");
            }
        }

        public (bool isValid, string message) VerifyResetCode(string email, string code)
        {
            try
            {
                var response = PostAsync<ResetCodeResponse>("PasswordReset/verify", new
                {
                    Email = email,
                    Code = code
                }).GetAwaiter().GetResult();

                return (response.IsValid, response.Message);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to verify reset code: {ex.Message}");
            }
        }

        public (bool isValid, string message) ResetPassword(string email, string code, string newPassword)
        {
            try
            {
                var response = PostAsync<ResetCodeResponse>("PasswordReset/reset", new
                {
                    Email = email,
                    Code = code,
                    NewPassword = newPassword
                }).GetAwaiter().GetResult();

                return (response.IsValid, response.Message);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to reset password: {ex.Message}");
            }
        }

        public void CleanupExpiredCodes()
        {
            // This is a server-side operation that doesn't need to be proxied
            try
            {
                PostAsync("PasswordReset/cleanup", null).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                // Ignore errors
            }
        }

        private class ResetCodeResponse
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
        }
    }
}
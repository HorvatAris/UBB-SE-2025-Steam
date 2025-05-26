using System;
using System.IO;
using System.Threading.Tasks;
using BusinessLayer.Exceptions;
using BusinessLayer.Services.Interfaces;
using BusinessLayer.Validators;
using BusinessLayer.Repositories;
using BusinessLayer.Repositories.Interfaces;

namespace BusinessLayer.Services
{
    public class PasswordResetService : IPasswordResetService
    {
        private readonly string resetCodesPath;
        private readonly IUserService userService;
        private readonly PasswordResetValidator passwordResetValidator;
        private readonly IPasswordResetRepository passwordResetRepository;

        public PasswordResetService(IPasswordResetRepository passwordResetRepository, IUserService userService)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            passwordResetValidator = new PasswordResetValidator();
            resetCodesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ResetCodes");
            Directory.CreateDirectory(resetCodesPath);
            this.passwordResetRepository = passwordResetRepository ?? throw new ArgumentNullException(nameof(passwordResetRepository));
        }

        public async Task<(bool isValid, string message)> SendResetCode(string email)
        {
            var validationResult = passwordResetValidator.ValidateEmail(email);
            if (!validationResult.isValid)
            {
                throw new InvalidOperationException(validationResult.errorMessage);
            }

            var user = userService.GetUserByEmail(email);
            if (user == null)
            {
                return (false, "Email is not registered.");
            }

            try
            {
                CleanupExpiredCodes();

                var code = GenerateResetCode();
                var filePath = GetResetCodeFilePath(email);
                // Store code with expiration time
                var expiryTime = DateTime.UtcNow.AddMinutes(15);
                var fileContent = $"{code}|{expiryTime:O}";
                await File.WriteAllTextAsync(filePath, fileContent);

                // Set up auto-deletion after 15 minutes
                _ = Task.Delay(TimeSpan.FromMinutes(15)).ContinueWith(_ =>
                {
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                });

                return (true, "Reset code sent successfully.");
            }
            catch (Exception exception)
            {
                return (false, $"Failed to send reset code: {exception.Message}");
            }
        }

        public (bool isValid, string message) VerifyResetCode(string email, string code)
        {
            var emailValidation = passwordResetValidator.ValidateEmail(email);
            if (!emailValidation.isValid)
            {
                throw new InvalidOperationException(emailValidation.errorMessage);
            }

            var codeValidation = passwordResetValidator.ValidateResetCode(code);
            if (!codeValidation.isValid)
            {
                throw new InvalidOperationException(codeValidation.errorMessage);
            }

            try
            {
                var filePath = GetResetCodeFilePath(email);
                if (!File.Exists(filePath))
                {
                    return (false, "Reset code has expired or does not exist.");
                }

                var fileContent = File.ReadAllText(filePath).Trim();
                var parts = fileContent.Split('|');

                if (parts.Length != 2 || !DateTime.TryParse(parts[1].Trim(), out DateTime expiryTime))
                {
                    // Invalid file format
                    return (false, "Invalid reset code.");
                }

                var storedCode = parts[0];
                if (storedCode != code)
                {
                    return (false, "Invalid reset code.");
                }

                if (expiryTime < DateTime.UtcNow)
                {
                    return (false, "Invalid or expired reset code.");
                }

                return (true, "Code verified successfully.");
            }
            catch (Exception exception)
            {
                return (false, $"Failed to verify reset code: {exception.Message}");
            }
        }

        public (bool isValid, string message) ResetPassword(string email, string code, string newPassword)
        {
            try
            {
                var emailValidation = passwordResetValidator.ValidateEmail(email);
                if (!emailValidation.isValid)
                {
                    throw new InvalidOperationException(emailValidation.errorMessage);
                }

                var codeValidation = passwordResetValidator.ValidateResetCode(code);
                if (!codeValidation.isValid)
                {
                    throw new InvalidOperationException(codeValidation.errorMessage);
                }

                var passwordValidation = passwordResetValidator.ValidatePassword(newPassword);
                if (!passwordValidation.isValid)
                {
                    return (false, passwordValidation.errorMessage);
                }

                var verificationResult = VerifyResetCode(email, code);
                if (!verificationResult.isValid)
                {
                    // Always return 'Invalid or expired reset code.' for expired codes
                    if (verificationResult.message == "Invalid reset code." ||
                        verificationResult.message == "Reset code has expired or does not exist.")
                    {
                        return (false, "Invalid or expired reset code.");
                    }
                    return verificationResult;
                }

                var user = userService.GetUserByEmail(email);
                if (user == null)
                {
                    return (false, "User not found.");
                }

                userService.UpdateUserPassword(user.UserId, newPassword);
                File.Delete(GetResetCodeFilePath(email));
                return (true, "Password reset successfully.");
            }
            catch (Exception exception)
            {
                return (false, $"Failed to reset password: {exception.Message}");
            }
        }

        public void CleanupExpiredCodes()
        {
            if (!Directory.Exists(resetCodesPath))
            {
                Directory.CreateDirectory(resetCodesPath);
                return;
            }

            string[] filePaths = null;
            try
            {
                filePaths = Directory.GetFiles(resetCodesPath);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"Error accessing directory: {exception.Message}");
                return;
            }

            foreach (string file in filePaths)
            {
                if (!File.Exists(file))
                {
                    continue;
                }

                string fileContent = string.Empty;
                try
                {
                    using (var streamReader = new StreamReader(file))
                    {
                        fileContent = streamReader.ReadToEnd().Trim();
                    }

                    string[] parts = fileContent.Split('|');
                    bool shouldDelete = false;

                    if (parts.Length == 2 && DateTime.TryParse(parts[1].Trim(), out DateTime expiryTime))
                    {
                        shouldDelete = expiryTime < DateTime.UtcNow;
                    }
                    else
                    {
                        // Invalid format, check file creation time
                        var fileInfo = new FileInfo(file);
                        shouldDelete = fileInfo.CreationTimeUtc < DateTime.UtcNow.AddMinutes(-15);
                    }

                    if (shouldDelete)
                    {
                        try
                        {
                            File.SetAttributes(file, FileAttributes.Normal); // Remove any read-only attributes
                            File.Delete(file);
                        }
                        catch (IOException)
                        {
                            try
                            {
                                // Wait a moment and try again
                                System.Threading.Thread.Sleep(100);
                                if (File.Exists(file))
                                {
                                    File.Delete(file);
                                }
                            }
                            catch
                            {
                                Console.WriteLine($"Failed to delete file: {file}");
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"Error processing file {file}: {exception.Message}");
                    try
                    {
                        // Try to delete by creation time if reading fails
                        if (new FileInfo(file).CreationTimeUtc < DateTime.UtcNow.AddMinutes(-15))
                        {
                            File.Delete(file);
                        }
                    }
                    catch
                    {
                        // Ignore deletion failures
                    }
                }
            }
        }

        private string GenerateResetCode()
        {
            var random = new Random();
            return random.Next(100000, 1000000).ToString().Substring(0, 6);
        }

        private string GetResetCodeFilePath(string email)
        {
            return Path.Combine(resetCodesPath, $"{email.ToLower()}_reset_code.txt");
        }
    }
}
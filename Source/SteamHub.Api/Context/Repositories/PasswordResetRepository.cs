using SteamHub.ApiContract.Models.PasswordReset;
using SteamHub.ApiContract.Repositories;

namespace SteamHub.Api.Context.Repositories
{
    /// <summary>
    /// Repository responsible for managing password reset operations such as storing,
    /// validating, and cleaning up reset codes.
    /// </summary>
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly DataContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordResetRepository"/> class.
        /// </summary>
        /// <param name="newContext">The database context.</param>
        /// <exception cref="ArgumentNullException">Thrown if the provided context is null.</exception>
        public PasswordResetRepository(DataContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        /// <summary>
        /// Stores a new reset code for the given user and removes any previous codes.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="code">The new reset code.</param>
        /// <param name="expiryTime">The expiration time of the code.</param>
        public void StoreResetCode(int userId, string code, DateTime expiryTime)
        {
            // Remove old codes
            var old = context.PasswordResetCodes.Where(p => p.UserId == userId);
            context.PasswordResetCodes.RemoveRange(old);

            // Add new code
            var newCode = new PasswordResetCode
            {
                UserId = userId,
                ResetCode = code,
                ExpirationTime = expiryTime,
                Used = false
            };

            context.PasswordResetCodes.Add(newCode);
            context.SaveChanges();
        }

        /// <summary>
        /// Verifies whether the provided reset code is valid and unexpired for the given email.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The reset code to verify.</param>
        /// <returns><c>true</c> if the code is valid and not expired; otherwise, <c>false</c>.</returns>
        public bool VerifyResetCode(string email, string code)
        {
            var user = context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                return false;
            }

            var resetCode = context.PasswordResetCodes
                .SingleOrDefault(p => p.UserId == user.UserId && p.ResetCode == code);

            return resetCode != null
                && !resetCode.Used
                && resetCode.ExpirationTime > DateTime.Now;
        }

        /// <summary>
        /// Validates and consumes a reset code, marking it as used.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The reset code to validate.</param>
        /// <returns><c>true</c> if the code was valid and has been marked as used; otherwise, <c>false</c>.</returns>
        public bool ValidateResetCode(string email, string code)
        {
            var user = context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                return false;
            }

            var resetCode = context.PasswordResetCodes
                .SingleOrDefault(p => p.UserId == user.UserId && p.ResetCode == code && !p.Used && p.ExpirationTime > DateTime.Now);
            if (resetCode == null)
            {
                return false;
            }

            // Mark code as used
            resetCode.Used = true;
            context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Resets the user's password if the provided code is valid and unexpired.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="code">The reset code.</param>
        /// <param name="hashedPassword">The new hashed password.</param>
        /// <returns><c>true</c> if the password was reset successfully; otherwise, <c>false</c>.</returns>
        public bool ResetPassword(string email, string code, string hashedPassword)
        {
            var user = context.Users.SingleOrDefault(u => u.Email == email);
            if (user == null)
            {
                return false;
            }

            var resetCode = context.PasswordResetCodes
                .SingleOrDefault(p => p.UserId == user.UserId && p.ResetCode == code && !p.Used && p.ExpirationTime > DateTime.Now);

            if (resetCode == null)
            {
                return false;
            }

            // Mark code as used and update password
            resetCode.Used = true;
            user.Password = hashedPassword;

            context.SaveChanges();
            return true;
        }

        /// <summary>
        /// Deletes all expired password reset codes from the database.
        /// </summary>
        public void CleanupExpiredCodes()
        {
            var expired = context.PasswordResetCodes
                .Where(p => p.ExpirationTime < DateTime.Now)
                .ToList();

            context.PasswordResetCodes.RemoveRange(expired);
            context.SaveChanges();
        }
    }
}

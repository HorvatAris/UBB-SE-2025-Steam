using System;
using System.Data;
using BusinessLayer.Data;
using Microsoft.Data.SqlClient;
using BusinessLayer.Exceptions;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.DataContext;
using BusinessLayer.Models;

namespace BusinessLayer.Repositories
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly ApplicationDbContext context;

        public PasswordResetRepository(ApplicationDbContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public void StoreResetCode(int userId, string code, DateTime expiryTime)
        {
            // Remove old codes
            var old = context.PasswordResetCodes.Where(p => p.UserId == userId);
            context.PasswordResetCodes.RemoveRange(old);

            // Add new
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

            // Mark code used and update password
            resetCode.Used = true;
            user.Password = hashedPassword;

            context.SaveChanges();
            return true;
        }

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

            // Mark code used
            resetCode.Used = true;
            context.SaveChanges();
            return true;
        }

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
using System.Data;
using BusinessLayer.Data;
using BusinessLayer.Models;
using BusinessLayer.Utils;
using Microsoft.Data.SqlClient;
using BusinessLayer.Repositories.Interfaces;
using BusinessLayer.Exceptions;
using BusinessLayer.DataContext;

namespace BusinessLayer.Repositories
{
    public sealed class UsersRepository : IUsersRepository
    {
        private readonly ApplicationDbContext context;

        private readonly string email_exists = "EMAIL_EXISTS";
        private readonly string username_exists = "USERNAME_EXISTS";

        public UsersRepository(ApplicationDbContext newContext)
        {
            this.context = newContext ?? throw new ArgumentNullException(nameof(newContext));
        }

        public List<User> GetAllUsers()
        {
            return context.Users.OrderBy(u => u.Username).ToList();
        }

        public User? GetUserById(int userId)
        {
            return context.Users.Find(userId);
        }

        public User UpdateUser(User user)
        {
            var existing = context.Users.Find(user.UserId)
                ?? throw new RepositoryException($"User with ID {user.UserId} not found.");

            existing.Email = user.Email;
            existing.Username = user.Username;
            existing.IsDeveloper = user.IsDeveloper;
            context.SaveChanges();
            return existing;
        }

        public User CreateUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
            return user;
        }

        public void DeleteUser(int userId)
        {
            var user = context.Users.Find(userId);
            if (user != null)
            {
                context.Users.Remove(user);
                context.SaveChanges();
            }
        }

        public User? VerifyCredentials(string emailOrUsername)
        {
            return context.Users.SingleOrDefault(u => u.Username == emailOrUsername || u.Email == emailOrUsername);
        }

        public User? GetUserByEmail(string email)
        {
            return context.Users.SingleOrDefault(u => u.Email == email);
        }

        public User? GetUserByUsername(string username)
        {
            return context.Users.SingleOrDefault(u => u.Username == username);
        }

        public string CheckUserExists(string email, string username)
        {
            if (context.Users.Any(u => u.Email == email))
            {
                return email_exists;
            }
            if (context.Users.Any(u => u.Username == username))
            {
                return username_exists;
            }
            return null;
        }

        public void ChangeEmail(int userId, string newEmail)
        {
            var user = context.Users.Find(userId)
                ?? throw new RepositoryException($"User with ID {userId} not found.");
            user.Email = newEmail;
            context.SaveChanges();
        }

        public void ChangePassword(int userId, string newPassword)
        {
            var user = context.Users.Find(userId)
                ?? throw new RepositoryException($"User with ID {userId} not found.");
            user.Password = PasswordHasher.HashPassword(newPassword);
            context.SaveChanges();
        }

        public void ChangeUsername(int userId, string newUsername)
        {
            var user = context.Users.Find(userId)
                ?? throw new RepositoryException($"User with ID {userId} not found.");
            user.Username = newUsername;
            context.SaveChanges();
        }

        public void UpdateLastLogin(int userId)
        {
            var user = context.Users.Find(userId)
                ?? throw new RepositoryException($"User with ID {userId} not found.");
            user.LastLogin = DateTime.UtcNow;
            context.SaveChanges();
        }

        // Left these here for test purposes ??
        public User? MapDataRowToUser(DataRow row)
        {
            if (row["user_id"] == DBNull.Value || row["email"] == DBNull.Value || row["username"] == DBNull.Value)
            {
                return null;
            }

            return new User
            {
                UserId = Convert.ToInt32(row["user_id"]),
                Username = row["username"].ToString(),
                Email = row["email"].ToString(),
                IsDeveloper = row["developer"] != DBNull.Value ? Convert.ToBoolean(row["developer"]) : false,
                CreatedAt = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : DateTime.MinValue,
                LastLogin = row["last_login"] != DBNull.Value ? row["last_login"] as DateTime? : null
            };
        }

        public User? MapDataRowToUserWithPassword(DataRow row)
        {
            if (row["user_id"] == DBNull.Value || row["email"] == DBNull.Value || row["username"] == DBNull.Value || row["hashed_password"] == DBNull.Value)
            {
                return null;
            }

            var user = new User
            {
                UserId = Convert.ToInt32(row["user_id"]),
                Email = row["email"].ToString(),
                Username = row["username"].ToString(),
                IsDeveloper = row["developer"] != DBNull.Value ? Convert.ToBoolean(row["developer"]) : false,
                CreatedAt = row["created_at"] != DBNull.Value ? Convert.ToDateTime(row["created_at"]) : DateTime.MinValue,
                LastLogin = row["last_login"] != DBNull.Value ? row["last_login"] as DateTime? : null,
                Password = row["hashed_password"].ToString()
            };

            return user;
        }
    }
}
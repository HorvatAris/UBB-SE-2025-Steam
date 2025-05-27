using SteamHub.ApiContract.Exceptions;
using SteamHub.ApiContract.Models.User;
using SteamHub.ApiContract.Models.Session;
using SteamHub.ApiContract.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SteamHub.ApiContract.Proxies;
using SteamHub.ApiContract.Repositories;
using SteamHub.ApiContract.Services.Interfaces;
using SteamHub.ApiContract.Models.Common;

namespace SteamHub.ApiContract.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;
        private readonly ISessionRepository sessionRepository;

        public UserService(IUserRepository userRepository, ISessionRepository sessionRepository)
        {
            this.userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            this.sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        }

        public List<User> GetAllUsers()
            => userRepository.GetAllUsers();

        public User GetUserByIdentifier(int userId)
            => userRepository.GetUserById(userId);

        public User GetUserByEmail(string email)
            => userRepository.GetUserByEmail(email);

        public User? GetUserByUsername(string username)
            => userRepository.GetUserByUsername(username);

        public void ValidateUserAndEmail(string email, string username)
        {
            var errorType = userRepository.CheckUserExists(email, username);
            if (errorType == "EMAIL_EXISTS") throw new EmailAlreadyExistsException(email);
            if (errorType == "USERNAME_EXISTS") throw new UsernameAlreadyTakenException(username);
        }

        public User CreateUser(User user)
        {
            ValidateUserAndEmail(user.Email, user.Username);
            user.Password = PasswordHasher.HashPassword(user.Password);
            return userRepository.CreateUser(user);
        }

        public User UpdateUser(User user)
            => userRepository.UpdateUser(user);

        public void DeleteUser(int userId)
        {
            sessionRepository.DeleteUserSessions(userId).Wait();
            userRepository.DeleteUser(userId);
        }

        public bool AcceptChanges(int userId, string givenPassword)
        {
            var user = userRepository.GetUserById(userId);
            return PasswordHasher.VerifyPassword(givenPassword, user.Password);
        }

        public void UpdateUserEmail(int userId, string newEmail)
            => userRepository.ChangeEmail(userId, newEmail);

        public void UpdateUserPassword(int userId, string newPassword)
            => userRepository.ChangePassword(userId, newPassword);

        public void UpdateUserUsername(int userId, string newUsername)
            => userRepository.ChangeUsername(userId, newUsername);

        public async Task<User?> LoginAsync(string emailOrUsername, string password)
        {
            var user = userRepository.VerifyCredentials(emailOrUsername);
            if (user != null && PasswordHasher.VerifyPassword(password, user.Password))
            {
                var sessionDetails = await sessionRepository.CreateSession(user.UserId);
                userRepository.UpdateLastLogin(user.UserId);
                return user;
            }
            return null;
        }

        public async Task LogoutAsync()
        {
            var currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                await sessionRepository.DeleteUserSessions(currentUser.UserId);
            }
        }

        public User? GetCurrentUser()
        {
            var sessionId = UserSession.Instance.CurrentSessionId;
            if (!sessionId.HasValue)
                return null;

            var userWithSession = sessionRepository.GetUserFromSession(sessionId.Value).Result;
            if (userWithSession == null)
                return null;

            return new User
            {
                UserId = userWithSession.UserId,
                Username = userWithSession.Username,
                Email = userWithSession.Email,
                UserRole = userWithSession.Developer ? UserRole.Developer : UserRole.User
            };
        }

        public bool IsUserLoggedIn()
        {
            var sessionId = UserSession.Instance.CurrentSessionId;
            if (!sessionId.HasValue)
                return false;

            var session = sessionRepository.GetSessionById(sessionId.Value).Result;
            return session != null && session.ExpiresAt > DateTime.UtcNow;
        }
        
        public bool UpdateUserUsername(string username, string currentPassword)
        {
            if (!VerifyUserPassword(currentPassword)) return false;
            userRepository.ChangeUsername(GetCurrentUser().UserId, username);
            return true;
        }

        public bool UpdateUserPassword(string newPassword, string currentPassword)
        {
            if (!VerifyUserPassword(currentPassword)) return false;
            userRepository.ChangePassword(GetCurrentUser().UserId, newPassword);
            return true;
        }

        public bool UpdateUserEmail(string newEmail, string currentPassword)
        {
            if (!VerifyUserPassword(currentPassword)) return false;
            userRepository.ChangeEmail(GetCurrentUser().UserId, newEmail);
            return true;
        }

        public bool VerifyUserPassword(string password)
        {
            var currentEmail = GetCurrentUser().Email;
            var user = userRepository.VerifyCredentials(currentEmail);
            return user != null && PasswordHasher.VerifyPassword(password, user.Password);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            var dto = await this.userRepository.GetUsersAsync();
            var result = new List<User>();
            foreach (var u in dto.Users)
            {
                result.Add(new User
                {
                    UserId = u.UserId,
                    Username = u.UserName,
                    Password = u.Password,
                    Email = u.Email,
                    WalletBalance = u.WalletBalance,
                    PointsBalance = u.PointsBalance,
                    UserRole = u.UserRole,
                    CreatedAt = u.CreatedAt,
                    LastLogin = u.LastLogin,
                    ProfilePicture = u.ProfilePicture,
                });   
            }
            return result;
        }
    }
}